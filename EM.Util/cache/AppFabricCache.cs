//using System;
//using Microsoft.ApplicationServer.Caching;
//using System.Collections.Generic;


//namespace EM.Cache
//{
//    /// <summary>
//    /// a cache store using an an App fabric instalation as the real cache engine
//    /// </summary>
//    public class AppFabricCache : ICacheProvider<string>
//    {
//        public TimeSpan time_out_period { get; set; }
//        private DataCache cache { get; set; }

//        //new AppFabricCache("myMachine", 22233, "default") { time_out_period = TimeSpan.Zero }  
//        public AppFabricCache(string server, int port, string cacheName)
//        {
//            this.time_out_period = new TimeSpan(0, 15, 0);  //15 minutes

//            List<DataCacheServerEndpoint> servers = new List<DataCacheServerEndpoint>(1);  //cache hosts
//            servers.Add(new DataCacheServerEndpoint(server, port));
//            DataCacheFactoryConfiguration configuration = new DataCacheFactoryConfiguration();
//            configuration.Servers = servers;
//            configuration.LocalCacheProperties = new DataCacheLocalCacheProperties();  //local cache disabled
//            DataCacheClientLogManager.ChangeLogLevel(System.Diagnostics.TraceLevel.Off);
//            var _factory = new DataCacheFactory(configuration);
//            this.cache = _factory.GetCache(cacheName);
//        }
//        public AppFabricCache(DataCache appFabricCache)
//        {
//            this.time_out_period = new TimeSpan(0, 15, 0);  //15 minutes
//            this.cache = appFabricCache;
//        }

//        public bool has_key(string key)
//        {
//            return this.cache.Get(key) != null;
//        }

//        public object get(string key)
//        {
//            return this.cache.Get(key);
//        }

//        public void set(string key, object value)
//        {
//            CacheValue<string> cv = new CacheValue<string>(value, this);
//            this.set(key, cv);
//        }

//        public void set(string key, ICacheValue value)
//        {
//            if (value.time_out_period == TimeSpan.Zero)
//            {
//                this.cache.Add(key, value.value);
//            }
//            else
//            {
//                this.cache.Add(key, value.value, value.time_out_period);
//            }
//        }

//        public void del(string key)
//        {
//            this.cache.Remove(key);
//        }

//    }

//}


