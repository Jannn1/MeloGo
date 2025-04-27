using backend_01.Model;
using backend_01.Models;
using backend_01.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;


namespace TokenApi.Controller
{
    public class LoginController : ApiController
    {
        WebContext c;

        public LoginController()
        {
            c = new WebContext();
        }
        public LoginController(WebContext context)
        {
            c = context;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/token")]
        public IHttpActionResult Login([FromBody] Felhasznalo user)
        {
            if (new Validator(c).IsValidUser(user))
            {
                return Ok(TokenManager.GenerateToken(user));
            }

            return Unauthorized(
                new AuthenticationHeaderValue[]
                {
                    new AuthenticationHeaderValue("Basic", "realm=\"TokenApi\"")
                });
        }


        [TokenAuthorize("User")]
        [HttpDelete]
        public IHttpActionResult Logoff()
        {
            return null;
        }
    }
}