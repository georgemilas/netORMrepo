using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Web.Caching;
using System.Data.SqlClient;
using System.Data.Common;

namespace EM.Cache
{
    /// <summary>
    /// a cache store using an instance of System.Web.Caching.Cache as the real cache engine
    /// </summary>
    public class WebCache : ICacheProvider<string>
    {
        
        protected TimeSpan _time_out_period;
        protected System.Web.Caching.Cache webCache;

        public WebCache() : this(new System.Web.Caching.Cache()) { }
        public WebCache(System.Web.Caching.Cache webCache)
        {
            this.time_out_period = TimeSpan.FromMinutes(15);
            this.webCache = webCache;
        }

        public TimeSpan time_out_period
        {
            get { return this._time_out_period; }
            set { this._time_out_period = value; }
        }

        public bool has_key(string key)
        {
            var val = this.webCache[key];
            if (val != null)  //it exists
            {
                if (((ICacheValue)val).expired)
                {
                    this.webCache.Remove(key);
                    return false;
                }
                return true;
            }
            return false;
        }

        public object get(string key)
        {
            ICacheValue cv = getWrappedCacheValue(key);
            return cv != null ? cv.value : null;            
        }

        public ICacheValue getWrappedCacheValue(string key)
        {
            ICacheValue cv = (ICacheValue)this.webCache[key];
            if (cv != null && cv.expired)
            {
                if (cv.dep != null && cv.dep.SupportsInvalidation)
                {
                    cv.dep.InvalidateCache();
                }
                this.webCache.Remove(key);
                return null;
            }
            return cv;            
        }

        public void set(string key, object value)
        {
            CacheValue<string> cv = new CacheValue<string>(value, this.time_out_period);
            this.set(key, cv);
        }

        public void set(string key, ICacheValue value)
        {
            if (value.time_out_period == TimeSpan.Zero)
            {
                this.webCache.Insert(key, value, value.dep != null ? value.dep.GetSystemWebCacheDependency() : null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
            {
                this.webCache.Insert(key, value, value.dep != null ? value.dep.GetSystemWebCacheDependency() : null, DateTime.Now.Add(value.time_out_period), System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

        public void del(string key)
        {
            this.webCache.Remove(key);
        }

    }

}


