using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using System.Data;
using System.Data.Common;
using ORM.exceptions;
using ORM.render;
using ORM.Util;
using ORM.db_store.persitence;


namespace ORM
{            
    [Serializable] 
    public class TablePersist<T> : EList<T>, ITable<T>
        where T : TableRowPersist
    {
        protected string _name;
        protected GenericDatabase _db;

        public TablePersist() : base() { }


        public ESet<ValidationException> validationExceptions { get; set; }

        /// <summary>
        /// a quick way to pass something into a datagrid
        /// </summary>
        public DataTable adoDataTable { get; set; }


        /// <summary>
        ///  - by default if this[0].dbObjectName.table unless you set it yourself
        ///  - if this.Count==0 returns null
        /// </summary>
        public string name
        {
            get 
            {
                if (this._name != null) return this._name;
                if (this.Count > 0)
                {
                    return this[0].dbObjectName.table;
                }
                
                return null;
            }
            set { this._name = value; }
        }

        /// <summary>
        /// - does a save for each GenericDataRow in the table 
        /// - if one row fails to save, then all of them fail 
        /// - if you want bulk insert use ORM.Util.DataTableBulkImporter
        /// </summary>
        /// <returns>success/error -> check validationExceptions for validation errors</returns>
        public virtual bool save()
        {
            
            //add all validation errors here
            this.validationExceptions = new ESet<ValidationException>();

            //GenericDatabase db = ORMContext.instance.db;
            bool raiseSetting = this.db.db.raise;
            this.db.db.raise = true;

            bool ok = true;
            bool myTrans = false;
            if (this.db.db.currentTransaction == null)
            {
                myTrans = true;
                DbTransaction trans = this.db.db.startTransaction();
                this.db.db.currentTransaction = trans;
            }

            foreach (TableRowPersist tr in this)
            {
                if (!ok) break;

                try
                {
                    tr.save();
                }
                catch (Exception e)
                {
                    try { this.db.db.currentTransaction.Rollback(); }
                    catch { }

                    if (myTrans)
                    {
                        this.db.db.currentTransaction = null;
                    }
                    throw e;
                }
                finally
                {
                    if (tr.validationExceptions.Count > 0)
                    {
                        this.validationExceptions.AddRange(tr.validationExceptions);
                        ok = false;
                    }
                }
            }
            if (ok && myTrans)
            {
                this.db.db.currentTransaction.Commit();
            }
            if (myTrans)
            {
                this.db.db.currentTransaction = null;
            }
            return ok;

        }

        public GenericDatabase db
        {
            get {
                if (_db == null) { throw new NullReferenceException("Database was not initialized"); }
                return _db; 
            }
            set { this._db = value; }
        }
        

        
    }


}
