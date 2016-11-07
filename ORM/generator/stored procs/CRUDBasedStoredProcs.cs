using System;
using System.Collections.Generic;
using System.Text;
using ORM.db_store;

namespace ORM.generator
{
    public enum SPtype { deleteByPK, updateByPK, insert, countAll, loadByPK }

    public class CRUDBasedStoredProcs
    {
        public TableName crudTable;
        private TableName crudProc;
        public TableRowStoredProcedures procs;

        public string versionNumber { get; set; }
        private string getVersionString() { return versionNumber != null ? "_v" + versionNumber : ""; }

        /// <summary>
        /// if versionNumber != null then it will append stored proc names with "_vVersionNumber"
        /// </summary>
        public CRUDBasedStoredProcs(TableName table, string versionNumber)
        {
            this.crudTable = table;
            this.crudProc = new TableName(table.catalog, "CRUD", "");
            this.crudProc.context = table.context;
            this.procs = new TableRowStoredProcedures(table, "CRUD");
            this.versionNumber = versionNumber;
            this.procs.loadByPK.table = string.Format("usp_{0}_{1}_loadByPK{2}", ORMContext.fixName(table.schema), ORMContext.fixName(table.table), getVersionString());
            this.procs.updateByPK.table = string.Format("usp_{0}_{1}_updateByPK{2}", ORMContext.fixName(table.schema), ORMContext.fixName(table.table), getVersionString());
            this.procs.deleteByPK.table = string.Format("usp_{0}_{1}_deleteByPK{2}", ORMContext.fixName(table.schema), ORMContext.fixName(table.table), getVersionString());
            this.procs.insert.table = string.Format("usp_{0}_{1}_insert{2}", ORMContext.fixName(table.schema), ORMContext.fixName(table.table), getVersionString());
            this.procs.countAll.table = string.Format("usp_{0}_{1}_countAll{2}", ORMContext.fixName(table.schema), ORMContext.fixName(table.table), getVersionString());

        }
        public TableName getProc(SPtype sptype)
        {
            switch (sptype)
            {
                case SPtype.countAll: return procs.countAll; 
                case SPtype.deleteByPK: return procs.deleteByPK; 
                case SPtype.insert:return procs.insert; 
                case SPtype.loadByPK: return procs.loadByPK; 
                case SPtype.updateByPK: return procs.updateByPK; 
            }
            return null;
        }

        /// <summary>
        /// select procs may return more then 1 row 
        /// </summary>
        public TableName getRelationSelectProc(List<string> fields)
        {
            TableName t = this.getRelationLoadProc(this.crudTable, fields);
            t.table = t.table.Replace("loadBy", "selectBy");
            if (fields.Count == 0) { t.table = t.table.Replace("selectBy", "selectAll"); }
            return t;
        }

        /// <summary>
        /// load procs should return 1 row 
        /// </summary>
        public TableName getRelationLoadProc(TableName table, List<string> fields)
        {
            string spname = string.Format("usp_{0}_{1}_loadBy", ORMContext.fixName(table.schema), ORMContext.fixName(table.table));
            spname += ORMContext.GetMethodName_ParamsPart(fields);
            spname += getVersionString();
            //for (int i = 0; i < fields.Count; i++)
            //{
            //    spname += ORMContext.fixName(fields[i]);
            //}
            TableName t = new TableName(crudTable.catalog, "CRUD", spname);
            t.context = crudTable.context;
            return t;
        }

        public TableName getRelationDeleteProc(TableName table, List<string> fields)
        {
            string spname = string.Format("usp_{0}_{1}_deleteBy", ORMContext.fixName(table.schema), ORMContext.fixName(table.table));
            spname += ORMContext.GetMethodName_ParamsPart(fields);
            spname += getVersionString();
            TableName t = new TableName(crudTable.catalog, "CRUD", spname);
            t.context = crudTable.context;
            return t;
        }

    }
}
