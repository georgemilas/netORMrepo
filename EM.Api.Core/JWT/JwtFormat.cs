using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;

namespace EM.Api.Core.JWT
{
    public class JwtFormat : ISecureDataFormat<AuthenticationTicket>
    {

        public string Protect(AuthenticationTicket data)
        {
            if (data == null) { throw new ArgumentNullException("data"); }
            //string clientId = data.Properties.Dictionary.ContainsKey("clientId") ? data.Properties.Dictionary["clientId"] : null;

            var options = new JwtOptions();
            
            var now = new DateTimeOffset(DateTime.Now);
            var issued = data.Properties.IssuedUtc ?? now;
            var expires = data.Properties.ExpiresUtc ?? now + TimeSpan.FromHours(1);
            //TODO: instead of InMemorySymmetricSecurityKey, use a certificate and X509SecurityKey
            var signingCredentials = new SigningCredentials(
                                    new InMemorySymmetricSecurityKey(options.Key),
                                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                                    "http://www.w3.org/2001/04/xmlenc#sha256");
            var claims = JwtClaims.AddHashClaims(data.Identity, issued, expires); 
            var token = new JwtSecurityToken(options.Issuer, options.Audience, claims, issued.UtcDateTime, expires.UtcDateTime, signingCredentials);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);


            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            var handler = new JwtSecurityTokenHandler();
            SecurityToken st;
            var options = new JwtOptions();
            var sc = new SigningCredentials(
                                    new InMemorySymmetricSecurityKey(options.Key),
                                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                                    "http://www.w3.org/2001/04/xmlenc#sha256");
            var pars = new TokenValidationParameters();
            pars.IssuerSigningKey = sc.SigningKey;
            pars.ValidIssuer = options.Issuer;
            pars.ValidAudience = options.Audience;
            try
            {
                var principal = handler.ValidateToken(protectedText, pars, out st);
                var ticket = JwtClaims.GetAuthenticatedTicketFromFederatedIdentity(principal);
                return ticket;
            }
            catch (SecurityTokenException)   //expired, wrong issuer, audience, sure payroll hash etc.
            {
                return null;
            }
        }
    }
}