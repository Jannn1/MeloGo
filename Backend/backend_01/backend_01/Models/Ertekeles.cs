using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;


namespace backend_01.Models
{

    public class Ertekeles
    {
        [Key]
        public int Ert_Id { get; set; }
        public string ErDatum { get; set; }
        public string Comment { get; set; }
        public int ertekeles { get; set; }
        [ForeignKey("Ertekelo")]
        public int Ertekelo_Id { get; set; }
        public virtual Felhasznalo Ertekelo { get; set; }

        [ForeignKey("Ertekelt")]
        public int Ertekelt_Id { get; set; }
        public virtual Felhasznalo Ertekelt { get; set; }


    }
}