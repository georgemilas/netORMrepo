using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using EM.Api.Core.Emit.Serialize;

namespace EM.Api.Core
{
    public class HttpResultTypeNegotiator 
    {
        /// <summary>
        /// Custom Content Result negociator aware of the custom formatters (see GetFormatters)
        /// </summary>
        public static NegotiatedContentResult<T> GetNegotiator<T>(HttpStatusCode status, T value, EMApiController controller)
        {
            var formatters = GetFormatters(controller);
            var contentNegotiator = controller.Configuration.Services.GetContentNegotiator();            
            //var connegResult = contentNegotiator.Negotiate(typeof(object), contoller.Request, formatters);
            return new NegotiatedContentResult<T>(status, value, contentNegotiator, controller.Request, formatters);
        }

        public static CreatedAtRouteNegotiatedContentResult<T> GetCreatedAtRouteNegotiator<T>(string routeName, object routeValues, T value, EMApiController controller)
        {
            var formatters = GetFormatters(controller);
            var contentNegotiator = controller.Configuration.Services.GetContentNegotiator();
            UrlHelper urlFactory = controller.Url ?? new UrlHelper(controller.Request);
            //var rr= new CreatedAtRouteNegotiatedContentResult<T>(routeName, new HttpRouteValueDictionary(routeValues), value, controller);
            return new CreatedAtRouteNegotiatedContentResult<T>(routeName, new HttpRouteValueDictionary(routeValues), value, urlFactory, contentNegotiator, controller.Request, formatters);
        }

        /// <summary>
        /// The normal Configuration.Formatters execpt if there is a SPXmlSerializerMediaFormatter then it ensures it is not a singleton but instead <para></para>
        /// is an instance tied to the current controller action and so is aware of the current APiObjects being returned 
        /// </summary>
        public static IEnumerable<MediaTypeFormatter> GetFormatters(EMApiController controller)
        {
            List<MediaTypeFormatter> formatters = new List<MediaTypeFormatter>();
            foreach (var f in controller.Configuration.Formatters)
            {
                if (f is MediaFormatter)
                {
                    formatters.Add(controller.ApiObjectXmlFormatter);
                }
                else
                {
                    formatters.Add(f);
                }
            }
            return formatters;
        }

        
    }
}