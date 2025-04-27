using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend_01.Controllers;
using backend_01.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Results;
using TokenApi.Controller;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class LoginTest
    {
        private LoginController controller;
        private WebContext context;

        [TestInitialize]
        public void Setup()
        {
            context = new WebContext();
            controller = new LoginController();
        }
        [TestMethod]
        public void Login_SikeresBejelentkezes()
        {
            var user = new Felhasznalo
            {
                Email = "tesztuser@example.com",
                Jelszo = "Teszt1234"
            };

            // Ellenőrizzük, hogy a felhasználó létezik a fake context-ben
            var context = new WebContext();
            var existingUser = context.Felhasznalo.FirstOrDefault(f => f.Email == user.Email);

            if (existingUser == null)
            {
                context.Felhasznalo.Add(user);  // Hozzáadjuk, ha nem találjuk
                context.SaveChanges();  // Mivel itt nem valódi adatbázis van, ezt a metódust is mockolhatjuk
            }

            var result = controller.Login(user) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result, "A válasz null.");
            Assert.AreEqual("A regisztráció sikeres!", result.Content);
        }




        [TestMethod]
        public void Login_HibasBejelentkezes_InvalidUser()
        {
            var user = new Felhasznalo
            {
                Email = "hibasuser@example.com",
                Jelszo = "HibasJelszo123"
            };

            var result = controller.Login(user) as UnauthorizedResult;

            Assert.IsNotNull(result, "Hibás válasz.");
        }

        [TestMethod]
        public void Login_Bejelentkezes_HianyzoJelszo()
        {
            var user = new Felhasznalo
            {
                Email = "tesztuser@example.com",
                Jelszo = ""  // üres jelszó
            };

            var result = controller.Login(user) as BadRequestErrorMessageResult;

            Assert.IsNotNull(result, "Hibás válasz.");
        }

        [TestMethod]
        public void Login_Bejelentkezes_HianyzoEmail()
        {
            var user = new Felhasznalo
            {
                Email = "",  // üres email
                Jelszo = "Teszt1234"
            };

            var result = controller.Login(user) as BadRequestErrorMessageResult;

            Assert.IsNotNull(result, "Hibás válasz.");
        }
    }
}