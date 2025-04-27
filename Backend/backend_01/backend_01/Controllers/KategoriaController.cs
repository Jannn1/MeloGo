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
    public class KategoriaController : ApiController
    {
        WebContext ctx;
        public KategoriaController()
        {
            ctx = new WebContext();
        }
        //GET api/<controller>
        [TokenAuthorize()]
        public IHttpActionResult Get()
        {
            var kategoriak = ctx.Kategoria
                .Include("FeladatKategoria")
                .Select(k => new
                {
                    k.Kat_Id,
                    k.Katnev,
                    Feladatok = k.FeladatKategoria.Select(fk => new
                    {
                        fk.Feladat.Task_Id,
                        fk.Feladat.Cim,
                        fk.Feladat.Statusz,
                        fk.Feladat.Helyszin,
                        fk.Feladat.PosztDatum,
                        fk.Feladat.Hatarido
                    })
                })
                .ToList();

            return Ok(kategoriak); // 200 OK
        }

        [TokenAuthorize()]
        //GET api/<controller>/x
        public IHttpActionResult Get(int id)
        {
            var kategoria = ctx.Kategoria
                .Include("FeladatKategoria")
                .Where(k => k.Kat_Id == id)
                .Select(k => new
                {
                    k.Kat_Id,
                    k.Katnev,
                    Feladatok = k.FeladatKategoria.Select(fk => new
                    {
                        fk.Feladat.Task_Id,
                        fk.Feladat.Cim,
                        fk.Feladat.Statusz,
                        fk.Feladat.Helyszin,
                        fk.Feladat.PosztDatum,
                        fk.Feladat.Hatarido
                    })
                })
                .FirstOrDefault();

            if (kategoria == null)
            {
                return NotFound(); // 404
            }

            return Ok(kategoria); // 200 OK
        }


        //POST api/<controller>/
        [TokenAuthorize("Admin")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] Kategoria kategoria)
        {
            
            if (kategoria == null || !ModelState.IsValid)
            {
                return BadRequest("Hibás adat!"); // 400
            }

            var existingCategory = ctx.Kategoria.FirstOrDefault(k => k.Katnev == kategoria.Katnev);
            if (existingCategory != null)
            {
                return Conflict(); // 409 
            }
            try
            {
                ctx.Kategoria.Add(kategoria);
                ctx.SaveChanges(); 
                return Ok(kategoria); // 200
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // 500 
            }
        }


        //PUT api/<controller>/
        [TokenAuthorize("Admin")]
        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] Kategoria kategoria)
        {
            if (kategoria == null || !ModelState.IsValid)
            {
                return BadRequest("Hibás adat!"); // 400 
            }

            var existingCategory = ctx.Kategoria.FirstOrDefault(k => k.Kat_Id == id);
            if (existingCategory == null)
            {
                return NotFound(); // 404 
            }

            try
            {
                existingCategory.Katnev = kategoria.Katnev;
                ctx.SaveChanges(); 
                return Ok(existingCategory); // 200
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // 500
            }
        }

        //PATCH api/<controller>/
        [TokenAuthorize("Admin")]
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] Kategoria update)
        {
            if (update == null || !ModelState.IsValid)
            {
                return BadRequest("Hiba."); //400
            }

            var exKat = ctx.Kategoria.FirstOrDefault(m => m.Kat_Id == id);
            if (exKat == null)
            {
                return NotFound(); //404 
            }

            if (!string.IsNullOrEmpty(update.Katnev))
            {
                exKat.Katnev = update.Katnev;
            }

            ctx.SaveChanges();

            return Ok(exKat);//200
        }


        //DELETE api/<controller>/x
        [HttpDelete]
        [TokenAuthorize("Admin")]
        public IHttpActionResult Delete(int id)
        {
            var kategoria = ctx.Kategoria.FirstOrDefault(j => j.Kat_Id == id);
            if (kategoria == null)
            {
                return NotFound();
            }

            try
            {
                ctx.Kategoria.Remove(kategoria);
                ctx.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [TokenAuthorize()]
        [HttpPost]
        [Route("api/kategoria/ujkategoriahozzaadas/{mid}/{katnev}")]
        public IHttpActionResult AddCategoriaToTask([FromBody] int mid, string katnev)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(katnev))
                {
                    return BadRequest();
                }

                var kategoria = ctx.Kategoria.FirstOrDefault(k => k.Katnev == katnev);

                if (kategoria == null)
                {
                    kategoria = new Kategoria { Katnev = katnev };
                    ctx.Kategoria.Add(kategoria);
                    ctx.SaveChanges();
                }

                bool kapcsolatLetezik = ctx.FeladatKategoria.Any(fk => fk.Task_Id == mid && fk.Kat_Id == kategoria.Kat_Id);

                if (kapcsolatLetezik == true)
                {
                    return BadRequest();
                }

                var ujKapcsolat = new FeladatKategoria
                {
                    Task_Id = mid,
                    Kat_Id = kategoria.Kat_Id
                };

                ctx.FeladatKategoria.Add(ujKapcsolat);
                ctx.SaveChanges();

                return Ok(new { kat_id = kategoria.Kat_Id });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [TokenAuthorize()]
        [HttpDelete]
        [Route("api/kategoria/kapcsolattorles/{fid}/{kid}")]
        public IHttpActionResult DeleteConnection(int fid, int kid)
        {
            try
            {
                var kapcsolat = ctx.FeladatKategoria
                    .FirstOrDefault(fk => fk.Task_Id == fid && fk.Kat_Id == kid);

                if (kapcsolat == null)
                {
                    return NotFound(); // 404
                }

                ctx.FeladatKategoria.Remove(kapcsolat);
                ctx.SaveChanges();

                return Ok(); //200
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // 500
            }
        }


    }
}
