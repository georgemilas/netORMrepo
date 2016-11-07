using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.Owin;
using EM.Api.Core.JWT;


namespace EM.Api.Authorization.Models
{
    public class AuthorizationProvider
    {
        public IOwinContext OwinContext { get; private set; }

        public AuthorizationProvider(IOwinContext owinContext)
        {
            OwinContext = owinContext;
        }

        public OauthClient OAuthClient
        {
            get { return OwinContext.Get<OauthClient>("oauth:client"); }
        }

        public AuthorizedUserProvider LoginUser(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                var user = new ApiUser();
                return new AuthorizedUserProvider(OwinContext, user);
            }

            return null;
        }
    }

    public class AuthorizedUserProvider
    {
        public IOwinContext OwinContext { get; private set; }
        public IApiUser User { get; private set; }
        public List<Claim> Claims { get; private set; }

        public AuthorizedUserProvider(IOwinContext owinContext, IApiUser user)
        {
            OwinContext = owinContext;
            User = user;
            JwtClaims jwtClaims = JwtClaims.GetJwtClaimsFromUser(User);
            Claims = jwtClaims.GetUserClaims();
        }
        public AuthorizedUserProvider(IOwinContext owinContext, ClaimsIdentity cookieIdentity)
        {
            OwinContext = owinContext;
            JwtClaims jwtClaims = JwtClaims.GetJwtClaimsFromCookieIdentity(cookieIdentity);
            User = jwtClaims.ApiUser;
            Claims = jwtClaims.Claims;
        }

        

        public IEnumerable<string> GetFilteredScopes(IEnumerable<string> scopes, OauthClient oc)
        {
            var httpContextWrapper = OwinContext.Environment["System.Web.HttpContextBase"] as HttpContextWrapper;
            var appId = httpContextWrapper.Request.Params["appId"];
            try
            {
                OauthClientProvider ocp = new OauthClientProvider();
                List<string> roles = ocp.GetOauthClientSecAppRole(oc.ClientId);
                var filteredScopes = roles == null ? new List<string>() : scopes.Where(scope => roles.Contains(scope));
                return filteredScopes;
            }
            catch
            {
                return scopes;
            }
        }

        public ClaimsIdentity GetClaimsBearerIdentity(ClaimsIdentity cookieIdentity, IEnumerable<string> scopes, OauthClient oc)
        {
            List<Claim> claims = new List<Claim>(cookieIdentity.Claims);
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Bearer", cookieIdentity.NameClaimType, cookieIdentity.RoleClaimType);
            var filteredScopes = GetFilteredScopes(scopes, oc);
            foreach (var scope in filteredScopes)
            {
                //identity.AddClaim(new Claim("oauth:scope", scope));
                identity.AddClaim(new Claim(ClaimTypes.Role, scope));
            }
            return identity;
        }

        public ClaimsIdentity GetClaimsBearerIdentityFromMobile(IEnumerable<string> scopes, OauthClient oc)
        {
            //SetMobileDataClaims();
            var filteredScopes = GetFilteredScopes(scopes, oc);
            foreach (var scope in filteredScopes)
            {
                //identity.AddClaim(new Claim("oauth:scope", scope));
                Claims.Add(new Claim(ClaimTypes.Role, scope));
            }

            var identity = new ClaimsIdentity(this.Claims, "Bearer");
            return identity;
        }

        //public AuthenticationTicket GetJwtTicket(ClaimsIdentity bearerIdentity)
        //{
        //    var props = new AuthenticationProperties();
        //    var ticket = new AuthenticationTicket(bearerIdentity, props);            
        //    return ticket;
        //}        

    }


    


}