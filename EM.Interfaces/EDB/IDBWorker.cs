using System;
using System.Data;
using System.Data.Common;
using EM.Cache;
using EM.Logging;

namespace EM.DB
{
    public interface IDBWorker : IDisposable
    {
        string CONN_STR { get; set; }
        DbConnection connection { get; }
        int commandTimeOut { get; set; }
        //void restoreConnection();
        //void init();

        ICacheProvider<string> cache { get; set; }
        
        string contextCommand { get; set; }
        
        ILogger logger { get; set; }
        bool raise { get; set; }
        
        
        DbDataAdapter getDataAdapter();
        DbCommand getDataCommand(string sqlStr);
        
        DbTransaction startTransaction();
        DbTransaction startTransaction(IsolationLevel level);
        DbTransaction currentTransaction { get; set; }

        DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ);
        DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams);
        DataSet addToDataSet(ref DataSet ds, string tableName, string sqlQ, IDBParams dbparams, CommandType cmdType);
        
        int executeInsertSql(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType, ILastInsertIDProvider IdSql);
        int executeInsertSql(string sqlQ, bool raise, IDBParams dbparams, ILastInsertIDProvider IdSql);
        int executeInsertSql(string sqlQ, bool raise, ILastInsertIDProvider IdSql);
        int executeInsertSql(string sqlQ, IDBParams dbparams, ILastInsertIDProvider IdSql);
        int executeInsertSql(string sqlQ, IDBParams dbparams);
        int executeInsertSql(string sqlQ, ILastInsertIDProvider IdSql);
        int executeInsertSql(string sqlQ);

        bool executeQuery(string sqlQ);
        bool executeQuery(string sqlQ, IDBParams dbparams);
        bool executeQuery(string sqlQ, bool raise, IDBParams dbparams);
        bool executeQuery(string sqlQ, bool raise);
        bool executeQuery(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType);
        bool executeQuery(string sqlQ, IDBParams dbparams, CommandType cmdType);

        object executeScalar(string sqlQ, bool raise);
        object executeScalar(string sqlQ, IDBParams dbparams);
        object executeScalar(string sqlQ, bool raise, IDBParams dbparams);
        object executeScalar(string sqlQ, bool raise, IDBParams dbparams, CommandType cmdType);
        object executeScalar(string sqlQ);
        
        DataSet getDataSet(string sqlQ);
        DataSet getDataSet(string sqlQ, IDBParams dbparams);
        DataSet getDataSet(string sqlQ, IDBParams dbparams, CommandType cmdType);

        DataTable getDataTable(string tableName, string sqlQ);
        DataTable getDataTable(string tableName, string sqlQ, IDBParams dbparams);
        DataTable getDataTable(string tableName, string sqlQ, IDBParams dbparams, CommandType cmdType);
        DataTable getDataTable(string sqlQ);
        DataTable getDataTable(string sqlQ, IDBParams dbparams);
        DataTable getDataTable(string sqlQ, IDBParams dbparams, CommandType cmdType);
        
                
    }
}
