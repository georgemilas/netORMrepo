using System;
using System.Collections.Generic;
using System.Text;
using ORM.db_store;

namespace ORM
{
    [Serializable]
    /// <summary>
    /// name of stoted procedures that are needed by TableRowStoredProcBased to perform database access
    /// </summary>
    public class TableRowStoredProcedures
    {
        private string spSchema;

        public TableRowStoredProcedures() { }
        public TableRowStoredProcedures(TableName name, string procsSchema) 
        {
            this.name = name;
            this.spSchema = procsSchema;
        }

        private TableName _name;
        public virtual TableName name
        {
            get { return _name; }
            set { _name = value; }
        }

        private TableName _countAll;
        public virtual TableName countAll
        {
            get 
            {
                if (_countAll == null)
                {
                    _countAll = new TableName(name.catalog, spSchema, "usp_" + this.name.table + "_countAll");
                    _countAll.context = name.context;
                }
                return _countAll; 
            }
            set { _countAll = value; }
        }

        private TableName _loadByPK;
        public TableName loadByPK
        {
            get 
            {
                if (_loadByPK == null)
                {
                    _loadByPK = new TableName(name.catalog, spSchema, "usp_" + this.name.table + "_loadByPK");
                    _loadByPK.context = name.context;
                }
                return _loadByPK; 
            }
            set { _loadByPK = value; }
        }
        
        private TableName _insert;
        public TableName insert
        {
            get 
            {
                if (_insert == null)
                {
                    _insert = new TableName(name.catalog, spSchema, "usp_" + this.name.table + "_insert");
                    _insert.context = name.context;
                }
                return _insert; 
            }
            set { _insert = value; }
        }
        
        private TableName _updateByPK;
        public TableName updateByPK
        {
            get 
            {
                if (_updateByPK == null)
                {
                    _updateByPK = new TableName(name.catalog, spSchema, "usp_" + this.name.table + "_updateByPK");
                    _updateByPK.context = name.context;
                }
                return _updateByPK; 
            }
            set { _updateByPK = value; }
        }

        private TableName _deleteByPK;
        public TableName deleteByPK
        {
            get 
            {
                if (_deleteByPK == null)
                {
                    _deleteByPK = new TableName(name.catalog, spSchema, "usp_" + this.name.table + "_deleteByPK");
                    _deleteByPK.context = name.context;
                }
                return _deleteByPK; 
            }
            set { _deleteByPK = value; }
        }

    }

}
