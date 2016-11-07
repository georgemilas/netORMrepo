using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EM.Api.Authorization.Models
{
    public class OauthClientProvider
    {
        public static List<OauthClient> GetAllOauthClients()
        {
            return new List<OauthClient>
            {
                new OauthClient
                {
                    ClientId = 1,
                    ClientName = "TestClient",
                    AccessTokenExpireMinutes = 10,
                    OauthClientId = Guid.NewGuid().ToString(),
                    OauthClientSecret = Guid.NewGuid().ToString(),
                    RefreshTokenEnabled = true,
                    RefreshTokenExpireMinutes = 20,
                    OauthClientRedirectUrl = ""
                }
            };            
        }

        private List<OauthClient> _allOauthClients;
        public List<OauthClient> AllOauthClients
        {
            get
            {
                if (_allOauthClients == null)
                {
                    _allOauthClients = GetAllOauthClients();
                }
                return _allOauthClients;
            }            
        }


        public OauthClient GetOauthClient(string clientId, string clientSecret)
        {
            var ocs = AllOauthClients;
            if (!string.IsNullOrWhiteSpace(clientSecret)) //clientSecret is not null , so check both  clientId &  clientSecret
            {
                return ocs.FirstOrDefault(oc => oc.OauthClientId == clientId && oc.OauthClientSecret == clientSecret);
            }
            return ocs.FirstOrDefault(oc => oc.OauthClientId == clientId);
        }

        public List<string> GetOauthClientSecAppRole(int clientId)
        {
            return new List<string>();            
        }

    }
}