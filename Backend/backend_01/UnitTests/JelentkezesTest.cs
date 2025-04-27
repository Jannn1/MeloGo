using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend_01.Controllers;
using backend_01.Models;
using System.Linq;
using System.Web.Http.Results;
using UnitTests;

namespace UnitTests
{
    [TestClass]
    public class JelentkezesTest
    {
        private JelentkezesekController controller;
        private WebContext context;

        [TestInitialize]
        public void Setup()
        {
            context = new WebContext(); // Az adatbázis kapcsolata
            controller = new JelentkezesekController();  // A controller példányosítása

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
        public void Post_Jelentkezes_Sikeres()
        {
            // Tesztadatok létrehozása manuálisan
            var testUser = new Felhasznalo
            {
                User_Id = 11,  // Manuálisan beállított User_Id
                Email = "felhasznalo8@test.com",
                Jelszo = "Teszt1234",
                VezNev = "Hajni",
                KerNev = "Lakatos",
                SzulDat = "1993-06-06",
                ProfilKep = null,
                Bio = "Rendszeres önkéntes",
                RegDatum = "2025-03-09 10:21:31",
                Felhtipus = "user",
                Telefonszam = "+36 20/890-1234"
            };

            var testTask = new Feladat
            {
                Task_Id = 41,  // Manuálisan beállított Task_Id
                Cim = "Új Feladat",
                Statusz = "Folyamatban",
                Helyszin = "Büfé",
                PosztDatum = "2023-04-26 14:30:00",
                Hatarido = "2023-04-27",
                Leiras = "Új feladat hozzáadása",
                User_Id = testUser.User_Id,  // A feladathoz a testUser User_Id-ja rendelve
                Idotartam = "2 óra",
                Fizetes = 8000
            };

            // A tesztadatok mentése a WebContext-be
            context.Felhasznalo.Add(testUser);
            context.Feladat.Add(testTask);
            context.SaveChanges(); // Az adatok mentése az adatbázisba

            // Új jelentkezés létrehozása
            var newJelentkezes = new Jelentkezesek
            {
                Statusz = "függőben",
                User_Id = testUser.User_Id,
                Task_Id = testTask.Task_Id
            };

            // POST metódus hívása
            var result = controller.Post(testUser.User_Id, testTask.Task_Id, newJelentkezes.Statusz);  // Argumentumok hozzáadása

            // Ellenőrizzük, hogy a válasz nem null és sikeres volt
            Assert.IsNotNull(result, "A válasz null.");
            var okResult = result as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(okResult, "A válasz nem OkNegotiatedContentResult.");
            Assert.AreEqual("A jelentkezés sikeres.", okResult.Content);
        }

    }

}
