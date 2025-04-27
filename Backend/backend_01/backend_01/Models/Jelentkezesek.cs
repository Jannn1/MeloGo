using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace backend_01.Models
{
    public enum JelentkezesekStatus
    {
        függőben,
        elfogadva,
        elutasítva
    }
    public class Jelentkezesek
    {
        public string Statusz { get; set;} = JelentkezesekStatus.függőben.ToString();
        public int Latta_e { get; set; }
        public string JelDatum { get; set; }
        public int User_Id { get; set; }
        [ForeignKey("User_Id")]
        public Felhasznalo Felhasznalo { get; set; }

        public int Task_Id { get; set; }
        [ForeignKey("Task_Id")]
        public Feladat Feladat { get; set; }
    }
}