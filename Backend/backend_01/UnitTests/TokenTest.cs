using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend_01.Controllers;
using backend_01.Models;
using backend_01.Security;
using System.Web.Http.Results;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class TokenTest
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
        public void Token_Generalas_Sikeres()
        {
            // Létrehozunk egy tesztfelhasználót
            var user = new Felhasznalo
            {
                Email = "tesztuser@example.com",
                Jelszo = "Teszt1234"
            };

            // Ellenőrizzük, hogy létezik a tesztfelhasználó
            var existingUser = context.Felhasznalo.FirstOrDefault(f => f.Email == user.Email);
            if (existingUser == null)
            {
                context.Felhasznalo.Add(user);
                context.SaveChanges();
            }

            // Token generálása
            var token = TokenManager.GenerateToken(user);

            // Ellenőrizzük, hogy a token nem null és van értéke
            Assert.IsNotNull(token, "A generált token nem lehet null.");
            Assert.IsTrue(token.Length > 0, "A token nem tartalmaz adatokat.");
        }



    }
}
