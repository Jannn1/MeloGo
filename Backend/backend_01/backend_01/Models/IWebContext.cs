using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_01.Models
{
    public interface IWebContext
    {
        ICollection<Felhasznalo> Felhasznalo { get; set; }
    }
}