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
    public class StoredProcDef
    {
        public GenericDatabase db;
        public TableName name;
        public EList<StoredProcParam> parameters;

        public bool getSchemaTableByRunningTheProcedure { get; set; }

        private DBParams _defaultParameters;
        /// <summary>
        /// - Set of parameters to use when computing ResultSet columns definition or DataTable
        /// - If not given then computing ResultSet columns definition or DataTable will use either null or dummy values by type of parameter
        /// </summary>
        public DBParams defaultParameters
        {
            get { return _defaultParameters; }
            set 
            { 
                _defaultParameters = value;
                if (value != null) { this.getSchemaTableByRunningTheProcedure = true; }
                else { this.getSchemaTableByRunningTheProcedure = false; }
            }
        }

        public StoredProcDef() { }
        public StoredProcDef(TableName name, GenericDatabase db)
        {
            init(name, db);
        }

        public virtual void init(TableName name, GenericDatabase db)
        {
            this.name = name;
            this.parameters = new EList<StoredProcParam>();
            this.db = db;
            this.getSchemaTableByRunningTheProcedure = false;
        }

        /// <summary>
        /// return defaultParameters if given or dummy values otherwise
        /// </summary>
        public object getParameterValue(StoredProcParam spp, bool useNull)
        {
            if (this.defaultParameters != null)
            {
                foreach (DBParam p in this.defaultParameters)
                {
                    if (spp.name == p.param.ParameterName)
                    {
                        return p.param.Value;
                    }
                }
            }
            
            return getParameterDummyValue(spp, useNull);
            
        }

        public object getParameterDummyValue(StoredProcParam spp, bool useNull)
        {
            
            if (useNull && spp.isNullable)
            {
                return System.DBNull.Value;  // null;
            }

            return this.db.getDbTypeDummyValue(spp.dbType);            
        }


        private TableColumnsInfo _resultsetColumnsDefinition;
        /// <summary>
        /// - inspects the columns schema definition using a reader.GetSchemaTable
        /// - if this.getSchemaTableByRunningTheProcedure is true then will run the proc instead of doing reader.GetSchemaTable
        /// - uses getParameterValue to compute values of parameters
        /// </summary>
        public virtual TableColumnsInfo resultsetColumnsDefinition
        {
            get
            {
                if (_resultsetColumnsDefinition == null)
                {
                    #region compute columns definition
                    DataTable tb = new DataTable();
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
                        if (this.getSchemaTableByRunningTheProcedure)
                        {
                            tb = this.db.db.getDataTable(this.name.sqlFromName, p, CommandType.StoredProcedure);
                        }
                        else
                        {
                            tb = this.db.db.getSchemaTable(this.name.sqlFromName, p, CommandType.StoredProcedure);
                            //if (tb == null) throw new Exception();
                        }
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
                        if (this.getSchemaTableByRunningTheProcedure)
                        {
                            tb = this.db.db.getDataTable(this.name.sqlFromName, p, CommandType.StoredProcedure);
                        }
                        else
                        {
                            tb = this.db.db.getSchemaTable(this.name.sqlFromName, p, CommandType.StoredProcedure);
                        }
                    }

                    TableColumnsInfo tci;
                    if (this.getSchemaTableByRunningTheProcedure)
                    {
                        tci = new TableColumnsInfo(tb.Columns, this.name);
                    }
                    else
                    {
                        tci = new TableColumnsInfo(this.name);
                        foreach (DataRow r in tb.Rows)
                        {
                            ColumnAttributes ca = new ColumnAttributes();
                            ca.allowNull = (bool)r["AllowDBNull"];
                            ca.autoIncrement = (bool)r["IsAutoIncrement"];
                            ca.dotNetType = ((Type)r["DataType"]).Name;
                            ca.dbType = r["DataTypeName"].ToString();       //24
                            //ca.hasDefaultConstraint
                            //ca.isComputed
                            ca.maxLength = int.Parse(r["ColumnSize"].ToString());
                            tci[r["ColumnName"].ToString()] = ca;
                        }
                    }
                    
                    #endregion compute columns definition

                    _resultsetColumnsDefinition = tci;
                }

                return _resultsetColumnsDefinition;
            }
        }

        private DataTable _resultsetDataTable;
        /// <summary>
        /// - runs the proc using getParameterValue to compute parameter values and returns the resulting DataTable
        /// </summary>
        public DataTable resultsetDataTable
        {
            get
            {
                if (_resultsetDataTable == null)
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
                        _resultsetDataTable = this.db.db.getDataTable(this.name.sqlFromName, p, CommandType.StoredProcedure);
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
                        _resultsetDataTable = this.db.db.getDataTable(this.name.sqlFromName, p, CommandType.StoredProcedure);
                    }
                    #endregion compute resulset data table
                }
                return _resultsetDataTable;
            }
        }


    }


    

}


