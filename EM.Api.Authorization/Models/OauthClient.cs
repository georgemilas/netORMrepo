using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EM.Api.Authorization.Models
{
    public class OauthClient
    {
        public string ClientName { get; set; }
        public string OauthClientId { get; set; }
        public string OauthClientSecret { get; set; }
        public string OauthClientRedirectUrl { get; set; }
        public int ClientId { get; set; }
        public int AccessTokenExpireMinutes { get; set; }
        public bool RefreshTokenEnabled { get; set; }
        public int RefreshTokenExpireMinutes { get; set; }
    }
}