using backend_01.Model;
using backend_01.Models;
using backend_01.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace backend_01.Controllers
{
    public class FeladatController : ApiController
    {
        WebContext ctx;
        public FeladatController()
        {
            ctx = new WebContext();
        }


        
        // GET api/<controller> //egyszerű get - nagyGet 
        //[TokenAuthorize()]
        [HttpGet]
        [Route("api/feladat")]
        public IHttpActionResult Get()
        {
            var feladatok = ctx.Feladat
                .Include("Felhasznalo")
                .Select(f => new
                {
                    f.Task_Id,
                    f.Statusz,
                    f.Helyszin,
                    f.Cim,
                    f.PosztDatum,
                    f.Hatarido,
                    f.Leiras,
                    f.Idotartam,
                    f.Fizetes,
                    Felhasznalo = new
                    {
                        f.Felhasznalo.User_Id,
                        f.Felhasznalo.Email,
                        f.Felhasznalo.SzulDat,
                        f.Felhasznalo.VezNev,
                        f.Felhasznalo.KerNev,
                        f.Felhasznalo.ProfilKep,
                        f.Felhasznalo.Bio,
                        f.Felhasznalo.RegDatum,
                        f.Felhasznalo.Felhtipus,
                        f.Felhasznalo.Telefonszam
                    }
                })
                .ToList();

            return Ok(feladatok); // 200 OK
        }



        //GET munkák ahol a státusz nyitott, mentett-e(?) 
        [HttpGet]
        [Route("api/mentettmunkak/{userId}")]
        public IHttpActionResult GetMentettMunkak(int userId)
        {
            var mentettMunkak = ctx.Mentes
                .Include("Feladat")
                .Where(m => m.User_Id == userId && m.Feladat.Statusz != "lezárt")
                .Select(m => new
                {
                    m.Task_Id,
                    Cim = m.Feladat.Cim,
                    Helyszin = m.Feladat.Helyszin,
                    Mentes = true
                })
                .ToList();

            return Ok(mentettMunkak);
        }

        [HttpGet]
        [Route("api/sajatmunkak/{id}")]
        public IHttpActionResult GetSajatMunkak(int id)
        {
            var sajatmunkak = ctx.Feladat.Where(k => k.User_Id == id).Select(a => new {
                a.Statusz,
                a.Task_Id,
                a.Cim,
                a.Fizetes,
                a.Helyszin,
                Ujjelentkezes = (ctx.Jelentkezesek.Count(j => j.Task_Id == a.Task_Id && j.Latta_e == 0 && a.Statusz=="függőben"))!=0
            }).ToList();
            return Ok(sajatmunkak);
        }
        [HttpGet]
        //[TokenAuthorize()]
        [Route("api/getmunka/{mid}/{uid}")]
        // GET api/<controller>/x   // csak egy munka van benne 
        public IHttpActionResult GetMunka(int mid,int uid)
        {
            var feladat = ctx.Feladat
                .Include("Felhasznalo")
                .Include("FeladatKategoriak.Kategoria")
                .Where(f => f.Task_Id == mid)
                .Select(f => new
                {
                    f.Task_Id,
                    f.Statusz,
                    f.Helyszin,
                    f.Cim,
                    f.PosztDatum,
                    f.Hatarido,
                    f.Leiras,
                    f.Idotartam,
                    f.Fizetes,
                    Felhasznalo = new
                    {
                        f.Felhasznalo.User_Id,
                        f.Felhasznalo.VezNev,
                        f.Felhasznalo.KerNev
                    },
                    FeladatKategoriak = f.FeladatKategoriak.Select(fk => new
                    {
                        fk.Kategoria.Kat_Id,
                        fk.Kategoria.Katnev
                    }).ToList(),
                    Jelentkezette = f.Jelentkezesek.Any(o => o.User_Id == uid)

                })
                .FirstOrDefault();

            if (feladat == null)
            {
                return NotFound();
            }

            return Ok(feladat);
        }
        [HttpGet]
        [TokenAuthorize()]
        [Route("api/munkakmentette/{userId}")]
        public IHttpActionResult GetMunkak(int userId)
        {
            var mentettTaskIds = ctx.Mentes
                .Where(m => m.User_Id == userId)
                .Select(m => m.Task_Id)
                .ToList();

            var munkak = ctx.Feladat
                .Where(f => f.Statusz != "lezárt")
                .Select(f => new
                {
                    f.Task_Id,
                    f.Cim,
                    f.Helyszin,
                    f.Fizetes,
                    Mentes = mentettTaskIds.Contains(f.Task_Id)
                })
                .ToList();

            return Ok(munkak);
        }

        
        [HttpGet]
        [Route("api/Feladat/Helyszinek")]
        [TokenAuthorize()]
        public IHttpActionResult GetHelyszinek()
        {
            try
            {
                var helyszinek = ctx.Feladat
                    .Select(f => f.Helyszin)
                    .Distinct()
                    .ToList();

                return Ok(helyszinek); // 200-as válasz az egyedi helyszínekkel
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // 500-as válasz, ha hiba lép fel
            }
        }
        //A !ModelState.IsValid:  az adatok érvénytelenek(mező hiányzik, formailag helytelen) //egy objektum kell nekem! 
        // POST api/<controller>/
        [TokenAuthorize()]
        [HttpPost]
        [Route("api/Feladat/uj")]
        public IHttpActionResult Post([FromBody] Feladat feladat)
        {
            if (feladat == null || !ModelState.IsValid) 
            {
                return BadRequest("Hibás adat."); //400
            }

            ctx.Feladat.Add(feladat);
            ctx.SaveChanges();

            return Ok(ctx.Feladat.Select(a => new { feladat.Task_Id})); //200
        }
        // PUT api/<controller>/x
        [TokenAuthorize()]
        public IHttpActionResult Put(int id, [FromBody] Feladat updatedFeladat)
        {
            if (updatedFeladat == null || !ModelState.IsValid)
                return BadRequest("Hibás adat."); //400 

            var existstask = ctx.Feladat.FirstOrDefault(m => m.Task_Id == id);
            if (existstask == null)
            {
                return NotFound();
            } //404
            else
            {
                existstask.Statusz = updatedFeladat.Statusz;
                existstask.Helyszin = updatedFeladat.Helyszin;
                existstask.Cim = updatedFeladat.Cim;
                existstask.Hatarido = updatedFeladat.Hatarido;
                existstask.Leiras = updatedFeladat.Leiras;
                existstask.Idotartam = updatedFeladat.Idotartam;
                existstask.Fizetes = updatedFeladat.Fizetes;

                ctx.SaveChanges();

                return Ok(existstask);
            }
        }
        // PATCH api/<controller>/x
        [TokenAuthorize()]
        [HttpPatch]
        [Route("api/Feladat/update/{id}")]
        public IHttpActionResult Patch(int id, [FromBody] Feladat updatedProperties)
        {
            if (updatedProperties == null || !ModelState.IsValid)
            {
                return BadRequest("Hiba."); //400
            }

            var existFeladat = ctx.Feladat.FirstOrDefault(m => m.Task_Id == id);
            if (existFeladat == null) {
                return NotFound(); //404 
            }

            if (Enum.IsDefined(typeof(FeladatStatus), updatedProperties.Statusz))
                existFeladat.Statusz = updatedProperties.Statusz;
            if (updatedProperties.Helyszin != null)
                existFeladat.Helyszin = updatedProperties.Helyszin;
            if (updatedProperties.Cim != null)
                existFeladat.Cim = updatedProperties.Cim;
            if (updatedProperties.Hatarido != null)
                existFeladat.Hatarido = updatedProperties.Hatarido;
            if (updatedProperties.Leiras != null)
                existFeladat.Leiras = updatedProperties.Leiras;
            if (updatedProperties.Idotartam != null)
                existFeladat.Idotartam = updatedProperties.Idotartam;
            if (updatedProperties.Fizetes != null)
                existFeladat.Fizetes = updatedProperties.Fizetes;
             

            ctx.SaveChanges();

            return Ok(existFeladat);//200
        }
        [TokenAuthorize()]
        [HttpPatch]
        [Route("api/Feladat/statusupdate/{id}/{status}")]
        public IHttpActionResult PatchStatus(int id, string status)
        {
            var updatedstatus = ctx.Feladat.Find(id);
            updatedstatus.Statusz = status; //azért mert van egy folyamatban státusz is, nem csak lezárt
            ctx.SaveChanges();
            return Ok();
        }
        // DELETE api/<controller>/x
        [TokenAuthorize()]
        public IHttpActionResult Delete(int id)
        {
            var feladat = ctx.Feladat.FirstOrDefault(m => m.Task_Id == id);
            if (feladat == null)
                return NotFound();
            try
            {
                var kapcsolatok = ctx.FeladatKategoria.Where(m => m.Task_Id == feladat.Task_Id).ToList();
                foreach (var item in kapcsolatok)
                {
                    ctx.FeladatKategoria.Remove(item);
                }
                ctx.Feladat.Remove(feladat);
                ctx.SaveChanges();

                return Ok(feladat);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);

            }
            
        }
    }
}
