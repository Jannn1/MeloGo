using backend_01.Model;
using backend_01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_01.Security
{
    public class Validator
    {
        private WebContext _context;

        public Validator(WebContext context)
        {
            _context = context;
        }

        public static string HashPassword(string password)
        {
            // a jelszó hash kódját állítja elő
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // ellenőrzi a kapott és a hash kódjával tárolt jelszavakat
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public bool IsValidUser(Felhasznalo user)
        {
            // ellenőrzi a megadott felhasználót (pl. HTTP kérésből kapott adatokból)
            var storedUser = _context.Felhasznalo.FirstOrDefault(p => p.Email == user.Email);
            return storedUser != null && VerifyPassword(user.Jelszo, storedUser.Jelszo);
        }
    }
}