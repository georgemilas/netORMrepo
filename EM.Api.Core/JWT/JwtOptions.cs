using System.Web;
using System.Web.Helpers;
using EM.Util;

namespace EM.Api.Core.JWT
{
    public class JwtOptions
    {
        public string KeyType { get { return "EM.Api.Core.JWT"; } }
        public string Issuer { get { return "EM"; } }
        public string Audience { get { return "all"; } }
        public byte[] Key
        {
            get
            {
                Encription c = new Encription(KeyType);
                return c.PasswordDeriveBytes.GetBytes(32);
            }
        }

    }
}