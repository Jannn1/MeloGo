using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace backend_01.Models
{
    public class Kategoria
    {
        [Key]
        public int Kat_Id { get; set; }
        public string Katnev { get; set; }
        public ICollection<FeladatKategoria> FeladatKategoria { get; set; }
    }
}