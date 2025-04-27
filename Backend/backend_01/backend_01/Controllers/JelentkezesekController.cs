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
    public class JelentkezesekController : ApiController
    {
        WebContext ctx;
        public JelentkezesekController()
        {
            ctx = new WebContext();
        }
        //GET api/<controller>
        [TokenAuthorize()]
        public IHttpActionResult Get()
        {
            var jelentkezesek = ctx.Jelentkezesek
                .Include("Feladat")
                .Include("Felhasznalo")
                .Select(j => new
                {
                    j.Statusz,
                    j.Latta_e,
                    j.JelDatum,
                    j.Task_Id,
                    j.User_Id,
                    Feladat = new
                    {
                        j.Feladat.Task_Id,
                        j.Feladat.Cim,
                        j.Feladat.Statusz,
                        j.Feladat.Helyszin,
                        j.Feladat.PosztDatum,
                        j.Feladat.Hatarido
                    },
                    Felhasznalo = new
                    {
                        j.Felhasznalo.User_Id,
                        j.Felhasznalo.Email,
                        j.Felhasznalo.VezNev,
                        j.Felhasznalo.KerNev,
                        j.Felhasznalo.ProfilKep,
                        j.Felhasznalo.Telefonszam
                    }
                })
                .ToList();

            return Ok(jelentkezesek); // 200 OK
        }
        [TokenAuthorize()]
        [HttpGet]
        [Route("api/munkarajelentkezesek/{taskid}")]
        public IHttpActionResult GetMunkaraAdottJel(int taskid)
        {
            var munka = ctx.Jelentkezesek
                .Where(i => i.Feladat.Task_Id == taskid)
                .Select(a => new
                {
                    a.Statusz,
                    Felhasznalo = new
                    {
                        a.Felhasznalo.Telefonszam,
                        a.Felhasznalo.User_Id,
                        TeljesNev = a.Felhasznalo.VezNev + " " + a.Felhasznalo.KerNev
                    },
                    a.JelDatum,
                    a.Latta_e
                })
                .ToList();
            return Ok(munka);
        }
        [TokenAuthorize()]
        //GET api/<controller>/x
        public IHttpActionResult Get(int id, int id2)
        {
            var jelentkezes = ctx.Jelentkezesek
                .Include("Feladat")
                .Include("Felhasznalo")
                .Where(j => j.User_Id == id && j.Task_Id == id2)
                .Select(j => new
                {
                    j.Statusz,
                    j.Latta_e,
                    j.JelDatum,
                    j.Task_Id,
                    j.User_Id,
                    Feladat = new
                    {
                        j.Feladat.Task_Id,
                        j.Feladat.Cim,
                        j.Feladat.Statusz,
                        j.Feladat.Helyszin,
                        j.Feladat.PosztDatum,
                        j.Feladat.Hatarido
                    },
                    Felhasznalo = new
                    {
                        j.Felhasznalo.User_Id,
                        j.Felhasznalo.Email,
                        j.Felhasznalo.VezNev,
                        j.Felhasznalo.KerNev,
                        j.Felhasznalo.ProfilKep
                    }
                })
                .FirstOrDefault();

            if (jelentkezes == null)
            {
                return NotFound(); // 404
            }

            return Ok(jelentkezes); // 200 OK
        }
        [TokenAuthorize()]
        [HttpGet]
        [Route("api/jelentkezesek/{userId}")]
        public IHttpActionResult GetJelentkezesek(int userId)
        {
            var jelentkezesek = ctx.Jelentkezesek
                .Include("Feladat")
                .Include("Felhasznalo")
                .Where(j => j.User_Id == userId)
                .Select(j => new
                {
                    Feladat = new
                    {
                        j.Feladat.Task_Id,
                        j.Feladat.Cim,
                        j.Feladat.Helyszin,
                        j.Feladat.Hatarido,
                        j.Feladat.Fizetes,
                        j.Feladat.Statusz,
                        j.Feladat.Leiras,
                        j.Feladat.Idotartam,
                        j.Feladat.PosztDatum

                    },
                    Felhasznalo = new
                    {
                        j.Felhasznalo.User_Id,
                        j.Felhasznalo.VezNev,
                        j.Felhasznalo.KerNev
                    },
                    j.Statusz,
                    j.Latta_e,
                    j.JelDatum
                })
                .ToList();

            return Ok(jelentkezesek);
        }
        [TokenAuthorize()]
        [HttpGet]
        [Route("api/jelentkezesek/sajatmunkak/{userId}")]
        public IHttpActionResult getsajatmunkajel(int userId)
        {
            var jelentkezesek = ctx.Feladat.Where(k=> k.User_Id == userId).Select(o=>o.Jelentkezesek.Where(l=>l.Latta_e==0 && l.Statusz== "függőben")).ToList().Count;

            return Ok(jelentkezesek);
        }

        [TokenAuthorize()]
        //POST api/<controller>/*/
        [HttpPost]
        public IHttpActionResult Post([FromBody] int fid, int mid, string datum)
        {
            var a = new Jelentkezesek
            {
                Statusz = JelentkezesekStatus.függőben.ToString(),
                Latta_e = 0,
                JelDatum = datum,
                User_Id = fid,
                Task_Id = mid
            };
            try
            {
                ctx.Jelentkezesek.Add(a);
                ctx.SaveChanges();
                return Ok(a);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [TokenAuthorize()]
        [HttpPost]
        [Route("api/jelentkezes/uj")]
        public IHttpActionResult NewJelentkezes([FromBody] Jelentkezesek newjel)
        {
            if (newjel == null || newjel.Task_Id == 0 || newjel.User_Id == 0)
                return BadRequest("Hiányzó adatok");

            var letezik = ctx.Jelentkezesek
                .Any(j => j.Task_Id == newjel.Task_Id && j.User_Id == newjel.User_Id);

            if (letezik)
                return BadRequest("Erre a munkára már jelentkeztél.");

            var jelentkezes = new Jelentkezesek
            {
                Task_Id = newjel.Task_Id,
                User_Id = newjel.User_Id,
                JelDatum = newjel.JelDatum,
                Statusz = newjel.Statusz ?? "függőben",
                Latta_e = 0
            };

            ctx.Jelentkezesek.Add(jelentkezes);

            try
            {
                ctx.SaveChanges();
                return Ok(new
                {
                    jelentkezes.Task_Id,
                    jelentkezes.User_Id,
                    jelentkezes.Statusz
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //PUT api/<controller>/
        [TokenAuthorize()]
        [HttpPut]
        [Route("api/jelentkezes/action/{id}/{id2}")]
        public IHttpActionResult Put(int id, int id2, [FromBody] Jelentkezesek updatedJelentkezes)
        {
            if (updatedJelentkezes == null)
            {
                return BadRequest("Hiba.");
            }

            var existingJelentkezes = ctx.Jelentkezesek.FirstOrDefault(j => j.User_Id == id && j.Task_Id == id2);
            if (existingJelentkezes == null)
            {
                return NotFound();
            }

            try
            {
                existingJelentkezes.Statusz = updatedJelentkezes.Statusz;
                existingJelentkezes.Latta_e = updatedJelentkezes.Latta_e;

                ctx.SaveChanges();
                return Ok(existingJelentkezes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [TokenAuthorize()]
        [HttpPatch]
        [Route("api/jelentkezes/seen/{fid}/{mid}")]
        public IHttpActionResult SeenModify(int fid, int mid)
        {
            var jelentkezes = ctx.Jelentkezesek
                .FirstOrDefault(k => k.User_Id == fid && k.Task_Id == mid);

            if (jelentkezes == null)
            {
                return NotFound(); //404 
            }
            jelentkezes.Latta_e = 1;

            try
            {
                ctx.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [TokenAuthorize()]
        [HttpPatch]
        public IHttpActionResult StatusModify(int fid, int mid, string status)
        {
            var jelentkezes = ctx.Jelentkezesek
                .FirstOrDefault(k => k.User_Id == fid && k.Task_Id == mid);

            if (jelentkezes == null)
            {
                return NotFound(); //404 
            }
            jelentkezes.Statusz = status;

            try
            {
                ctx.SaveChanges();
                return Ok();//200
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);//500
            }
        }
        //DELETE api/<controller>/x
        [HttpDelete]
        [TokenAuthorize()]
        public IHttpActionResult Delete(int id)
        {
            var jelentkezes = ctx.Jelentkezesek.FirstOrDefault(j => j.User_Id == id );
            if (jelentkezes == null)
            {
                return NotFound();
            }

            try
            {
                ctx.Jelentkezesek.Remove(jelentkezes);
                ctx.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        

    }
}
