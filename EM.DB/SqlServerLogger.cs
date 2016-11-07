using System;
using System.Collections.Generic;
using System.Text;
using EM.Logging;

namespace EM.DB
{
    public class SqlServerLogger : SqlServerDBWorker, ILoggingDB
    {
        public bool executeQuery(string sql, Dictionary<string, object> parameters)
        {
            DBParams p = new DBParams();
            foreach(string k in parameters.Keys)
            {
                p.Add(new DBParam(k, parameters[k]));
            }
            return this.executeQuery(sql, p);
        }
    }
}
