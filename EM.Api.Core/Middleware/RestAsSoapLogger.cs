using Owin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using EM.Api.Core.JWT;
using System.Security.Claims;

namespace EM.Api.Core.Middleware
{
    public class RestAsSoapLogger : OwinMiddleware
    {

        public RestAsSoapLogger(OwinMiddleware nextMiddleware) : base(nextMiddleware) { }

        public long Log(string text, IOwinContext context)
        {
            
            string clientId = "";
            int appId = 0;
            bool isAuthorized = false;
            try
            {
                if (context.Request.User.Identity is ClaimsIdentity)
                {
                    JwtClaims jwtClaims = JwtClaims.GetJwtClaimsFromBearerIdentity(context.Request.User.Identity as ClaimsIdentity);
                    if (jwtClaims != null)
                    {
                        isAuthorized = true;
                        //ISurePayrollUser user = jwtClaims.User;
                        var client = jwtClaims.GetClaim("em:ci");
                        var app = jwtClaims.GetClaim("em:ai");
                        clientId = client != null ? jwtClaims.GetDecryptedValue(client.Value) : null;
                        appId = app != null ? int.Parse(app.Value) : 0;
                        if (clientId != null)
                        {
                            text = "ClientID: " + clientId + Environment.NewLine + text;
                        }
                    }
                }
            }
            catch { }

            long? id = null;
            //we have only one log for both Request and Response so we just set MessageType to 4 aka "Debug Message" (should we create a new LogWebServiceMessageTypeID?)
            //AddLogWebService2.execute(cx, 4, text, context.Request.RemoteIpAddress, isAuthorized, appId, 0, ref id);  
            return id.Value;
                        
        }

        public override async Task Invoke(IOwinContext context)
        {
            var url = context.Request.Uri.AbsoluteUri ?? "";
            var lUrl = url.ToLower();

            //TODO: add include/exclude list of items in web.config
            if ((lUrl.Contains("/v1/") || lUrl.Contains("/auth/"))   //include
                && !lUrl.Contains("/swagger/"))                     //exclude
            {
                //await Log(context.Request.Body);          
                var requestBuffer = new MemoryStream();
                var requestStream = new ContentStream(requestBuffer, context.Request.Body);
                context.Request.Body = requestStream;

                // replace the response stream in order to intercept downstream writes
                var responseBuffer = new MemoryStream();
                var responseStream = new ContentStream(responseBuffer, context.Response.Body);
                context.Response.Body = responseStream;


                await Next.Invoke(context);

                // rewind the request and response buffers and record their content
                var input = await WriteContentAsync(requestStream, context.Request.Headers);
                var output = await WriteContentAsync(responseStream, context.Response.Headers);



                StringBuilder sb = new StringBuilder();
                sb.AppendLine(context.Request.Method + " " + context.Request.Uri.AbsoluteUri);
                sb.AppendLine(input);
                sb.AppendLine();
                sb.AppendLine("------------------------------------------------");
                sb.AppendLine("RESULT: " + context.Response.StatusCode);
                sb.AppendLine(output);
                long id = Log(sb.ToString(), context);

                // add the Log Id in the response headers so that the user of the API can correlate 
                context.Response.Headers.Add("Http-RestAsSoapLogger-Id", new[] {id.ToString()});
            }
            else
            {
                await Next.Invoke(context);
            }
            
        }

        private static async Task<string> WriteContentAsync(ContentStream stream, IDictionary<string, string[]> headers)
        {
            var contentType = headers.ContainsKey("Content-Type") ? headers["Content-Type"][0] : null;
            return await stream.ReadContentAsync(contentType, Int64.MaxValue);
        }
    }
}