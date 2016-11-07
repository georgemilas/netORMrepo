using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using EM.DB;

namespace EM.Logging
{
    public interface ILoggingDB
    {
        bool executeQuery(string sql);
        bool executeQuery(string sql, Dictionary<string, object> parameters);
        DataTable getDataTable(string sql);
    }
}
