using backend_01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Data.Entity;
using backend_01.Model;
using backend_01.Security;

namespace backend_01.Controllers
{
    public class ErtekelesController : ApiController
    {
        WebContext ctx;
        public ErtekelesController()
        {
            ctx = new WebContext();
        }

        // GET api/<controller>
        public IHttpActionResult Get()
        {
            var ertekelesek = ctx.Ertekeles
                .Include(e => e.Ertekelo)
                .Include(e => e.Ertekelt)
                .Select(e => new
                {
                    e.Ert_Id,
                    e.ErDatum,
                    e.Comment,
                    e.ertekeles,
                    Ertekelo = new
                    {
                        e.Ertekelo.User_Id,
                        e.Ertekelo.Email,
                        e.Ertekelo.VezNev,
                        e.Ertekelo.KerNev,
                        e.Ertekelo.ProfilKep
                    },
                    Ertekelt = new
                    {
                        e.Ertekelt.User_Id,
                        e.Ertekelt.Email,
                        e.Ertekelt.VezNev,
                        e.Ertekelt.KerNev,
                        e.Ertekelt.ProfilKep
                    }
                }).ToList();

            return Ok(ertekelesek);
        }

        // GET api/<controller>/x
        [TokenAuthorize()]
        public IHttpActionResult Get(int id)
        {
            var ertekelesek = ctx.Ertekeles
                .Include(e => e.Ertekelo)
                .Include(e => e.Ertekelt)
                .Where(e => e.Ertekelt.User_Id == id) // értékelt alapján szűrés
                .Select(e => new
                {
                    e.Ert_Id,
                    e.ErDatum,
                    e.Comment,
                    e.ertekeles,
                    Ertekelo = new
                    {
                        e.Ertekelo.VezNev,
                        e.Ertekelo.KerNev,
                        e.Ertekelo.ProfilKep
                    }
                }).ToList();

            if (!ertekelesek.Any())
            {
                return NotFound();
            }

            return Ok(ertekelesek);
        }

        // POST api/<controller>
        [TokenAuthorize()]
        [HttpPost]
        [Route("api/felhasznaloertekeles")]
        public IHttpActionResult Post([FromBody] Ertekeles value)
        {
            if (value == null)
                return BadRequest("Az értékelés nem lehet üres.");

            if (value.ertekeles < 1 || value.ertekeles > 5)
                return BadRequest("Az értékelés csak 1 és 5 közötti érték lehet.");

            if (value.Ertekelo_Id == value.Ertekelt_Id)
                return BadRequest("Saját magát nem lehet értékelni.");

            if (string.IsNullOrWhiteSpace(value.Comment))
                value.Comment = null;

            try
            {
                //value.ErDatum = DateTime.Now.ToString();
                ctx.Ertekeles.Add(value);
                ctx.SaveChanges();
                return Ok(value);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/<controller>/x
        [TokenAuthorize("Admin")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var ertekeles = ctx.Ertekeles.FirstOrDefault(e => e.Ert_Id == id);
            if (ertekeles == null)
                return NotFound();

            try
            {
                ctx.Ertekeles.Remove(ertekeles);
                ctx.SaveChanges();
                return Ok(ertekeles);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
