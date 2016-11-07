using System;
using System.Collections.Generic;
using System.Text;

//using System.Data.SqlClient;
using System.Data.Common;
using EM.Collections;
using System.Web.Caching;

namespace EM.Cache
{
    public class DictCache<TK>: ICacheProvider<TK>
    {
        private Queue<TK> oldest;        //FIFO
        private int oldest_capacity;
        private EDictionary<TK, ICacheValue> cache;
        private TimeSpan _time_out_period;
        private int _max_capacity;

        public DictCache()
        {
            this.cache = new EDictionary<TK, ICacheValue>();
            this.time_out_period = TimeSpan.FromMinutes(15);
            this.oldest = new Queue<TK>();               
        }

        public TimeSpan time_out_period
        {
            get { return this._time_out_period; }
            set { this._time_out_period = value; }
        }

        /// <summary>
        /// if set to 0 then is 'infinite'  (default)
        /// </summary>
        public int max_capacity
        {
            get { return this._max_capacity; }
            set 
            { 
                this._max_capacity = value;
                this.oldest_capacity = value / 4;
            }
        }

        private object _lock = new object();
        private void removeAllOldestEntries()
        {
            lock (_lock)
            {
                foreach (TK k in oldest)
                {
                    this.cache.Remove(k);
                }
                oldest.Clear();
            }
        }

        public bool has_key(TK key) 
        { 
            bool exists = this.cache.ContainsKey(key);
            if (exists)
            {
                var val = this.cache[key];
                if (val == null)
                {
                    return false;
                }
                if (val.expired)
                {
                    this.cache.Remove(key);
                    return false;
                }
                return true;
            }
            return false;
        }

        public object get(TK key) 
        {
            ICacheValue cv = getWrappedCacheValue(key);
            return cv != null ? cv.value : null;                
        }

        public ICacheValue getWrappedCacheValue(TK key)
        {
            ICacheValue cv = (ICacheValue)this.cache[key];
            if (cv != null && cv.expired)
            {
                this.cache.Remove(key);
                return null;
            }
            return cv;
        }

        public void set(TK key, object value) 
        {
            this.set(key, new CacheValue<TK>(value, this.time_out_period, (ICacheDependency)null));            
        }

        public void set(TK key, ICacheValue value) 
        {
            if (max_capacity > 0 && this.cache.Keys.Count == max_capacity)
            {
                removeAllOldestEntries();
            }

            this.cache[key] = value;

            if (max_capacity > 0)
            {
                if (oldest.Count == oldest_capacity)
                {
                    oldest.Dequeue();
                }
                oldest.Enqueue(key);
            }
        }

        public void del(TK key)
        {
            this.cache.Remove(key);
        }
        
    }
}
