using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace backend_01.Models
{
    public enum FeladatStatus
    {
        nyitott,
        lezart
    }
    public class Feladat
    {
        [Key]
        public int Task_Id { get; set; }
        public string Statusz { get; set; } = FeladatStatus.nyitott.ToString();
        public string Helyszin { get; set; }
        public string Cim { get; set; }
        public string PosztDatum { get; set; }
        public string Hatarido { get; set; }
        public string Leiras { get; set; }
        public int User_Id { get; set; }
        public Felhasznalo Felhasznalo { get; set; }
        public string Idotartam { get; set; }
        public int Fizetes { get; set; }

        public ICollection<FeladatKategoria> FeladatKategoriak { get; set; }
        public ICollection<Jelentkezesek> Jelentkezesek { get; set; }
        public ICollection<Mentes> Mentesek { get; set; }
    }
}