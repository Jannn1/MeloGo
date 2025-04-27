using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace backend_01.Models
{
    public class Mentes
    {
        public int User_Id { get; set; }
        [ForeignKey("User_Id")]
        public Felhasznalo Felhasznalo { get; set; }
        public int Task_Id { get; set; }
        [ForeignKey("Task_Id")]
        public Feladat Feladat { get; set; }
    }
}