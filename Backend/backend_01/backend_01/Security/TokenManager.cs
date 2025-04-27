using backend_01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace backend_01.Security
{
    public class TokenManager
    {
        public static string GenerateToken(Felhasznalo user)
        {
            var bytes = Encoding.UTF8.GetBytes($"{user.Email}|{user.Felhtipus}");
            return Convert.ToBase64String(bytes);
        }

        public static Felhasznalo DecodeToken(string token)
        {
            try
            {
                var bytes = Convert.FromBase64String(token);
                var tokens = Encoding.UTF8.GetString(bytes).Split('|');
                return new Felhasznalo
                {
                    Email = tokens[0],
                    Felhtipus = ((FelhasznaloTipus)Enum.Parse(typeof(FelhasznaloTipus), tokens[1])).ToString()

                };

            }

            catch (Exception)
            {
                return null;
            }
        }

    }
}