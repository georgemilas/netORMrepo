using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using EM.Api.Authorization.Controllers;
using Microsoft.Owin.Cors;
using EM.Api.Authorization.Models;
using EM.Api.Core.JWT;


namespace EM.Api.Authorization
{
    public partial class Startup
    {

        #region Main Config
        public void ConfigureAuthorization(IAppBuilder app)
        {

            
            app.UseCors(CorsOptions.AllowAll);

            // Setup Authorization Server
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AuthorizeEndpointPath = new PathString(Paths.AuthorizePath),
                // for authorization code flow part 1 do:  GET http:localhost/auth?response_type=code&client_id=CLIENT_ID&redirect_uri=http:localhost/back    ==> redirect to  http:localhost/back?code=TEMP_CODE 
                // for implicit flow do:  GET http:localhost/auth?response_type=token&client_id=CLIENT_ID&redirect_uri=http:localhost/back                    ==> redirect to  http:localhost/back?token=ACCESS_TOKEN    

                TokenEndpointPath = new PathString(Paths.TokenPath),
                // for resource owner password flow do:  POST http:localhost/token?grant_type=password&client_id=CLIENT_ID&username=USER&password=PASSWORD                ==> JSON with ACCESS_TOKEN and REFRESH_TOKEN
                // for authorization code flow part 2 do:  POST http:localhost/token?grant_type=code&client_id=CLIENT_ID&client_secret=SECRET&code=TEMP_CODE              ==> JSON with ACCESS_TOKEN and REFRESH_TOKEN
                // for refresh token flow do:  POST http:localhost/token?grant_type=refresh_token&client_id=CLIENT_ID&client_secret=SECRET&refresh_token=REFRESH_TOKEN    ==> JSON with ACCESS_TOKEN  
                // for client credentials flow do:  POST http:localhost/token?grant_type=client_credentials&client_id=CLIENT_ID&client_secret=SECRET                      ==> JSON with ACCESS_TOKEN and REFRESH_TOKEN


                ApplicationCanDisplayErrors = true,                 
                AuthenticationMode = AuthenticationMode.Active,
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
                AccessTokenFormat = new JwtFormat(),
                RefreshTokenFormat = new JwtFormat(),   //refresh token need not be JWT as we issue it and exchange it for an access token at the same endpoint
                
                //we must allow HTTP because IIS will never be configured with certificates etc. 
                //and we enforce and terminate HTTPS in BigIP only 

                AllowInsecureHttp = true,  //allow HTTP, without it only HTTPS would work
                
                // Authorization server provider which controls the lifecycle of Authorization Server
                Provider = new OAuthAuthorizationServerProvider    
                {
                    
                    OnValidateClientRedirectUri = ValidateClientRedirectUri,
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,    //grant_type=password
                    OnGrantRefreshToken = GrantRefreshToken,
                    //OnGrantClientCredentials = GrantClientCredetails                  //grant_type=client_credentials
                    OnValidateTokenRequest = ValidateTokenRequest
                },
                
                // Authorization code provider which creates and receives authorization code
                AuthorizationCodeProvider = new AuthenticationTokenProvider     //grant_type=authorization_code
                {           
                    OnCreate = CreateAuthenticationCode,
                    OnReceive = ReceiveAuthenticationCode,
                },
                
                AccessTokenProvider = new AuthenticationTokenProvider      
                {
                    OnCreate = CreateAccessToken,
                    OnReceive = ReceiveAccessToken,
                },

                // Refresh token provider which creates and receives referesh token
                RefreshTokenProvider = new AuthenticationTokenProvider      //grant_type=refresh_token 
                {
                    OnCreate = CreateRefreshToken,
                    OnReceive = ReceiveRefreshToken,
                }
            });
        }
        #endregion Main Config


        #region Client Validation
        /////////////////////////////  CLIENT VALIDATION  //////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        
       
        private Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            var ocp = new OauthClientProvider();
            var oc = ocp.GetOauthClient(context.ClientId,null);
            if (oc != null)
            {
                context.OwinContext.Set<OauthClient>("oauth:client", oc);
                if (!string.IsNullOrWhiteSpace(oc.OauthClientRedirectUrl))
                {
                    if (context.RedirectUri == oc.OauthClientRedirectUrl)
                    {
                        context.Validated(context.RedirectUri);
                    }
                }
                else
                {
                    context.Validated(context.RedirectUri);
                }
            }
            else
            {
                context.SetError("invalid client id", string.Format("Invalid client_id '{0}'", context.ClientId));                
            }
            return Task.FromResult(0);
        }
        
        private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret; 

            if (context.TryGetBasicCredentials(out clientId, out clientSecret) || context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                var ocp = new OauthClientProvider();
                var oc = ocp.GetOauthClient(clientId, clientSecret);
                if (oc!=null)
                {
                    //context.OwinContext.Set<string>("oauth:client", clientId);
                    context.OwinContext.Set<OauthClient>("oauth:client", oc);
                    context.Validated();  
                } 
                else
                {
                    if (clientSecret != null)
                    {
                        context.SetError("invalid client credentials",
                            string.Format("Invalid client_id {0} and/or client_secret {1}", clientId, clientSecret));
                    }
                    else
                    {
                        context.SetError("invalid client id", string.Format("Invalid client_id '{0}'", clientId));
                    }
                }
            }
            
            return Task.FromResult(0);
        }
        #endregion Client Validation


        #region Temp Token
        /////////////////////////////  TEMP TOKEN  //////////////////////////
        ////////////////////////////////////////////////////////////////////////

        private readonly ConcurrentDictionary<string, string> _authenticationCodes = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        private void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            _authenticationCodes[context.Token] = context.SerializeTicket();    
        }

        private void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }
        #endregion Temp Token


        #region Refresh Token
        /////////////////////////////  REFRESH TOKEN  //////////////////////////
        ////////////////////////////////////////////////////////////////////////
        
        private void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            //we should only issue refresh token to trusted clients, for example don't issue to single page applications in JavaScript as it can't securly store it and therefor it can be stolen                                            
            var client = context.OwinContext.Get<OauthClient>("oauth:client");
            if (client != null)
            {
                if (client.RefreshTokenEnabled && client.RefreshTokenExpireMinutes > 0)
                {
                    //for example access token expires in one hour and refresh token in one day
                    context.Ticket.Properties.ExpiresUtc = context.Ticket.Properties.IssuedUtc.Value.AddMinutes(client.RefreshTokenExpireMinutes);
                    context.SetToken(context.SerializeTicket());
                }
            }                            
        }

        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket != null)
            {
                var ci = context.Ticket.Identity.Claims.FirstOrDefault(c => c.Type == "em:ci");
                if (ci != null)
                {
                    var ocp = new OauthClientProvider();
                    var oc = ocp.GetOauthClient(JwtClaims.DecryptValue(ci.Value), null);
                    if (oc != null)
                    {
                        context.OwinContext.Set<OauthClient>("oauth:client", oc);                        
                    }    
                }                                
                //context.SetTicket(null);  //invalidate ticket                    
            }            
        }

        private Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var client = context.OwinContext.Get<OauthClient>("oauth:client");
            if (client != null)
            {
                if (client.RefreshTokenEnabled && client.RefreshTokenExpireMinutes > 0)
                {
                    var newId = new ClaimsIdentity(context.Ticket.Identity);
                    var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
                    context.Validated(newTicket);
                }
            }
            else
            {
                context.SetError("invalid client id");
            }
            return Task.FromResult(0);
        }
        #endregion Refresh Token


        #region Access Token
        /////////////////////////////  ACCESS TOKEN  //////////////////////////
        ////////////////////////////////////////////////////////////////////////
        private void CreateAccessToken(AuthenticationTokenCreateContext context)
        {
            var client = context.OwinContext.Get<OauthClient>("oauth:client");
            if (client != null)
            {
                if (client.AccessTokenExpireMinutes > 0)
                {
                    //for example access token expires in one hour and refresh token in one day
                    context.Ticket.Properties.ExpiresUtc = context.Ticket.Properties.IssuedUtc.Value.AddMinutes(client.AccessTokenExpireMinutes);
                    context.SetToken(context.SerializeTicket());
                }
            }                                        
        }

        private void ReceiveAccessToken(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);            
        }

            
        private Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }
        #endregion Access Token


        #region grant_type Password
        /////////////////////////////  grant_type PASSWORD   //////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        private Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });  //alow cors
            var client = context.OwinContext.Get<OauthClient>("oauth:client");

            var httpContextWrapper = context.OwinContext.Environment["System.Web.HttpContextBase"] as HttpContextWrapper;
            
            //todo: Set those in the database as a field, so we will know it is Employee or Employer
            var spAuth = new AuthorizationProvider(context.OwinContext);
            var claimsAuth = spAuth.LoginUser(context.UserName, context.Password);
            if (claimsAuth != null)
            {
                var identity = claimsAuth.GetClaimsBearerIdentityFromMobile(context.Scope, client);
                //only add valid scopes as roles
                context.Validated(identity);
                //var jwtTicket = spAuth.GetJwtTicket(identity);
                //context.Validated(jwtTicket);
            }
            else
            {
                context.SetError("Invalid username and/or password");
            }    
        
            return Task.FromResult(0);
        }
        #endregion grant_type Password


    }
}
