using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using EM.Collections;
using System.Data.Common;

namespace EM.DB
{
    public class DBParam : IDBParam 
    {
        //public SqlParameter param;
        private DbParameter _param;

        public DbParameter param
        {
            get { return this._param; }
            set { this._param = value; }
        }

        public DBParam(DbParameter param) 
        {
            this.param = param;
        }

        public DBParam(string name, Object value) : this(name, value, typeof(SqlParameter)) { }
        public DBParam(string name, Object value, ParameterDirection direction) : this(name, value, direction, typeof(SqlParameter)) { }
        public DBParam(string name, Object value, Type actualDBParamType)
        {
            this.param = (DbParameter)Activator.CreateInstance(actualDBParamType);
            param.ParameterName = name;
            param.Value = value;
        }
        public DBParam(string name, Object value, ParameterDirection direction, Type actualDBParamType)
            : this(name, value, actualDBParamType)
        {
            this.param.Direction = direction;            
        }
        public DBParam(string name, Object value, DbType paramValueType) : this(name, value, typeof(SqlParameter), paramValueType) { }
        public DBParam(string name, Object value, Type actualDBParamType, DbType paramValueType)
            : this(name, value)
        {
            this.param = (DbParameter)Activator.CreateInstance(actualDBParamType);
            this.param.ParameterName = name;
            this.param.Value = value;
            this.param.DbType = paramValueType;
        }
        
        public DBParam(string name, Object value, Type actualDBParamType, DbType paramValueType, ParameterDirection direction)
            : this(name, value, actualDBParamType, paramValueType)
        {
            this.param.Direction = direction;
        }


        

        //////////////////////////////////////////////////////////////////////////////////////////////
        //// SQL Server Parameter
        public DBParam(string name, Object value, SqlDbType paramValueType)
            : this(name, value)
        {
            this.param = new SqlParameter();
            this.param.ParameterName = name;
            this.param.Value = value;
            ((SqlParameter)this.param).SqlDbType = paramValueType;                             
        }


        public DBParam(string name, Object value, SqlDbType paramType, ParameterDirection direction)
            : this(name, value, paramType)
        {
            this.param.Direction = direction;
        }

        public DBParam(string name, object value, SqlDbType paramType, int size, ParameterDirection direction)
        {
            this.param = new SqlParameter(name, paramType, size);
            SqlParameter param = (SqlParameter)this.param;
            param.Direction = direction;
            param.Value = value;            
        }
        public DBParam(string name, object value, SqlDbType paramType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale)
        {
            this.param = new SqlParameter(name, paramType, size);
            SqlParameter param = (SqlParameter)this.param;
            param.Direction = direction;
            param.Precision = precision;
            param.Scale = scale;
            param.IsNullable = isNullable;
            param.Value = value;            
        }

        public override string ToString()
        {
            return string.Format("DBParam {2}: {0} = {1}", this.param.ParameterName, this.param.Value, this.param.DbType);
        }

        public DBParam clone()
        {
            DbParameter p = (DbParameter)Activator.CreateInstance(this.param.GetType());
            p.ParameterName = this.param.ParameterName;
            p.Value = this.param.Value;
            p.Direction = this.param.Direction;
            p.DbType = this.param.DbType;
            p.Size = this.param.Size;
            
            if (this.param is SqlParameter)
            {
                var dp = (SqlParameter)p;
                var sp = (SqlParameter)this.param;
                dp.SqlDbType = sp.SqlDbType;
                dp.Precision = sp.Precision;
                dp.Scale = sp.Scale;
                dp.IsNullable = sp.IsNullable;
            }
            return new DBParam(p);
        }
        

    }
    
}

