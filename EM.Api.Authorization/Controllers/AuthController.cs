using System;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using EM.Api.Authorization.Models;
using EM.Api.Core.JWT;

namespace EM.Api.Authorization.Controllers
{
    public class AuthController : Controller
    {


        [Route("Login")]
        public ActionResult Login()
        {
            var owinContext = HttpContext.GetOwinContext();
            var authentication = owinContext.Authentication;
            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("btLogin")))
                {
                    //var oc = owinContext.Get<OauthClient>("oauth:client");
                    var user = Request.Form["username"];
                    var pss = Request.Form["password"];
                                        
                    var spAuth = new AuthorizationProvider(owinContext);
                    var claimsAuth = spAuth.LoginUser(user, pss);
                    if (claimsAuth != null)
                    {
                        authentication.SignIn(new AuthenticationProperties(), new ClaimsIdentity(claimsAuth.Claims, "Application"));
                    }
                    else
                    {
                        ViewBag.Msg = "Invalid login, verify user name and password and try again;";
                    }
                }
            }

            return View();
        }

        [Route("Logout")]
        public ActionResult Logout()
        {
            return View();
        }

        [Route("Authorize")]
        public ActionResult Authorize()
        {
            if (Response.StatusCode != 200)
            {
                return View("AuthorizeError");
            }

            var owinContext = HttpContext.GetOwinContext();
            var authentication = owinContext.Authentication;
            var ticket = authentication.AuthenticateAsync("Application").Result;
            var identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }

            var scopes = (Request.QueryString.Get("scope") ?? "").Split(' ');
            
            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("btGrant")))
                {
                    var oc = owinContext.Get<OauthClient>("oauth:client");                    
                    AuthorizedUserProvider claimsAuth = new AuthorizedUserProvider(owinContext, identity);
                    identity = claimsAuth.GetClaimsBearerIdentity(identity, scopes, oc);
                    authentication.SignIn(identity);
                    //var jwtTicket = spAuth.GetJwtTicket(identity);
                    //authentication.SignIn(jwtTicket.Properties, jwtTicket.Identity);
                    
                }
                if (!string.IsNullOrEmpty(Request.Form.Get("btLogin")))
                {
                    authentication.SignOut("Application");
                    authentication.Challenge("Application");
                    return new HttpUnauthorizedResult();
                }
            }

            return View();
        }
    }
}