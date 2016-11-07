using EM.Api.Core.OData;

namespace EM.Api.Core
{
    public class EMApiModelControllerProvider
    {
        public EMApiController Controller { get; set; }

        public SelectExpandResult GetCurrentSelectExpand<T>() where T: class 
        {            
            if (Controller.CurrentODataQueryOptions == null)
            {
                ODataDataQueryOptionsHelper hlp = new ODataDataQueryOptionsHelper(Controller);
                var queryOptions = hlp.GetODataQueryOptions<T>();
                Controller.CurrentODataQueryOptions = queryOptions;
                Controller.CurrentSelectExpandResult = hlp.GetSelectExpandResult(queryOptions);
            }
            return Controller.CurrentSelectExpandResult;
        }

        public EMApiModelControllerProvider(EMApiController controller)
        {
            this.Controller = controller;
        }


        

    }
}