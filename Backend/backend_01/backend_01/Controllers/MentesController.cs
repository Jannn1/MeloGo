using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using backend_01.Models;
using backend_01.Model;
using backend_01.Security;

namespace backend_01.Controllers
{
    public class MentesController : ApiController
    {
        WebContext ctx;
        public MentesController()
        {
            ctx = new WebContext();
        }

        //GET api/<controller> 
        [TokenAuthorize()]
        [HttpGet]
        [Route("api/mentes")]
        //[TokenAuthorize()]
        public IHttpActionResult Get()
        {
            var mentettFeladatok = ctx.Mentes
                .Select(m => new
                {
                    m.User_Id,
                    m.Task_Id,
                    Felhasznalo = new
                    {
                        m.Felhasznalo.User_Id,
                        m.Felhasznalo.Email,
                        m.Felhasznalo.VezNev,
                        m.Felhasznalo.KerNev,
                        m.Felhasznalo.ProfilKep,
                        m.Felhasznalo.Telefonszam
                    },
                    Feladat = new
                    {
                        m.Feladat.Task_Id,
                        m.Feladat.Cim,
                        m.Feladat.Statusz,
                        m.Feladat.Helyszin,
                        m.Feladat.PosztDatum,
                        m.Feladat.Hatarido
                    }
                })
                .ToList(); 

            return Ok(mentettFeladatok); // 200 OK
        }


        [TokenAuthorize()]
        //GET api/<controller>/x
        public IHttpActionResult Get(int id, int id2)
        {
            var mentett = ctx.Mentes
                .Include("Felhasznalo")
                .Include("Feladat")
                .Where(m => m.User_Id == id && m.Task_Id == id2)
                .Select(m => new
                {
                    m.User_Id,
                    m.Task_Id,
                    Felhasznalo = new
                    {
                        m.Felhasznalo.User_Id,
                        m.Felhasznalo.Email,
                        m.Felhasznalo.VezNev,
                        m.Felhasznalo.KerNev,
                        m.Felhasznalo.ProfilKep,
                        m.Felhasznalo.Telefonszam
                    },
                    Feladat = new
                    {
                        m.Feladat.Task_Id,
                        m.Feladat.Cim,
                        m.Feladat.Statusz,
                        m.Feladat.Helyszin,
                        m.Feladat.PosztDatum,
                        m.Feladat.Hatarido
                    }
                })
                .FirstOrDefault();

            if (mentett == null)
            {
                return NotFound(); // 404
            }

            return Ok(mentett); // 200 OK
        }


        //POST api/<controller>
        [TokenAuthorize()]
        [HttpPost]
        [Route("api/mentes")]
        public IHttpActionResult AddSave([FromBody] Mentes value)
        {
            try
            {
                var res = new Mentes()
                {
                    Task_Id = value.Task_Id,
                    User_Id = value.User_Id
                };


                ctx.Mentes.Add(res);
                ctx.SaveChanges();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //DELETE api/<controller>/x
        [TokenAuthorize()]
        [HttpDelete]
        [Route("api/mentestorles/{userId}/{taskId}")]
        public IHttpActionResult Delete(int userId, int taskId)
        {
            var mentes = ctx.Mentes.FirstOrDefault(m => m.User_Id == userId && m.Task_Id == taskId);

            if (mentes == null)
            {
                return NotFound(); // 404
            }

            try
            {
                ctx.Mentes.Remove(mentes);
                ctx.SaveChanges(); 

                return Ok(mentes); // 200
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // 500
            }
        }



    }
}
