using System;
using System.Web.Caching;

namespace EM.Cache
{
    public interface ICacheValue
    {
        bool expired { get; }
        TimeSpan time_out_period { get; set; }
        bool timeoutExpired { get; }
        object value { get; set; }
        ICacheDependency dep { get; set; }
    }

    public interface ICacheDependency
    {
        bool HasChanged { get; }
        bool SupportsInvalidation { get; }
        void InvalidateCache();
        CacheDependency GetSystemWebCacheDependency();
    }

}
