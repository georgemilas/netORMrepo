using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;
using Microsoft.Data.OData;
using System.Net;
using EM.Api.Core.Emit;
using EM.Api.Core.OData;
using EM.Api.Core.Emit.Serialize;
using EM.Api.Core.Metadata;
using System.Configuration;
using System.Net.Http;
using System.Web.OData;
using System.Web;
using Microsoft.Owin;
using EM.Api.Core.Models.Exceptions;
using EM.Api.Core.JWT;

namespace EM.Api.Core
{
    
    //class PerControllerXmlFormatterAttribute : Attribute, IControllerConfiguration
    //{
    //    public void Initialize(HttpControllerSettings controllerSettings, 
    //                           HttpControllerDescriptor controllerDescriptor)
    //    {
    //        var frm = controllerSettings.Formatters.First(m => m is SPXmlSerializerMediaFormatter);
    //        if (frm != null)
    //        {
    //            controllerSettings.Formatters.Remove(frm);
    //            controllerSettings.Formatters.Add(new SPXmlSerializerMediaFormatter());
    //        }            
    //    }
    //}

    //ApiController action invocation lifecycle http://stackoverflow.com/questions/12277293/execution-order-for-the-apicontroller
    public abstract class EMApiController : ApiController, IAuthorizedTicket
    {
        /*
         *  ATTENTION: Make all methods in this file protected or private otherwise 
         *  public ones will count as a valid and routeable controller action which you probably don't want
        */

        public virtual HttpContextWrapper HttpContextWrapper
        {
            get
            {
                //var owinContext = HttpContext.Current.GetOwinContext();
                //var httpContextWrapper = (HttpContextWrapper)Request.Properties["MS_HttpContext"];
                try
                {
                    var owinContext = (IOwinContext) Request.Properties["MS_OwinContext"];
                    var httpContextWrapper = owinContext.Environment["System.Web.HttpContextBase"] as HttpContextWrapper;
                    return httpContextWrapper;
                }
                catch
                {
                    return null;    
                }                
            }
        }

        /// <summary>
        /// Return Url to be used as the base of an ApiObject.Href value
        /// </summary>
        protected string GetHrefRoot(string host, string virtualFolder)
        {
            var uri = Request.RequestUri;
            var scheme = uri.Scheme.ToLower().Trim();
            //we want the port mostly for localhost dev machines where we may have special ports
            var port = ((scheme == "http" && uri.Port != 80) || (scheme == "https" && uri.Port != 443)) ? ":" + uri.Port : "";

            if (HttpContextWrapper != null)
            {
                //Our BigIP firewall terminates HTTPS and tunnels over to actual IIS server as HTTP 
                //so the IIS server may only see HTTP, however we must still generate HTTPS urls in this case
                var bigIPheader = HttpContextWrapper.Request.Headers["HTTP_X_SSL_CONN"];
                var bigIPheader2 = HttpContextWrapper.Request.ServerVariables["HTTP_X_SSL_CONN"];
                if ((bigIPheader != null && bigIPheader == "443") || (bigIPheader2 != null && bigIPheader2 == "443"))
                {
                    scheme = "https";
                    port = "";
                }
            }
            return string.Format("{0}://{1}{2}{3}", scheme, host, port, virtualFolder != "/" ? virtualFolder : "");            
        }

        private string LocalRootHost
        {
            get
            {
                var virtualFolder = ControllerContext.RequestContext.VirtualPathRoot ?? "";
                return GetHrefRoot(Request.RequestUri.Host, virtualFolder);                
            }
        }

        private string RootHost
        {
            get
            {
                var rootFactory = ConfigurationManager.AppSettings["URL_ROOT_FACTORY"];
                //when we are proxied via an API management platform such as Azure, we want the url to be the proxy url  
                if (rootFactory != null && rootFactory.ToLower() == "azure")  
                {
                    var lp = Request.RequestUri.LocalPath.ToLower();
                    if (lp.Contains("/todoes") || lp.Contains("/people") || lp.Contains("/projects"))
                    {
                        return GetHrefRoot("milas.azure-api.net", "/Todoes"); //http(s)://milas.azure-api.net/Todoes -> Azure "Todoes API"
                    }
                    return GetHrefRoot("milas.azure-api.net", "/Payroll");    //http(s)://milas.azure-api.net/Payroll -> Azure "Payroll API"
                    
                }
                return LocalRootHost;                
            }
        }

        public abstract string UrlVersionPrefix { get; }

        public string RootUrl
        {
            get
            {                
                return string.Format("{0}/{1}", this.RootHost, UrlVersionPrefix);                
            }
        }

        private MediaFormatter _ApiObjectXmlFormatter;
        private bool _verifiedRegisteredFormatters;
        public MediaFormatter ApiObjectXmlFormatter
        {
            get
            {
                if (_ApiObjectXmlFormatter == null && !_verifiedRegisteredFormatters)
                {
                    foreach (var f in Configuration.Formatters)
                    {
                        if (f is MediaFormatter)
                        {
                            _ApiObjectXmlFormatter = ((MediaFormatter)f).GetNewFormatterInstance();
                            break;
                        }
                    }
                    _verifiedRegisteredFormatters = true;
                }
                return _ApiObjectXmlFormatter;
            }
        }

        //public SurePayroll.Business.Entity.User SurePayrollUser { get; set; }
        public IApiUser ApiUser { get; set; }
        public ClientAppData ClientAppData { get; set; }
        
        public ODataQueryOptions CurrentODataQueryOptions { get; set; }
        protected ODataQueryOptions<T> GetCurrentODataQueryOptions<T>() where T : class
        {
            return (ODataQueryOptions<T>)CurrentODataQueryOptions;             
        }
        public SelectExpandResult CurrentSelectExpandResult { get; set; }
        

        /// <summary>
        /// The underlying root entity type (CurrentPayroll, Employee, Photo etc.) used 
        /// </summary>
        public Type ApiObjectMetadataStrategyType { get; set; }
        /// <summary>
        /// The strategy used to determine the configuration (MetadataConfiguration or Attrributes configuration) for the underlying root entity type ApiObjectMetadataStrategyType
        /// </summary>
        public Func<Type, TypeMetadataStrategy> ApiObjectMetadataStrategy
        {
            get
            {
                if (ApiObjectMetadataStrategyType == null)                    
                {
                    return t => new TypeDescriptorBasedTypeMetadataStrategy(t);
                }
                if (ApiObjectMetadataStrategyType == typeof (ReflectionBasedTypeMetadataStrategy))
                {
                    return t => new ReflectionBasedTypeMetadataStrategy(t);
                }
                return t => new TypeDescriptorBasedTypeMetadataStrategy(t);
            }
        }
        

        ///// <summary>
        ///// If OAuth scope attrinbutes are used as RoleClaims this is not necesary 
        ///// </summary>
        //protected bool IsUserAuthorizedFor(string role)
        //{
        //    var lrole = role.ToLower();
        //    return AuthorizedRoles.Any(s => s.ToLower() == lrole);
        //}

        private List<KeyValuePair<string, string>> _queryString;
        public QueryStringValues QueryString
        {
            get
            {
                if (_queryString == null)
                {
                    _queryString = Request.GetQueryNameValuePairs().ToList();
                }
                return new QueryStringValues(_queryString);
            }
        }  

        //protected void ProcessQuery()
        //{
        //    KeyValuePair<string, string> querykv = Request.GetQueryNameValuePairs().FirstOrDefault(kv => kv.Key == "$query");
        //    if (querykv.Value != null)
        //    {
        //        var query = querykv.Value;

        //    }
        //}
       
        /// <summary>
        /// Map a collection of objects to an collection of ApiObjects based on the select/expand rules requested in the query string as well as in the object MetadataConfiguration or Attrributes configuration
        /// </summary>
        protected ResultWrap<IEnumerable<object>> GetApiObjects<T>(IEnumerable<T> data, ObjectAndParent parent = null, IEnumerable<AdditionalProperty> additionalProprieties = null) where T: class
        {
            if (CurrentODataQueryOptions == null)
            {
                ODataDataQueryOptionsHelper hlp = new ODataDataQueryOptionsHelper(this);
                var queryOptions = hlp.GetODataQueryOptions<T>();
                CurrentODataQueryOptions = queryOptions;
                CurrentSelectExpandResult = hlp.GetSelectExpandResult(queryOptions);
            }
            return GetApiObjects(data, GetCurrentODataQueryOptions<T>(), CurrentSelectExpandResult, parent, additionalProprieties);
        }

        /// <summary>
        /// Map a collection of objects to an collection of ApiObjects based on the select/expand rules parameter
        /// </summary>
        protected ResultWrap<IEnumerable<object>> GetApiObjects<T>(IEnumerable<T> data, ODataQueryOptions<T> queryOptions, SelectExpandResult se, ObjectAndParent parent = null, IEnumerable<AdditionalProperty> additionalProprieties = null)
        {
            IEnumerable<T> results = data;
            if (queryOptions != null)
            {
                //http://blogs.msdn.com/b/alexj/archive/2013/05/10/parsing-odata-paths-select-and-expand-using-the-odatauriparser.aspx
                var vs = new ODataValidationSettings() { MaxAnyAllExpressionDepth = Int32.MaxValue, MaxExpansionDepth = Int32.MaxValue };
                queryOptions.Validate(vs);

                //do it manually instead of results = queryOptions.ApplyTo(results.AsQueryable())
                if (queryOptions.Skip != null) results = results.Skip(queryOptions.Skip.Value);
                if (queryOptions.Top != null) results = results.Take(queryOptions.Top.Value);
                if (queryOptions.OrderBy != null)
                    results = queryOptions.OrderBy.ApplyTo(results.AsQueryable()).AsEnumerable();
                //no support for filter or InlineCount
                if (queryOptions.Filter != null)
                {
                    var filteredRes = queryOptions.Filter.ApplyTo(results.AsQueryable(), new ODataQuerySettings());
                    
                    List<T> newRes = new List<T>();
                    foreach (T r in filteredRes)
                    {
                        newRes.Add(r);
                    }
                    results = newRes;
                }
                //if (queryOptions.InlineCount != null) { long? count = queryOptions.InlineCount.GetEntityCount(results.AsQueryable()); } 
            }
            if (se == null)
            {
                se = new SelectExpandResult(typeof(T).FullName);
            }

            var em = new ApiObjectEmitter(this);
            if (additionalProprieties != null)
            {
                em.AdditionalProperties.AddRange(additionalProprieties);
            }
            if (parent == null)
            {
                parent = new ObjectAndParent(this);
            }
            var res = em.BuildCollection(results, typeof(T), se, parent);
            
            return ResultWrap<IEnumerable<object>>.ResultData(res);
        }

        /// <summary>
        /// Map an object to an ApiObject based on the select/expand rules requested in the query string as well as in the object MetadataConfiguration or Attrributes configuration
        /// </summary>
        protected ResultWrap<object> GetApiObject<T>(T orig, ObjectAndParent parent = null, IEnumerable<AdditionalProperty> additionalProprieties = null) where T: class
        {
            if (CurrentODataQueryOptions == null)
            {
                ODataDataQueryOptionsHelper hlp = new ODataDataQueryOptionsHelper(this);
                var queryOptions = hlp.GetODataQueryOptions<T>();
                CurrentODataQueryOptions = queryOptions;
                CurrentSelectExpandResult = hlp.GetSelectExpandResult(queryOptions);
            }
            return GetApiObject(orig, GetCurrentODataQueryOptions<T>(), CurrentSelectExpandResult, parent, additionalProprieties);
        }

        /// <summary>
        /// Map an object to an ApiObject based on the select/expand rules parameter
        /// </summary>
        protected ResultWrap<object> GetApiObject<T>(T orig, ODataQueryOptions<T> queryOptions, SelectExpandResult se, ObjectAndParent parent = null, IEnumerable<AdditionalProperty> additionalProprieties = null)
        {
            if (queryOptions != null)
            {
                var vs = new ODataValidationSettings() { MaxAnyAllExpressionDepth = Int32.MaxValue, MaxExpansionDepth = Int32.MaxValue };
                queryOptions.Validate(vs);

                if (queryOptions.Filter != null)
                {
                    List<T> results = new List<T>() { orig }; 
                    var filteredRes = queryOptions.Filter.ApplyTo(results.AsQueryable(), new ODataQuerySettings());
                    List<T> newRes = new List<T>();
                    foreach (T r in filteredRes)
                    {
                        newRes.Add(r);
                    }
                    orig = newRes[0];
                }

            }
            if (se == null)
            {
                se = new SelectExpandResult(typeof(T).FullName);
            }

            ApiObjectEmitter em = new ApiObjectEmitter(this);
            if (additionalProprieties != null)
            {
                em.AdditionalProperties.AddRange(additionalProprieties);
            }
            if (parent == null)
            {
                parent = new ObjectAndParent(this); 
            }
            var res = em.BuildObject(orig, typeof(T), se, parent);
            
            return ResultWrap<object>.ResultData(res);
        }

       
        /// <summary>
        /// Function used to return a Custom Negociated Content of an ApiObject result (aka use proper custom formatters as necesary)<para></para>
        /// See SPNegotiator.GetNegotiator for more info<para></para>
        /// Ensures there are no unhandeled exceptions
        /// </summary>
        protected IHttpActionResult GetResult(Func<IResultWrap> getResult) 
        {
            try
            {
                var res = getResult();
                return res.GetActionResult(this);
            }
            catch (ODataException oerr)
            {
                //return BadRequest(oerr.Message);
                return ErrorMessage.GetNewErrorAcctionResult(new ApiParameterParsingException(oerr.Message, oerr), this);
            }
            catch (ApiException merr)
            {
                return ErrorMessage.GetNewErrorAcctionResult(merr, this);
            }
            catch (Exception err)
            {
                return ErrorMessage.GetNewErrorAcctionResult(new ApiUnexpectedException(err.Message, err), this);
            }
        }

        /// <summary>
        /// Validates patch and if ModelState.IsValid then calls doUpdate with patch.GetEntity()<para></para>
        /// Ensures there are no unhandeled exceptions
        /// </summary>
        protected IHttpActionResult ValidateAndUpdate<T>(Delta<T> patch, Func<T, IResultWrap> doUpdate) where T: class
        {
            if (patch == null)
            {
                return ErrorMessage.GetNewErrorAcctionResult(new ApiModelValidationException("The object received cannot be null"), this);
            }

            try
            {
                T obj = patch.GetEntity();
                Validate(obj);

                if (!ModelState.IsValid)
                {
                    return ErrorMessage.GetNewErrorAcctionResult(new ApiModelValidationException("The object received has some invalid data: " + ModelState), this);
                    //return BadRequest(ModelState);
                }

                var res = doUpdate(obj);
                return res.GetActionResult(this);
            }
            catch (ApiException mer)
            {
                return ErrorMessage.GetNewErrorAcctionResult(mer, this);
            }
            catch (Exception er)
            {
                return ErrorMessage.GetNewErrorAcctionResult(new ApiUnexpectedException(er.Message, er), this);
                //return InternalServerError(er);
            }
        }


        /// <summary>
        /// Validates obj and if ModelState.IsValid then calls doUpdate<para></para> 
        /// Ensures there are no unhandeled exceptions
        /// </summary>
        protected IHttpActionResult ValidateAndUpdate<T>(T obj, Func<IResultWrap> doUpdate) where T : class
        {
            try
            {
                Validate(obj);

                if (!ModelState.IsValid)
                {
                    return
                        ErrorMessage.GetNewErrorAcctionResult(
                            new ApiModelValidationException("The object received has some invalid data: " + ModelState),
                            this);
                    //return BadRequest(ModelState);
                }

                var res = doUpdate();
                return res.GetActionResult(this);
            }
            catch (ApiException mer)
            {
                return ErrorMessage.GetNewErrorAcctionResult(mer, this);
            }
            catch (Exception er)
            {
                return ErrorMessage.GetNewErrorAcctionResult(new ApiUnexpectedException(er.Message, er),this);
                //return InternalServerError(er);
            }
        }


    }
}