using System;
using System.Collections.Generic;
using System.Text;

using System.Data.Common;
using System.Web.Caching;


namespace EM.Cache
{
    public interface ICacheProvider<TK>
    {
        /// <summary>
        /// if set to TimeSpan.Zero there should be no time_out_period so the item would stay in the cache forever until manually removed
        /// </summary>
        TimeSpan time_out_period { get; set; }
        
        bool has_key(TK key);

        /// <summary>
        /// get the value out of the cache 
        /// </summary>
        object get(TK key);

        /// <summary>
        /// get the value as the actual type (ICacheValue) stored in the cache 
        /// </summary>
        ICacheValue getWrappedCacheValue(TK key);
        
        /// <summary>
        /// uses cache instance time_out_period expiration 
        /// </summary>
        void set(TK key, object value);

        /// <summary>
        /// - to use diferent time_out_period for diferent keys or to use dinamic expiration time 
        ///    via a CacheDependency provider, wrap the value in a CacheValue Instance.
        /// - Notice that the get function should return the real value not the CacheValue instance
        /// </summary>
        void set(TK key, ICacheValue value);

        /// <summary>
        /// instead of removing the item from cache you should probably use expiration 
        /// </summary>
        void del(TK key);
        
    }
}
