using backend_01.Models;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    public class WebContext : IWebContext
    {
        // Minden entitás egy List-ben tárolva
        public ICollection<Felhasznalo> Felhasznalo { get; set; }
        public ICollection<Feladat> Feladat { get; set; }
        public ICollection<Ertekeles> Ertekeles { get; set; }
        public ICollection<Kategoria> Kategoria { get; set; }
        public ICollection<FeladatKategoria> FeladatKategoria { get; set; }
        public ICollection<Jelentkezesek> Jelentkezesek { get; set; }  // Hozzáadjuk a Jelentkezesek táblát
        public ICollection<Mentes> Mentes { get; set; }

        // Konstruktorban inicializáljuk a listákat
        public WebContext()
        {
            Felhasznalo = new List<Felhasznalo>
            {
                new Felhasznalo { User_Id = 1, Email = "tesztuser@example.com", Jelszo = "Teszt1234", SzulDat="1990-05-10", VezNev="Teszt", KerNev="TTeszt", ProfilKep=null, Bio="asdasdtestasd", RegDatum="2000-01-01", Felhtipus="user", Telefonszam="+36 20/123-4567" }
            };

            Feladat = new List<Feladat>
            {
                new Feladat { Task_Id = 1, Cim = "Teszt Feladat", Statusz = "nyitott", Helyszin = "Büfé", PosztDatum = "2023-04-26", Hatarido = "2023-04-27", User_Id = 1, Idotartam = "2 óra", Fizetes = 8000 }
            };

            Ertekeles = new List<Ertekeles>
            {
                new Ertekeles { Ertekelo_Id = 1, Ertekelt_Id = 1, ertekeles = 5, Comment = "Nagyszerű!" }
            };

            Kategoria = new List<Kategoria>
            {
                new Kategoria { Kat_Id = 1, Katnev = "Kertészkedés" },
                new Kategoria { Kat_Id = 2, Katnev = "Takarítás" }
            };

            FeladatKategoria = new List<FeladatKategoria>
            {
                new FeladatKategoria { Task_Id = 1, Kat_Id = 1 }
            };

            Jelentkezesek = new List<Jelentkezesek>
            {
                new Jelentkezesek { Statusz = "függőben", User_Id = 1, Task_Id = 1 }
            };

            Mentes = new List<Mentes>
            {
                new Mentes { User_Id = 1, Task_Id = 1 }
            };
        }

        // Módszer, ami hasonlít a SaveChanges-ra
        public void SaveChanges() { /* Nem csinál semmit, de a teszteléshez elég */ }
    }
}
