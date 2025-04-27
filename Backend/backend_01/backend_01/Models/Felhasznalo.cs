using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace backend_01.Models
{
    public enum FelhasznaloTipus
    {
        Admin,
        User
    }
    public class Felhasznalo
    {
        [Key]
        public int User_Id { get; set; }
        public string Jelszo { get; set; }
        public string Email { get; set; }
        public string SzulDat { get; set; }
        public string VezNev { get; set; }
        public string KerNev { get; set; } 
        public string ProfilKep { get; set; }
        public string Bio { get; set; }
        public string RegDatum { get; set; }
        public string Felhtipus { get; set; } = FelhasznaloTipus.User.ToString();
        public string Telefonszam { get; set; }
        public ICollection<Feladat> Feladatok { get; set; } 
        public ICollection<Ertekeles> Ertekelesek { get; set; }
        public ICollection<Jelentkezesek> Jelentkezesek { get; set; }
        public ICollection<Mentes> Mentesek { get; set; }



    }
    
}