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
using System.Collections.Generic;
using EM.Collections;

namespace EM.Cache
{
    //see this for how to make SQLDependency work: 
    //      http://davidhayden.com/blog/dave/archive/2006/04/29/2929.aspx
    ///     http://msdn2.microsoft.com/en-us/library/ms181122.aspx
    /// <summary>
    /// a wrapper over a cache store (ICacheProvider) with utility for database dependency cache mechanism
    /// </summary>
    public class DepCache : ICacheProvider<string>
    {
        public string connStr;
        public ICacheProvider<string> provider;

        public DepCache(ICacheProvider<string> provider, string connStr)
        {
            this.connStr = connStr;
            this.provider = provider;
        }

        private static EDictionary<string, bool> connectionHasAllTablesForNotifications = new EDictionary<string,bool>();
        public static EDictionary<string, EList<string>> connectionWithTablesNotificationsList = new EDictionary<string, EList<string>>();
        /// <summary>
        /// if all calls to DepCache.addTablesNotifications were succesfull then this is true
        /// </summary>
        public static bool hasAllTablesForNotifications(string connStr)
        {
            return DepCache.connectionHasAllTablesForNotifications.get(connStr, false);
        }

        /// <summary>
        /// - sets all given tables for notifications and sets DepCache.hasAllTablesForNotifications to true
        /// - if one table fails then DepCache.hasAllTablesForNotifications is false
        /// </summary>
        public static void addTablesNotifications(string connStr, IEnumerable<string> tables)
        {
            try
            {
                SqlCacheDependencyAdmin.EnableNotifications(connStr);                
            }
            catch (Exception)
            {
                //UnauthorizedAccessException, DatabaseNotEnabledForNotificationException 
                DepCache.connectionHasAllTablesForNotifications[connStr] = false;
                return;
            }

            foreach(string table in tables) 
            {
                try
                {
                    SqlCacheDependencyAdmin.EnableTableForNotifications(connStr, table);
                    DepCache.connectionHasAllTablesForNotifications[connStr] = true;
                    DepCache.connectionWithTablesNotificationsList.setdefault(connStr, new EList<string>()).Add(table);
                }
                catch (Exception)
                {
                    //UnauthorizedAccessException, TableNotEnabledForNotificationException, 
                    //Invalid table name etc.
                    DepCache.connectionHasAllTablesForNotifications[connStr] = false;
                    return;
                }
            }
            
        }

        public static void removeTablesNotifications(string connStr)
        {
            DepCache.connectionHasAllTablesForNotifications[connStr] = false;
            EList<string> tables = DepCache.connectionWithTablesNotificationsList.get(connStr, new EList<string>());

            foreach (string table in tables)
            {
                try
                {
                    SqlCacheDependencyAdmin.DisableTableForNotifications(connStr, table);
                    tables.Remove(table);
                }
                catch (Exception)
                {
                    //UnauthorizedAccessException, TableNotEnabledForNotificationException                    
                }
            }
            if (tables.Count == 0)
            {
                DepCache.connectionWithTablesNotificationsList.Remove(connStr);
            }
        }

        /// <summary>
        /// - returns null in case of errors or if no tables were set for notifications
        /// - it assumes you already called DepCache.SetTablesForNotification of all required tables
        ///   but if you set some and not others it may not see the changes
        /// - if hasAllTablesForNotifications if false then this will not succeed
        /// </summary>
        public CacheDependency getCacheDedendency(SqlCommand cmd)
        {
            return DepCache.getCacheDedendency(this.connStr, cmd);
        }

        /// <summary>
        /// - returns null in case of errors or if no tables were set for notifications
        /// - it assumes you already called DepCache.SetTablesForNotification of all required tables
        ///   but if you set some and not others it may not see the changes
        /// - if hasAllTablesForNotifications if false then this will not succeed
        /// </summary>
        public static CacheDependency getCacheDedendency(string connStr, SqlCommand cmd)
        {
            if ( !DepCache.hasAllTablesForNotifications(connStr) )
            {
                return null;
            }

            try
            {
                return new SqlCacheDependency((SqlCommand)cmd);
            }
            catch (Exception)
            {
                return null;                
            }            
        }


        /// <summary>
        /// returns null in case of errors
        /// </summary>
        public CacheDependency getCacheDedendency(string dbName, string tableName)
        {
            return DepCache.getCacheDedendency(this.connStr, dbName, tableName);
        }

        /// <summary>
        /// returns null in case of errors
        /// </summary>
        public static CacheDependency getCacheDedendency(string connStr, string dbName, string tableName)
        {
            try
            {
                SqlCacheDependencyAdmin.EnableNotifications(connStr);
                SqlCacheDependencyAdmin.EnableTableForNotifications(connStr, tableName);
            }
            catch (Exception)
            {
                return null;
            }

            try
            {
                return new SqlCacheDependency(dbName, tableName);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public void set(string key, object value, string dbName, string tableName)
        {
            CacheValue<string> cv = new CacheValue<string>(value, this.time_out_period, new CacheDependencyWrapper(this.getCacheDedendency(dbName, tableName)));
            provider.set(key, cv);
        }
        
        public void set(string key, object value, SqlCommand cmd)
        {
            CacheValue<string> cv = new CacheValue<string>(value, this.time_out_period, new CacheDependencyWrapper(this.getCacheDedendency(cmd)));
            provider.set(key, cv);
        }

        #region ICacheProvider Members

        public TimeSpan time_out_period
        {
            get { return provider.time_out_period; }
            set { provider.time_out_period = value; }
        }

        public bool has_key(string key)
        {
            return provider.has_key(key);
        }

        public object get(string key)
        {
            return provider.get(key);
        }

        public ICacheValue getWrappedCacheValue(string key)
        {
            return provider.getWrappedCacheValue(key);
        }

        public void set(string key, object value)
        {
            provider.set(key, value);
        }

        public void set(string key, ICacheValue value)
        {
            provider.set(key, value);
        }

        public void del(string key)
        {
            provider.del(key);
        }

        #endregion
    }

}


