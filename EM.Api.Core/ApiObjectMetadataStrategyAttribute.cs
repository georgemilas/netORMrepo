using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace EM.Api.Core
{
    /// <summary>
    /// Specify one of TypeDescriptorBasedTypeMetadataStrategy or ReflectionBasedTypeMetadataStrategy<para></para>
    /// If not specified, by default it assumes TypeDescriptorBasedTypeMetadataStrategy
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ApiObjectMetadataStrategyAttribute : ActionFilterAttribute
    {
        public ApiObjectMetadataStrategyAttribute(Type strategyType)
        {
            this.MetadataStrategyType = strategyType;
        }
        protected Type MetadataStrategyType { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ControllerContext.Controller is EMApiController)
            {
                var controller = actionContext.ControllerContext.Controller as EMApiController;
                controller.ApiObjectMetadataStrategyType = MetadataStrategyType;                                    
            }
            base.OnActionExecuting(actionContext);
        }
    }
}