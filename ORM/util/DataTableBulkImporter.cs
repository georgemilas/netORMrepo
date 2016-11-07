using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using EM.DB;
using EM.Collections;
using System.IO;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using ORM;
using ORM.generator;
using ORM.exceptions;
using ORM.db_store;

namespace ORM.Util
{


    public class DataTableBulkImporter: IDisposable
    {
        public TableName databaseTable;
        protected Dictionary<string, int> fileMap;
        public TableColumnsWrap tableDef;

        public string errMessage = "";
        public string okMessage = "";

        #region validator callbacks

        public delegate object ValueValidator(string columnName, object parsedValue, ValueParser fileRow);
        public delegate object ColumnValueProvider(string columnName, ValueParser lineParserObject);

        private ValueValidator _columnValidateAndReturnDBValue;
        /// <summary>
        /// importer will parse values of columns acording to their data type but if extra business logic validation 
        /// must be aplied use this callback
        /// </summary>
        public ValueValidator columnValidateAndReturnDBValue
        {
            get { return _columnValidateAndReturnDBValue; }
            set { _columnValidateAndReturnDBValue = value; }
        }

        private ColumnValueProvider _columnValueCallback;
        /// <summary>
        /// columns for which in fileMap you specify index -1 will get a value via this callback
        /// </summary>
        public ColumnValueProvider columnValueCallback
        {
            get { return _columnValueCallback; }
            set { _columnValueCallback = value; }
        }

        #endregion validator callbacks

        
        

        private IDBWorker _db;
        public virtual IDBWorker db
        {
            get { return this._db; }
            set { this._db = value; }             
        }

        
        
        protected delegate void DataAdder(DataTable tb);
        protected void importBulkTemplate(DataAdder dataAdder)
        {
            errMessage = "";
            okMessage = "";

            bool newTran = false;
            if (this.db.currentTransaction == null)
            {
                DbTransaction t = this.db.startTransaction();
                this.db.currentTransaction = t;
                newTran = true;
            }

            try
            {                
                DataTable dt = new DataTable(databaseTable.table);
                foreach (string column in this.fileMap.Keys)
                {                    
                    dt.Columns.Add(column, this.tableDef.dataTableColumnType(column));
                }

                dataAdder(dt);

                applyBulkInsert(dt);

                //t.Rollback();
                if (newTran) { this.db.currentTransaction.Commit(); }
                okMessage = "Data Was succesfully Imported";                                
            }
            catch (InvalidDataException er)
            {
                errMessage = string.Format("Invalid data in the file: <br>{0}<br>Correct the data and try again", er.Message);
                try { if (newTran) { this.db.currentTransaction.Rollback(); } }   //?should already be rolled back?
                catch { }
                throw er;
            }
            catch (Exception er)
            {
                errMessage = string.Format("There was an error while trying to import the file: <br>{0}<br>{1}<br>Correct the error and try again", er.Message, er.StackTrace);
                try { if (newTran) { this.db.currentTransaction.Rollback(); } }
                catch { }
                throw er;
            }
            finally
            {
                if (newTran) { this.db.currentTransaction = null; }
            }

        }

        protected virtual void applyBulkInsert(DataTable dt)
        {
            SqlBulkCopy bc = new SqlBulkCopy((SqlConnection)this.db.connection, SqlBulkCopyOptions.Default, (SqlTransaction)this.db.currentTransaction);
            bc.BulkCopyTimeout = 600;
            bc.DestinationTableName = databaseTable.sqlFromName;
            foreach (string column in this.fileMap.Keys)
            {
                bc.ColumnMappings.Add(column, column);
            }
            bc.WriteToServer(dt);
        }

        

        



        //////////////////////////////////////////////////////////////////////////////
        ///////   Import Summary Report
        //////////////////////////////////////////////////////////////////////////////
        public virtual OrderedDictionary<string, OrderedDictionary<string, int>> computeSummary(string filePath, EList<string> columns) { return computeSummary(CSV.parse(File.OpenText(filePath)), columns); }
        public virtual OrderedDictionary<string, OrderedDictionary<string, int>> computeSummary(Stream stream, EList<string> columns) { return computeSummary(CSV.parse(new StreamReader(stream)), columns); }
        public virtual OrderedDictionary<string, OrderedDictionary<string, int>> computeSummary(EList<EList<string>> csv, EList<string> columns)
        {
            OrderedDictionary<string, OrderedDictionary<string, int>> report = new OrderedDictionary<string, OrderedDictionary<string, int>>();

            for (int i = 1; i < csv.Count; i++)     //first line is header
            {
                EList<string> line = csv[i];
                foreach (string k in columns)
                {
                    OrderedDictionary<string, int> v = report.setdefault(k, new OrderedDictionary<string, int>());
                    //try
                    //{
                    if (line.Count > fileMap[k])
                    {
                        string vk = line[fileMap[k]];
                        if (vk != null && vk.Trim().ToLower() != "")
                        {
                            vk = vk.Trim().ToLower();
                            v[vk] = v.get(vk, 0) + 1;
                        }
                    }
                    //}
                    //catch (ArgumentOutOfRangeException e)
                    //{
                    //    ;   //pass provider, customer, comunity probably null (toatl of less columns)
                    //}
                }
                
            }

            return report;

            
        }



        #region IDisposable Members

        public void Dispose()
        {
            if ( this.db != null ) { this.db.Dispose(); }
        }

        #endregion
    }
}
