using backend_01.Model;
using backend_01.Models;
using backend_01.Security; // Az új Validator osztály használata
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace backend_01.Controllers
{
    public class FelhasznaloController : ApiController
    {
        WebContext ctx;

        public FelhasznaloController()
        {
            ctx = new WebContext();
        }

        // GET api/<controller>
        //[TokenAuthorize()]
        public IHttpActionResult Get()
        {
            var felhasznalok = ctx.Felhasznalo
                .Include("Feladatok")
                .Include("Ertekelesek")
                .Include("Jelentkezesek")
                .Include("Mentesek")
                .Select(f => new
                {
                    f.User_Id,
                    f.Email,
                    f.SzulDat,
                    f.VezNev,
                    f.KerNev,
                    f.ProfilKep,
                    f.Bio,
                    f.RegDatum,
                    f.Felhtipus,
                    f.Telefonszam,
                    Feladatok = f.Feladatok.Select(task => new
                    {
                        task.Task_Id,
                        task.Cim,
                        task.Statusz,
                        task.Helyszin,
                        task.PosztDatum,
                        task.Hatarido
                    }),
                    Ertekelesek = f.Ertekelesek.Select(e => new
                    {
                        e.Ert_Id,
                        e.ErDatum,
                        e.Comment,
                        e.ertekeles
                    }),
                    Jelentkezesek = f.Jelentkezesek.Select(j => new
                    {
                        j.Statusz,
                        j.JelDatum,
                        j.Latta_e,
                        j.Task_Id
                    }),
                    Mentesek = f.Mentesek.Select(m => new
                    {
                        m.Task_Id
                    })
                })
                .ToList();

            return Ok(felhasznalok); // 200 OK
        }

        // GET api/<controller>/x
        [TokenAuthorize()]
        public IHttpActionResult Get(int id)
        {
            var felhasznalo = ctx.Felhasznalo
                .Where(f => f.User_Id == id)
                .Select(f => new
                {
                    f.SzulDat,
                    f.VezNev,
                    f.KerNev,
                    f.ProfilKep,
                    f.Bio,
                    f.Telefonszam
                });
            if (felhasznalo == null)
            {
                return NotFound(); // 404
            }

            return Ok(felhasznalo); // 200 OK
        }

        // GET api/<controller>/x
        [TokenAuthorize()]
        [HttpGet]
        [Route("api/felhasznalo/getuser/{id}/{activ}")]
        public IHttpActionResult GetDatas(int id, int activ)
        {
            var felhasznalo = ctx.Felhasznalo
                .Where(f => f.User_Id == id)
                .Select(f => new
                {
                    f.SzulDat,
                    f.RegDatum,
                    TeljesNev = f.VezNev + " " + f.KerNev,
                    f.ProfilKep,
                    f.Bio,
                    Telefonszam = ctx.Jelentkezesek.Any(j => j.User_Id == activ && j.Feladat.User_Id == id && j.Statusz == "elfogadva")
                        ? f.Telefonszam
                        : null,
                    Ertekelesek = ctx.Ertekeles
                        .Where(r => r.Ertekelt_Id == id)
                        .Select(l => new
                        {
                            l.ErDatum,
                            l.Comment,
                            l.ertekeles,
                            TeljesNev = l.Ertekelo.VezNev + " " + l.Ertekelo.KerNev
                        }).ToList(),
                    Feladatok = ctx.Feladat
                        .Where(t => t.User_Id == id)
                        .Select(t => new
                        {
                            t.Task_Id,
                            t.Statusz,
                            t.Helyszin,
                            t.Cim,
                            t.PosztDatum,
                            t.Leiras,
                            t.Idotartam,
                            t.Fizetes
                        }).ToList()
                }).FirstOrDefault();

            if (felhasznalo == null)
            {
                return NotFound(); // 404
            }

            return Ok(felhasznalo); // 200 OK
        }



        // POST api/<controller> 
        [HttpPost]
        //[TokenAuthorize()]
        [Route("api/felhasznalo/login/{email}/{pw}")]
        public IHttpActionResult Login(string email, string pw)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                return BadRequest("Hibás email formátum.");
            }

            var user = ctx.Felhasznalo.FirstOrDefault(f => f.Email == email);

            if (user == null)
            {
                return Unauthorized(); // 401 Unauthorized
            }

            
            if (!Validator.VerifyPassword(pw, user.Jelszo)) 
            {
                return Unauthorized(); // 401 Unauthorized
            }

            return Ok(user); // 200 OK
        }

        // POST api/<controller>
        //[TokenAuthorize("Admin")]
        [HttpPost]
        [Route("api/register")]
        public IHttpActionResult Register([FromBody] Felhasznalo felhasznalo)
        {
            // Validáció a kötelező mezőkre
            if (string.IsNullOrEmpty(felhasznalo.Email) || string.IsNullOrEmpty(felhasznalo.Jelszo))
            {
                return BadRequest("Minden mezőt ki kell tölteni!");
            }

            // Ellenőrizzük az email formátumot
            try
            {
                var emailCheck = new System.Net.Mail.MailAddress(felhasznalo.Email);
            }
            catch
            {
                return BadRequest("Érvénytelen email cím!");
            }

            // Ellenőrizzük, hogy a felhasználónév vagy email már létezik-e
            var existingUser = ctx.Felhasznalo.FirstOrDefault(u => u.Email == felhasznalo.Email);
            if (existingUser != null)
            {
                return Conflict(); // 409
            }

            var ujfelhasznalo = new Felhasznalo
            {
                Email = felhasznalo.Email,
                Jelszo = Validator.HashPassword(felhasznalo.Jelszo), 
                RegDatum = felhasznalo.RegDatum,
                VezNev = null,
                KerNev = null,
                ProfilKep = null,
                Bio = null
            };

            try
            {
                // Adatok mentése az adatbázisba
                ctx.Felhasznalo.Add(ujfelhasznalo);
                ctx.SaveChanges();
                return Ok("A regisztráció sikeres!");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [TokenAuthorize()]
        [HttpPatch]
        [Route("api/felhasznalo/updatedata")]
        public IHttpActionResult ExpandRegistration([FromBody] Felhasznalo felhasznalo)
        {
            if (string.IsNullOrEmpty(felhasznalo.Email) || string.IsNullOrEmpty(felhasznalo.VezNev) || string.IsNullOrEmpty(felhasznalo.KerNev))
            {
                return BadRequest("Az email, vezetéknév és keresztnév mezők kitöltése kötelező!");
            }

            try
            {
                var user = ctx.Felhasznalo.FirstOrDefault(u => u.Email == felhasznalo.Email);
                if (user == null)
                {
                    return NotFound(); // 404
                }

                user.VezNev = felhasznalo.VezNev;
                user.KerNev = felhasznalo.KerNev;
                user.ProfilKep = felhasznalo.ProfilKep ?? user.ProfilKep; // Ha nincs új adat, megtartjuk a régi értéket
                user.Bio = felhasznalo.Bio ?? user.Bio;
                user.Telefonszam = felhasznalo.Telefonszam ?? user.Telefonszam;
                user.SzulDat = felhasznalo.SzulDat ?? user.SzulDat;
                if (!string.IsNullOrWhiteSpace(felhasznalo.Jelszo))
                {
                    user.Jelszo = Validator.HashPassword(felhasznalo.Jelszo);
                }
                ctx.SaveChanges();

                return Ok("A profil frissítése sikeres!");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PATCH api/<controller>/x
        [TokenAuthorize()]
        [HttpPatch]
        [Route("api/felhasznalo/update/{id}")]
        public IHttpActionResult Update(int id, [FromBody] Felhasznalo updatedData)
        {
            var user = ctx.Felhasznalo.FirstOrDefault(m => m.User_Id == id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(updatedData.Jelszo))
                {
                    user.Jelszo = Validator.HashPassword(updatedData.Jelszo); // Jelszó hash-elése statikus metódussal
                }
                
                if (!string.IsNullOrWhiteSpace(updatedData.VezNev))
                {
                    user.VezNev = updatedData.VezNev;
                }
                if (!string.IsNullOrWhiteSpace(updatedData.KerNev))
                {
                    user.KerNev = updatedData.KerNev;
                }
                if (!string.IsNullOrWhiteSpace(updatedData.ProfilKep))
                {
                    user.ProfilKep = updatedData.ProfilKep;
                }
                if (!string.IsNullOrWhiteSpace(updatedData.Bio))
                {
                    user.Bio = updatedData.Bio;
                }

                ctx.SaveChanges();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/<controller>/x
        [TokenAuthorize("Admin")]
        public IHttpActionResult Delete(int id)
        {
            var delUser = ctx.Felhasznalo.FirstOrDefault(m => m.User_Id == id);
            if (delUser == null)
                return NotFound();

            try
            {
                var ertekelesek = ctx.Ertekeles.Where(e => e.Ertekelo_Id == delUser.User_Id && e.Ertekelt_Id==delUser.User_Id).ToList();
                foreach (var ertekeles in ertekelesek)
                {
                    ctx.Ertekeles.Remove(ertekeles);
                }

                var mentesek = ctx.Mentes.Where(m => m.User_Id == delUser.User_Id).ToList();
                foreach (var mentes in mentesek)
                {
                    ctx.Mentes.Remove(mentes);
                }

                var jelentkezesek = ctx.Jelentkezesek.Where(j => j.User_Id == delUser.User_Id).ToList();
                foreach (var jelentkezes in jelentkezesek)
                {
                    ctx.Jelentkezesek.Remove(jelentkezes);
                }

                ctx.Felhasznalo.Remove(delUser);
                ctx.SaveChanges();
                return Ok("Felhasználó sikeresen törölve!");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
