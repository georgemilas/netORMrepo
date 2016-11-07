using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using ORM.db_store;
using EM.DB;
using System.Data;

namespace ORM
{
    public class StoredProcTableRow: QueryStatementTableRow
    {
        public StoredProcDef spDef;

        private StoredProcTableRow(StoredProcTableRow other)
            : base(other.context)
        {
            this.spDef = other.spDef;
            //this.defTable = other.defTable;
            this.setFields();
            this.dbObjectName = other.dbObjectName;
            this.pk = other.pk;
            this.fk = other.fk;
            this._isReadOnly = true;

            this.init();
            //this.initPresentation();
        }

        public StoredProcTableRow(ORMContext context, StoredProcDef sp)
            : base(context)
        {
            sp.name.context = this.context;
            this.spDef = sp;
            //this.defTable = sp.resultsetDataTable;
            
            this.setFields();
            this._isReadOnly = true;

            //table name
            this.setTableName(sp.name);

            //inherit support
            this.init();
            //this.initPresentation();
        }



        public override TableRow getInstance()
        {
            return new StoredProcTableRow(this);
        }

        /// <summary>
        /// SQLStatements are ignored in the case of stored procedures
        /// </summary>
        public override Table<T> select<T>(SQLStatement atr, DBParams param)
        {
            return this.select<T>(param);                        
        }

        public virtual Table<T> select<T>(DBParams param) where T : TableRow 
        {
            DataTable tb = db.selectStoredProcedure(this.dbObjectName, param);     //ABSTRACT STORAGE         
            return this.getInstancesFromDataTable<T>(tb);
        }

        
    }
}
