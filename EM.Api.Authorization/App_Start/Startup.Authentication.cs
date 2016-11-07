using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EM.Api.Authorization
{
    public partial class Startup
    {
        
        public void ConfigureAuthentication(IAppBuilder app)
        {
            // Enable Application Sign In Cookie
            //the cookie should be expired after 10 min
            //in lower environment, you can set it longer then that
            var cookieExpireTime = ConfigurationManager.AppSettings["cookieExpireTime"];
            int icookExpTime =!string.IsNullOrEmpty(cookieExpireTime)?Convert.ToInt32(cookieExpireTime): 10;
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Application",
                AuthenticationMode = AuthenticationMode.Passive,
                LoginPath = new PathString(Paths.LoginPath),
                LogoutPath = new PathString(Paths.LogoutPath),
                ExpireTimeSpan = TimeSpan.FromMinutes(icookExpTime) 
            });
        }
    }
}
