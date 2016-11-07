using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Owin.Security;
using EM.Util;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace EM.Api.Core.JWT
{
    
    public interface IApiUser 
    {
        object UserId { get; }
    }

    public class ApiUser : IApiUser
    {
        public virtual object UserId { get; set; }
        //public string Serialize()
        //{
        //    return JsonConvert.SerializeObject(this);
        //}
        //public static User<T> Deserialize(string userStr)
        //{
        //    return JsonConvert.DeserializeObject<User<T>>(userStr);
        //}
    }

    public class JwtClaims
    {
        public IApiUser ApiUser { get; set; }
        public List<Claim> Claims { get; set; }
        
        private Encription _crypt;
        private const int HASH_ITERATIONS = 100;

        private JwtClaims()
        {
            _crypt = GetCrypt();
        }

        public static JwtClaims GetJwtClaimsFromUser(IApiUser user)
        {
            var res = new JwtClaims();
            res.ApiUser = user;
            return res;
        }
        public static JwtClaims GetJwtClaimsFromCookieIdentity(ClaimsIdentity identity)
        {
            var res = new JwtClaims();
            res.Claims = new List<Claim>(identity.Claims);
            var user = res.GetDecryptedClaim("em:sui");
            res.ApiUser = JsonConvert.DeserializeObject<IApiUser>(user);
            return res;
        }

        public static JwtClaims GetJwtClaimsFromBearerIdentity(ClaimsIdentity identity)
        {
            return GetJwtClaimsFromClaims(identity.Claims);
        }
        public static JwtClaims GetJwtClaimsFromFederatedIdentity(ClaimsPrincipal principal)
        {
            return GetJwtClaimsFromClaims(principal.Claims);
        }
        public static JwtClaims GetJwtClaimsFromClaims(IEnumerable<Claim> claims)
        {
            var res = new JwtClaims();
            res.Claims = new List<Claim>(claims);

            if (VerifyHash(res.Claims))
            {
                var user = res.GetDecryptedClaim("em:sui");
                res.ApiUser = JsonConvert.DeserializeObject<IApiUser>(user);
                return res;
            }

            return null;
        }

        public Claim GetClaim(string claimKey)
        {
            return Claims.FirstOrDefault(c => c.Type == claimKey);
        }
        public List<Claim> GetClaims(string claimKey)
        {
            return Claims.FindAll(c => c.Type == claimKey);
        }
        public string GetDecryptedClaim(string claimKey)
        {
            return _crypt.Decrypt(GetClaim(claimKey).Value);
        }
        public static string EncryptValue(string decryptedValue)
        {
            var jwt = new JwtClaims();
            return jwt.GetEncryptedValue(decryptedValue);
        }
        public string GetEncryptedValue(string decryptedValue)
        {
            return _crypt.Encrypt(decryptedValue);
        }
        public static string DecryptValue(string encyptedValue)
        {
            var jwt = new JwtClaims();
            return jwt.GetDecryptedValue(encyptedValue);
        }
        public string GetDecryptedValue(string encyptedValue)
        {
            return _crypt.Decrypt(encyptedValue);
        }

        public static Encription GetCrypt()
        {
            JwtOptions op = new JwtOptions();
            return new Encription(op.KeyType);
        }

        public static IEnumerable<Claim> AddHashClaims(ClaimsIdentity identity, DateTimeOffset created, DateTimeOffset expires)
        {
            var crypt = GetCrypt();
            List<Claim> claims = new List<Claim>(identity.Claims);

            //delete existing hash claims and create new ones
            Action<string> delClaim = key =>
            {
                var clm = claims.FirstOrDefault(c => c.Type == key);
                if (clm != null)
                {
                    claims.Remove(clm);
                }
            };
            delClaim("em:exp");
            delClaim("em:iat");
            delClaim("em:sle");
            delClaim("em:slh");

            var encSecLoginId = claims.First(c => c.Type == "em:sli").Value;
            var encSecUserTypeId = claims.First(c => c.Type == "em:sut").Value;
            var encNonce = claims.First(c => c.Type == "em:jti").Value;

            var hashCrypt = new Encription("EncryptionKeySets.RFC_2898"); // new Crypt(EncryptionKeySets.RFC_2898);
            var salt = Encoding.UTF8.GetString(hashCrypt.PasswordDeriveBytes.Salt);
            var saltEnc = crypt.Encrypt(salt);
            var value = encSecLoginId + encSecUserTypeId + encNonce + expires.UtcTicks + created.UtcTicks;
            var hash = Encription.GetRfc2898Hash(value, salt, HASH_ITERATIONS, 64);

            claims.Add(new Claim("em:exp", expires.UtcTicks.ToString()));
            claims.Add(new Claim("em:iat", created.UtcTicks.ToString()));
            claims.Add(new Claim("em:sle", saltEnc));
            claims.Add(new Claim("em:slh", hash));
            return claims;
        }

        public static bool VerifyHash(IEnumerable<Claim> claims)
        {
            var crypt = GetCrypt();
            var encSecLoginId = claims.First(c => c.Type == "em:sli").Value;
            var encSecUserTypeId = claims.First(c => c.Type == "em:sut").Value;
            var encNonce = claims.First(c => c.Type == "em:jti").Value;
            var expiresTicks = claims.First(c => c.Type == "em:exp").Value;
            var createdTicks = claims.First(c => c.Type == "em:iat").Value;
            var salt = crypt.Decrypt(claims.First(c => c.Type == "em:sle").Value);
            var value = encSecLoginId + encSecUserTypeId + encNonce + expiresTicks + createdTicks;
            //var hashCrypt = new Crypt(EncryptionKeySets.RFC_2898);
            var computedHash = Encription.GetRfc2898Hash(value, salt, HASH_ITERATIONS, 64);
            var jwtHash = claims.First(c => c.Type == "em:slh").Value;
            DateTime expireUtc = new DateTime(long.Parse(expiresTicks), DateTimeKind.Utc);  
            //we do own tracking of expiration time because .NET OAuth RefreshTokens normaly never expire but we want them to expire
            //allow for plus/minus one/two minues of incosistency in case clocks are not properly synced
            //return computedHash == jwtHash && DateTime.UtcNow.AddMinutes(-1) < expireUtc.AddMinutes(1);
            return computedHash == jwtHash && DateTime.UtcNow < expireUtc;            
            
        }

        public List<Claim> GetUserClaims()
        {
            var claims = new List<Claim>();
            var userName = _crypt.Encrypt(ApiUser.UserId.ToString());
            //can't use User.FullName we would need User.ValidateAndPopulate to have it set
            //var fullName = User.FirstName + " " + User.LastName;
            //claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, fullName));
            //claims.Add(new Claim(ClaimTypes.Name, fullName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));

            //No need to store BCL / Employee in the ticket, it will be known via SecLoginId/SecUserTypeId
            //claims.Add(new Claim("urn:em:enc", User.BCL_Code));
            //claims.Add(new Claim("urn:em:e1id", User.E1_ID.ToString()));
            
            var nonce = Guid.NewGuid().ToString();
            claims.Add(new Claim("em:sui", _crypt.Encrypt(ApiUser.UserId.ToString())));
            claims.Add(new Claim("em:jti", _crypt.Encrypt(nonce)));
            
            return claims;
        }

        public static AuthenticationTicket GetAuthenticatedTicketFromFederatedIdentity(ClaimsPrincipal principal)
        {
            //verify hash and do own tracking of expiration time because .NET OAuth RefreshTokens normaly never expire but we want them to expire
            if (!JwtClaims.VerifyHash(principal.Claims))  
            {
                return null;    
            }
                
            //var expireMinutes = 60;
            //var ci = principal.Claims.FirstOrDefault(c => c.Type == "em:ci");
            //if (ci != null)
            //{
            //    var ocp = new OauthClientProvider();
            //    var oc = ocp.GetOauthClient(DecryptValue(ci.Value), null);                
            //    var expireMinutes = oc.RefreshTokenExpireMinutes 
            //}          

            var props = new AuthenticationProperties();
            props.AllowRefresh = true;
            props.IssuedUtc = DateTime.UtcNow;
            props.ExpiresUtc = props.IssuedUtc.Value.AddHours(1);  //temporary expire time, the AccessToken / RefreshToken will get proper expire time as configured in DB
            var identity = new ClaimsIdentity(principal.Identity);
            //identity.AddClaims(principal.Claims);
            return new AuthenticationTicket(identity, props);                


        }

    }
}