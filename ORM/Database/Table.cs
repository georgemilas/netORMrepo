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
    public class Table<T> : EList<T>, ITable<T>
        where T : TableRow
    {
        protected string _name;
        protected GenericDatabase _db;
        
        public Table() : base() { }

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
