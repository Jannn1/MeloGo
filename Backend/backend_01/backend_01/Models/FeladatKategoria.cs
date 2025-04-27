using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace backend_01.Models
{
    public class FeladatKategoria
    {
        [Key, Column(Order = 0)]
        public int Task_Id { get; set; }

        [ForeignKey("Task_Id")]
        public Feladat Feladat { get; set; }

        [Key, Column(Order = 1)]
        public int Kat_Id { get; set; }

        [ForeignKey("Kat_Id")]
        public Kategoria Kategoria { get; set; }
    }
}