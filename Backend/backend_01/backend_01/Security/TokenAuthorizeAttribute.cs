using backend_01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace backend_01.Security
{
    public class TokenAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public List<FelhasznaloTipus> ut;

        public TokenAuthorizeAttribute(string roles = null)
        {
            ut = new List<FelhasznaloTipus>();

            if (!string.IsNullOrEmpty(roles))
            {
                foreach (var roleName in roles.Split(','))
                {
                    if (Enum.TryParse(roleName.Trim(), out FelhasznaloTipus user))
                    {
                        ut.Add(user);
                    }
                }
            }
            else
            {
                ut.AddRange((FelhasznaloTipus[])Enum.GetValues(typeof(FelhasznaloTipus))); // Összes szerepkör engedélyezése
            }
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader == null || authHeader.Scheme != "Bearer" || string.IsNullOrEmpty(authHeader.Parameter))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Hiányzó vagy érvénytelen token!");
                return;
            }

            var user = TokenManager.DecodeToken(authHeader.Parameter);

            if (user == null || !Enum.TryParse(user.Felhtipus, out FelhasznaloTipus userType) || !ut.Contains(userType))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, "Hozzáférés megtagadva");
                return;
            }

        }
    }
}
