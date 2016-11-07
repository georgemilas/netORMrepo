using System;
using System.Reflection;
using System.Security;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;

namespace EM.Api.Core.OData
{
    public class ODataDataQueryOptionsHelper
    {
        public EMApiController Controller { get; set; }

        public ODataDataQueryOptionsHelper(EMApiController controller)
        {
            this.Controller = controller;
        }

        /// <summary>
        /// Instead of having ODataDataQueryOptions as a parameter in the controler action and be automaticaly created by the WebAPI runtime 
        /// like     public IHttpActionResult Index(ODataQueryOptions[RemindersMonth> queryOptions)
        /// 
        /// we are manually building it from the Request 
        /// we do this to be friendly with swagger documentation generator (Swashbuckle) 
        /// otherwise swagger UI will say the api requires queryOptions parameter, which is not the case
        /// </summary>
        public ODataQueryOptions<T> GetODataQueryOptions<T>() where T : class
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<T>(typeof(T).Name);
            var model = builder.GetEdmModel();

            var cx = new System.Web.Http.OData.ODataQueryContext(model, typeof(T));
            ODataQueryOptions<T> opt = new ODataQueryOptions<T>(cx, Controller.Request);
            return opt;
        }

        /// <summary>
        /// Reflection based version of GetODataDataQueryOptions[T] 
        /// </summary>
        public ODataQueryOptions GetODataQueryOptions(Type entityType)
        {            
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            //do: builder.EntitySet<EntityType>(EntityType.Name);
            var tp = builder.GetType();
            MethodInfo method = tp.GetMethod("EntitySet");
            MethodInfo generic = method.MakeGenericMethod(entityType);
            generic.Invoke(builder, new object[] { entityType.Name });
            var model = builder.GetEdmModel();
            var cx = new System.Web.Http.OData.ODataQueryContext(model, entityType);
            //var queryOptions = new ODataQueryOptions<T>(cx, Request);
            var queryOptionType = typeof(ODataQueryOptions<>);
            Type[] lstArgs = { entityType };
            Type genericQueryOptionType = queryOptionType.MakeGenericType(lstArgs);
            var queryOptions = (ODataQueryOptions)Activator.CreateInstance(genericQueryOptionType, cx, Controller.Request);
            return queryOptions;
        }

        public SelectExpandResult GetSelectExpandResult<T>(ODataQueryOptions<T> queryOptions) //, Type parentType)
        {
            return GetSelectExpandResult(queryOptions, typeof(T));
        }

        public SelectExpandResult GetSelectExpandResult(ODataQueryOptions queryOptions, Type entityType)
        {
            return SelectExpandResult.GetFromODataExpandAndCustomSelectAndCustomQuery(queryOptions, entityType, Controller.QueryString.Get("$query"));            
        }
    }
}