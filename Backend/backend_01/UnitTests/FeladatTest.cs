using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend_01.Controllers;
using backend_01.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using System;

namespace UnitTests
{
    [TestClass]
    public class FeladatTest
    {
        private FeladatController controller;
        private WebContext context;

        [TestInitialize]
        public void Setup()
        {
            context = new WebContext(); // Az adatbázis kapcsolata
            controller = new FeladatController();  // A controller példányosítása

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
        public void Post_UjFeladat_SikeresHozzaadas()
        {
            // Tesztfelhasználó létrehozása, ha még nem létezik
            var testUser = context.Felhasznalo.FirstOrDefault(f => f.Email == "felhasznalo8@test.com");

            if (testUser == null)
            {
                testUser = new Felhasznalo
                {
                    User_Id = 11,
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

                context.Felhasznalo.Add(testUser);
                context.SaveChanges();
            }

            // Fixált dátum beállítása
            var fixedDate = DateTime.UtcNow.AddMonths(-6).ToString("yyyy-MM-dd HH:mm:ss");

            // Új feladat létrehozása, nem adjuk meg a Task_Id-t, hogy az automatikusan generálódjon
            var newFeladat = new Feladat
            {
                Cim = "Új Feladat",
                Statusz = "Folyamatban",
                Helyszin = "Büfé",
                PosztDatum = fixedDate,  // Fixált dátum
                Hatarido = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss"),
                Leiras = "Új feladat hozzáadása",
                User_Id = testUser.User_Id, // A feladat a tesztfelhasználóhoz tartozik
                Idotartam = "2 óra",  // Kitöltött mező
                Fizetes = 8000  // Fizetés kitöltése
            };

            // POST metódus hívása
            var result = controller.Post(newFeladat);

            // Ellenőrizzük, hogy a válasz nem null és sikeres volt
            Assert.IsNotNull(result, "A válasz null.");

            // Ellenőrizzük, hogy a válasz valóban tartalmazza a Task_Id-t
            var okResult = result as OkNegotiatedContentResult<object>;
            Assert.IsNotNull(okResult, "A válasz nem OkNegotiatedContentResult.");

            // Használunk konkrét típusokat az adatfeldolgozáshoz
            var content = okResult.Content as IEnumerable<Dictionary<string, object>>;  // Típusos adatfeldolgozás
            Assert.IsNotNull(content, "A válasz tartalma üres.");
            Assert.IsTrue(content.Any(), "A válaszban nincs feladat.");

            // Ellenőrizzük, hogy a Task_Id megtalálható a válaszban
            var taskIdObj = content.FirstOrDefault()?["Task_Id"];
            Assert.IsNotNull(taskIdObj, "A Task_Id nem található a válaszban.");

            // Konvertáljuk az object típusú taskId-t int típusra
            int taskId = Convert.ToInt32(taskIdObj);

            // Ellenőrizzük, hogy a feladat valóban hozzá lett adva az adatbázishoz
            var createdFeladat = context.Feladat.FirstOrDefault(f => f.Task_Id == taskId);
            Assert.IsNotNull(createdFeladat, "A feladat nem lett hozzáadva.");

            // Ha a feladat hozzá lett adva, naplózzuk, hogy sikerült
            Console.WriteLine($"Feladat hozzáadva: Task_Id = {createdFeladat.Task_Id}, Cim = {createdFeladat.Cim}");
        }






    }
}
