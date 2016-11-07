using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using EM.DB;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using ORM.db_store.persitence;

namespace ORM.db_store
{
    /// <summary>
    /// This Stored Procedure is one that should return a DataTable 
    /// </summary>
    public class StoredProcDataSetDef: StoredProcDef
    {
        public StoredProcDataSetDef() : base() { }
        public StoredProcDataSetDef(TableName name, GenericDatabase db): base(name, db) { }

        public override void init(TableName name, GenericDatabase db)
        {
            base.init(name, db);
            this.getSchemaTableByRunningTheProcedure = true;
        }

        private List<TableColumnsInfo> _resultsetColumnsDefinition;
        /// <summary>
        /// - inspects the columns schema definition using a reader.GetSchemaTable
        /// - if this.getSchemaTableByRunningTheProcedure is true then will run the proc instead of doing reader.GetSchemaTable
        /// - uses getParameterValue to compute values of parameters
        /// </summary>
        public new List<TableColumnsInfo> resultsetColumnsDefinition
        {
            get
            {
                if (_resultsetColumnsDefinition == null)
                {
                    #region compute columns definition
                    DataSet ds = new DataSet();

                    try
                    {
                        DBParams p = new DBParams();
                        foreach (StoredProcParam spp in this.parameters)
                        {
                            if (!spp.hasDefault)
                            {
                                p.Add(spp.name, this.getParameterValue(spp, true));  //use null values where posible
                            }
                        }
                        ds = this.db.db.getDataSet(this.name.sqlFromName, p, CommandType.StoredProcedure);
                        
                    }
                    catch
                    {   
                        //runing with null values failed, so trying dummy default type values
                        DBParams p = new DBParams();
                        foreach (StoredProcParam spp in this.parameters)
                        {
                            if (!spp.hasDefault)
                            {
                                p.Add(spp.name, this.getParameterValue(spp, false));
                            }
                        }
                        ds = this.db.db.getDataSet(this.name.sqlFromName, p, CommandType.StoredProcedure);                        
                    }

                    _resultsetColumnsDefinition = new List<TableColumnsInfo>();
                    foreach (DataTable tb in ds.Tables)
                    {
                        TableColumnsInfo tci = new TableColumnsInfo(tb.Columns, this.name);
                        _resultsetColumnsDefinition.Add(tci);
                    }
                    #endregion compute columns definition
                    
                }

                return _resultsetColumnsDefinition;
            }
        }

        private DataSet _resultsetDataSet;
        /// <summary>
        /// - runs the proc using getParameterValue to compute parameter values and returns the resulting DataSet
        /// </summary>
        public DataSet resultsetDataSet
        {
            get
            {
                if (_resultsetDataSet == null)
                {
                    #region compute resulset data table
                    try
                    {
                        DBParams p = new DBParams();
                        foreach (StoredProcParam spp in this.parameters)
                        {
                            if (!spp.hasDefault)
                            {
                                p.Add(spp.name, this.getParameterValue(spp, true));
                            }
                        }
                        _resultsetDataSet = this.db.db.getDataSet(this.name.sqlFromName, p, CommandType.StoredProcedure);
                    }
                    catch
                    {
                        DBParams p = new DBParams();
                        foreach (StoredProcParam spp in this.parameters)
                        {
                            if (!spp.hasDefault)
                            {
                                p.Add(spp.name, this.getParameterValue(spp, false));
                            }
                        }
                        _resultsetDataSet = this.db.db.getDataSet(this.name.sqlFromName, p, CommandType.StoredProcedure);
                    }
                    #endregion compute resulset data table
                }
                return _resultsetDataSet;
            }
        }


    }


    

}


