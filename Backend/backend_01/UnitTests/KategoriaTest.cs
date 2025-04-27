using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend_01.Controllers;
using backend_01.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using UnitTests;

namespace UnitTests
{
    [TestClass]
    public class KategoriaTest
    {
        private KategoriaController controller;
        private WebContext context;

        [TestInitialize]
        public void Setup()
        {
            context = new WebContext();  // Az adatbázis kapcsolata
            controller = new KategoriaController();  // A controller példányosítása

            // Tesztadatok hozzáadása, ha még nem léteznek
            if (!context.Kategoria.Any())
            {
                context.Kategoria.Add(new Kategoria { Kat_Id = 1, Katnev = "Kertészkedés" });
                context.Kategoria.Add(new Kategoria { Kat_Id = 2, Katnev = "Takarítás" });
                context.Kategoria.Add(new Kategoria { Kat_Id = 3, Katnev = "Költöztetés" });
                context.Kategoria.Add(new Kategoria { Kat_Id = 4, Katnev = "Futár" });
                context.SaveChanges(); // Az adatok mentése
            }
        }

        [TestMethod]
        public void Get_OsszesKategoria_VisszaadLista()
        {
            // GET metódus hívása a KategoriaController-hez
            var result = controller.Get();

            // Ellenőrizzük, hogy a válasz nem null és sikeres volt
            Assert.IsNotNull(result, "A válasz null.");

            // Ellenőrizzük, hogy a válasz valóban tartalmazza a kategóriákat
            var okResult = result as OkNegotiatedContentResult<IEnumerable<dynamic>>;
            Assert.IsNotNull(okResult, "A válasz nem OkNegotiatedContentResult.");
            var content = okResult.Content as IEnumerable<Dictionary<string, object>>;  // Típusos adatfeldolgozás
            Assert.IsNotNull(content, "A válasz tartalma üres.");
            Assert.IsTrue(content.Any(), "A válaszban nincs kategória.");

            // Ellenőrizzük, hogy a kategóriák Task_Id-ja és neve megtalálható
            foreach (var item in content)
            {
                Assert.IsTrue(item.ContainsKey("Kat_Id"), "A Kat_Id nem található.");
                Assert.IsTrue(item.ContainsKey("Katnev"), "A Katnev nem található.");
            }
        }
    }

}
