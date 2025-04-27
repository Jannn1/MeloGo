using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend_01.Controllers;
using backend_01.Models;
using backend_01.Model;
using System.Web.Http.Results;
using System.Collections.Generic;
using System.Linq;
using backend_01.Security;

namespace UnitTests
{
    [TestClass]
    public class FelhasznaloTest
    {
        private FelhasznaloController controller;
        private WebContext context;

        [TestInitialize]
        public void Setup()
        {
            context = new WebContext();
            controller = new FelhasznaloController();
        }

        [TestMethod]
        public void Get_OsszesFelhasznalo_VisszaadLista()
        {
            try
            {
                // Ne használjunk Include-ot, csak az alap felhasználók lekérdezése
                var result = controller.Get() as OkNegotiatedContentResult<List<object>>;

                if (result == null)
                {
                    Assert.Fail("Nem OkNegotiatedContentResult jött vissza.");
                }

                Console.WriteLine("Felhasználók száma: " + result.Content.Count);
                Assert.IsTrue(result.Content.Count >= 0);
            }
            catch (Exception ex)
            {
                var full = ex;
                int depth = 0;
                while (full.InnerException != null && depth < 10)
                {
                    full = full.InnerException;
                    depth++;
                }

                Assert.Fail("Kivétel történt: " + full.Message);
            }
        }




        [TestMethod]
        public void Post_UjFelhasznalo_SikeresRegisztracio()
        {
            var ujfelhasznalo = new Felhasznalo
            {
                Email = "tesztuser" + Guid.NewGuid().ToString("N").Substring(0, 8) + "@example.com",
                Jelszo = Validator.HashPassword("Teszt1234"),
                RegDatum = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                VezNev = "Teszt",
                KerNev = "User",
                ProfilKep = "",
                Bio = "",
                SzulDat = "2000-01-01",
                Telefonszam = "123456789"
            };

            try
            {
                var result = controller.Register(ujfelhasznalo);
                Console.WriteLine("Típus: " + result?.GetType().Name);

                var ok = result as OkNegotiatedContentResult<string>;
                Assert.IsNotNull(ok, "Nem OkNegotiatedContentResult jött vissza.");
                Assert.AreEqual("A regisztráció sikeres!", ok.Content);
            }
            catch (Exception ex)
            {
                Assert.Fail("Kivétel történt: " + ex.Message + "\n\nInner: " + ex.InnerException?.Message);
            }
        }

        [TestMethod]
        public void Put_FelhasznaloFrissites_SikeresModositas()
        {
            int felhasznaloId = 1;
            var update = new Felhasznalo
            {
                VezNev = "Teszt",
                KerNev = "Felhasznalo",
                Bio = "Új bio",
                Jelszo = "UjJelszo1234"
            };

            var result = controller.Update(felhasznaloId, update) as OkNegotiatedContentResult<Felhasznalo>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Teszt", result.Content.VezNev);
        }
    }
}