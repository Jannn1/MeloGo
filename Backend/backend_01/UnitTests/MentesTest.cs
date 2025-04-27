using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend_01.Controllers;
using backend_01.Models;
using System.Linq;
using System.Web.Http.Results;

namespace UnitTests
{
    [TestClass]
    public class MentesTest
    {
        private MentesController controller;
        private WebContext context;

        [TestInitialize]
        public void Setup()
        {
            context = new WebContext(); // Az adatbázis kapcsolata
            controller = new MentesController();  // A controller példányosítása

            // Tesztadatok hozzáadása, ha még nem léteznek
            if (!context.Felhasznalo.Any())
            {
                context.Felhasznalo.Add(new Felhasznalo { User_Id = 1, Email = "tesztuser@example.com", Jelszo = "Teszt1234", SzulDat = "1990-05-10", VezNev = "Teszt", KerNev = "TTeszt", ProfilKep = null, Bio = "asdasdtestasd", RegDatum = "2000-01-01", Felhtipus = "user" });
            }

            if (!context.Feladat.Any())
            {
                context.Feladat.Add(new Feladat { Task_Id = 1, Cim = "Teszt Feladat", Statusz = "nyitott", Helyszin = "Büfé", PosztDatum = "2023-04-26", Hatarido = "2023-04-27", User_Id = 1 });
            }

            context.SaveChanges(); // Az adatok mentése a listában
        }

        [TestMethod]
        public void Post_Mentes_Sikeres()
        {
            // Tesztadatok létrehozása manuálisan
            var testUser = context.Felhasznalo.FirstOrDefault(f => f.Email == "felhasznalo8@test.com");
            var testTask = context.Feladat.FirstOrDefault(f => f.Task_Id == 1);

            if (testUser == null || testTask == null)
            {
                Assert.Fail("Tesztadatok nem találhatók.");
            }

            // Új mentés létrehozása
            var newMentes = new Mentes
            {
                User_Id = testUser.User_Id,
                Task_Id = testTask.Task_Id
            };

            // POST metódus hívása a mentes mentésére
            //var result = controller.Post(newMentes);  // Hívjuk a Post metódust

            // Ellenőrizzük, hogy a válasz nem null és sikeres volt
            //Assert.IsNotNull(result, "A válasz null.");
            //var okResult = result as OkNegotiatedContentResult<string>;
            //Assert.IsNotNull(okResult, "A válasz nem OkNegotiatedContentResult.");
            //Assert.AreEqual("A feladat sikeresen mentve.", okResult.Content);

            // Ellenőrizzük, hogy a Mentes tábla tartalmazza-e az új mentést
            var createdMentes = context.Mentes.FirstOrDefault(m => m.User_Id == testUser.User_Id && m.Task_Id == testTask.Task_Id);
            Assert.IsNotNull(createdMentes, "A feladat nem lett mentve.");
        }
    }

}
