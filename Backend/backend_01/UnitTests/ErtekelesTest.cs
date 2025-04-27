using backend_01.Controllers;
using backend_01.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;

namespace UnitTests
{
    [TestClass]
    public class ErtekelesTest
    {
        private ErtekelesController controller;
        private WebContext context;

        [TestInitialize]
        public void Setup()
        {
            context = new WebContext(); // Az adatbázis kapcsolata
            controller = new ErtekelesController();  // A controller példányosítása

            // Tesztadatok hozzáadása, ha még nem léteznek
            if (!context.Felhasznalo.Any())
            {
                context.Felhasznalo.Add(new Felhasznalo { User_Id = 1, Email = "tesztuser@example.com", Jelszo = "Teszt1234", SzulDat = "1990-05-10", VezNev = "Teszt", KerNev = "TTeszt", ProfilKep = null, Bio = "asdasdtestasd", RegDatum = "2000-01-01", Felhtipus = "user" });
            }

            if (!context.Ertekeles.Any())
            {
                context.Ertekeles.Add(new Ertekeles { Ertekelo_Id = 1, Ertekelt_Id = 2, ertekeles = 5, Comment = "Nagyszerű munka!" });
            }

            context.SaveChanges(); // Az adatok mentése a listában
        }

        [TestMethod]
        public void Get_Ertekelesek_Sikeres()
        {
            // GET metódus hívása
            var result = controller.Get();  // Hívjuk a GET metódust

            // Ellenőrizzük, hogy a válasz nem null és sikeres volt
            Assert.IsNotNull(result, "A válasz null.");
            var okResult = result as OkNegotiatedContentResult<List<Ertekeles>>;
            Assert.IsNotNull(okResult, "A válasz nem OkNegotiatedContentResult.");
            Assert.IsTrue(okResult.Content.Any(), "A válasz nem tartalmaz értékeléseket.");
        }

        [TestMethod]
        public void Post_Ertekeles_Frissites_Sikeres()
        {
            // Tesztadatok hozzáadása, ha még nem léteznek
            var testUser = context.Felhasznalo.FirstOrDefault(f => f.Email == "tesztuser@example.com");
            var testErtekeles = context.Ertekeles.FirstOrDefault(e => e.Ertekelo_Id == testUser.User_Id);

            if (testUser == null || testErtekeles == null)
            {
                Assert.Fail("Tesztadatok nem találhatók.");
            }

            // POST metódus hívása az értékelés frissítésére (ha nincs PUT)
            testErtekeles.ertekeles = 4;  // Módosítjuk az értékelést
            testErtekeles.Comment = "Kicsit javítani kellene.";

            // POST metódus hívása
            var result = controller.Post(testErtekeles);

            // Ellenőrizzük, hogy a válasz nem null és sikeres volt
            Assert.IsNotNull(result, "A válasz null.");
            var okResult = result as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(okResult, "A válasz nem OkNegotiatedContentResult.");
            Assert.AreEqual("Az értékelés sikeresen frissítve.", okResult.Content);

            // Ellenőrizzük, hogy a módosított értékelés státusza frissült-e
            var updatedErtekeles = context.Ertekeles.FirstOrDefault(e => e.Ertekelo_Id == testUser.User_Id);
            Assert.IsNotNull(updatedErtekeles, "Az értékelés nem lett frissítve.");
            Assert.AreEqual(4, updatedErtekeles.ertekeles, "Az értékelés nem frissült.");
        }
    }
}
