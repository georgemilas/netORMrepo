using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;

namespace EM.Cache
{
    public class CacheValue<TK> : ICacheValue
    { 
        private object _value;
        private ICacheDependency _dep;

        private DateTime createdDateTime;
        
        public CacheValue(object value, TimeSpan timeOutPeriod): this(value, timeOutPeriod, null) { }
        /// <summary>
        /// use a CacheDependency so that value is expired via either time_out_period or dependency
        /// </summary>
        public CacheValue(object value, TimeSpan timeOutPeriod, ICacheDependency dep)
        {
            this.value = value;
            this.dep = dep;
            this.createdDateTime = DateTime.Now;
            this.time_out_period = timeOutPeriod;
        }

        /// <summary>
        /// bring in ICacheProvider to keep a dinamic time_out_period depending on the main cache
        /// </summary>
        public CacheValue(object value): this(value, (ICacheDependency)null) { }
        public CacheValue(object value, CacheDependency dep): this(value, new CacheDependencyWrapper(dep)) {}
        /// <summary>
        /// - bring in ICacheProvider to keep a dinamic time_out_period depending on the main cache
        /// - use a CacheDependency so that value is expired via either time_out_period or dependency
        /// </summary>
        public CacheValue(object value, ICacheDependency dep)
        {
            this.value = value;
            this.dep = dep;
            this.createdDateTime = DateTime.Now;
            this.time_out_period = TimeSpan.Zero;
        }

        public object value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        public ICacheDependency dep
        {
            get { return this._dep; }
            set { this._dep = value; }
        }

        /// <summary>
        /// if the value is TimeSpan.Zero (the default value) then there is no timeout 
        /// </summary>
        public TimeSpan time_out_period { get; set; }
    
        public bool timeoutExpired
        {
            get
            {
                if (this.time_out_period == TimeSpan.Zero)  //never expires
                {
                    return false;   
                }
                return DateTime.Now.Subtract(this.createdDateTime) >= this.time_out_period;
            }            
        }

        public bool expired
        {
            get 
            {
                if (dep != null)
                {
                    return dep.HasChanged || this.timeoutExpired;
                }
                else
                {
                    return this.timeoutExpired;
                }
            }
        }
    }


    public class CacheDependencyWrapper : ICacheDependency
    {
        public CacheDependency InternalData { get; set; }

        public CacheDependencyWrapper(CacheDependency dep)
        {
            this.InternalData = dep;
        }
        public bool HasChanged
        {
            get { return InternalData.HasChanged; }
        }
        public bool SupportsInvalidation { get { return false; } }
        public void InvalidateCache()
        {
            throw new NotSupportedException("Manualy invalidating System.Web.Caching.CacheDependency is not supported");
        }
        public CacheDependency GetSystemWebCacheDependency()
        {
            return InternalData;
        }
    }

}
