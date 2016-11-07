using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using EM.Api.Core.JWT;
using EM.Api.Core.Models.Exceptions;

namespace EM.Api.Core
{

    public interface IAuthorizedTicket
    {
        //User SurePayrollUser { get; }
        IApiUser ApiUser { get; }

        
        //IEnumerable<string> AuthorizedRoles { get; }
        //string BclCode { get; }
    }

    /// <summary>
    /// Ensure IAuthorizedTicket properties are properly assigned such as BclCode and ISurePayrollUser
    /// </summary>
    public class EMAuthorizedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ControllerContext.Controller is EMApiController)
            {
                var controller = actionContext.ControllerContext.Controller as EMApiController;
                try
                {
                    var identity = (ClaimsIdentity)controller.User.Identity;
                    JwtClaims jwtClaims = JwtClaims.GetJwtClaimsFromBearerIdentity(identity);
                    if (jwtClaims != null)
                    {
                        IApiUser user = jwtClaims.ApiUser;

                        var clientId = jwtClaims.GetClaim("em:ci");
                        var appId = jwtClaims.GetClaim("em:ai");
                        var deviceId = jwtClaims.GetClaim("em:di");
                        var deviceTokenId = jwtClaims.GetClaim("em:dti");
                        var clientAppData = new ClientAppData()
                        {
                            ClientId = clientId != null ? jwtClaims.GetDecryptedValue(clientId.Value) : null,
                            AppId = appId != null ? int.Parse(appId.Value) : (int?)null,
                            DeviceId = deviceId != null ? deviceId.Value : null,
                            DeviceTokenId = deviceTokenId != null ? deviceTokenId.Value : null
                        };

                        //var sessionClaim = identity.Claims.FirstOrDefault(c => c.Type == "urn:em:sessionid");                    
                        //if (sessionClaim != null)
                        //{
                        //    var sessionId = sessionClaim.Value;
                        //    //validate as well as extend session if valid 
                        //    user.ValidateAndPopulate(sessionId, clientAppData.AppId ?? 0, controller.Request.RequestUri.AbsolutePath, "", 10);
                        //    if (!user.IsValidUser || user.IsSessionExpired)
                        //    {
                        //        throw new InvalidOperationException("User is not authorized or session expired");
                        //    }
                        //}

                        controller.ApiUser = user;
                        controller.ClientAppData = clientAppData;
                        //controller.AuthorizedRoles = (from c in identity.Claims where c.Type == "urn:oauth:scope" select c.Value).ToList();
                    }
                    else
                    {
                        var m = new ErrorMessage(new ApiAuthorizationException("Bearer token provided is invalid, could not match referential integrity"));
                        var response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, m);
                        throw new HttpResponseException(response);
                    }
                }
                catch (Exception err)
                {
                    var m = new ErrorMessage(new ApiAuthorizationException("Bearer token provided has some invalid data and is not authorized for this request: " + err.Message, err));
                    var response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, m);
                    throw new HttpResponseException(response);
                }
            }
            base.OnActionExecuting(actionContext);
        }
    }

}