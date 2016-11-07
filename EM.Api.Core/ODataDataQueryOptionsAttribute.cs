using EM.Api.Core.OData;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using EM.Api.Core.Metadata;
using EM.Api.Core.Models.Exceptions;

namespace EM.Api.Core
{
    /// <summary>
    /// Ensure parsing ODataDataQueryOptions in Request and set controller's CurrentODataQueryOptions and CurrentSelectExpandResult
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ODataDataQueryOptionsAttribute : ActionFilterAttribute
    {
        public ODataDataQueryOptionsAttribute(Type entityType)
        {
            this.EntityType = entityType;
        }
        public Type EntityType { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ControllerContext.Controller is EMApiController)
            {
                var controller = actionContext.ControllerContext.Controller as EMApiController;
                try
                {
                    ODataDataQueryOptionsHelper hlp = new ODataDataQueryOptionsHelper(controller);
                    var queryOptions = hlp.GetODataQueryOptions(EntityType);
                    controller.CurrentODataQueryOptions = queryOptions;
                    controller.CurrentSelectExpandResult = hlp.GetSelectExpandResult(queryOptions, EntityType);                                    
                }
                catch (Exception err)
                {
                    var m = new ErrorMessage(new ApiParameterParsingException("An error occured: " + err.Message, err));
                    var response = actionContext.Request.CreateResponse(m.HttpStatus, m);
                    throw new HttpResponseException(response);
                }
            }
            base.OnActionExecuting(actionContext);
        }
    }
}