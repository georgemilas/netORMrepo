using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using EM.Collections;

namespace EM.DB
{
    public class DBParams : EList<IDBParam>, IDBParams
    {
        public Type actualDBParamType;
        private Dictionary<string, DBParam> byName;

        public DBParams(): this(typeof(SqlParameter)) {}
        public DBParams(Type actualDBParamType)
            : base()
        {
            this.actualDBParamType = actualDBParamType;
            this.byName = new Dictionary<string, DBParam>();
        }


        //a shortcut
        public void Add(string name, object value)
        {
            if (this.actualDBParamType != null)
            {
                this.Add(name, value, this.actualDBParamType);                
            }
            else
            {
                DBParam p = new DBParam(name, value);
                this.Add(p);
                byName[name] = p;
            }
        }

        public void Add(string name, object value, ParameterDirection direction)
        {
            if (this.actualDBParamType != null)
            {
                this.Add(name, value, direction, this.actualDBParamType);
            }
            else
            {
                DBParam p = new DBParam(name, value, direction);
                this.Add(p);
                byName[name] = p;
            }
        }

        public void Add(string name, object value, Type actualDBParamType)
        {
            DBParam p = new DBParam(name, value, actualDBParamType);
            this.Add(p);
            byName[name] = p;
        }

        public void Add(string name, object value, ParameterDirection direction, Type actualDBParamType)
        {
            DBParam p = new DBParam(name, value, direction, actualDBParamType);
            this.Add(p);
            byName[name] = p;
        }

        public void Add(string name, object value, ParameterDirection direction, SqlDbType sqlDbType)
        {
            DBParam p = new DBParam(name, value, sqlDbType, direction);
            this.Add(p);
            byName[name] = p;
        }
        public void Add(string name, object value, ParameterDirection direction, SqlDbType paramType, int size)
        {
            DBParam p = new DBParam(name, value, paramType, size, direction);
            this.Add(p);
            byName[name] = p;
        }

        /// <summary>
        /// get the value of the named parameter (for ex. if it's a output parameter, you might want to get it's value)
        /// </summary>
        public object this[string name] 
        {
            get 
            {
                return byName[name].param.Value;
            }
        }

        /// <summary>
        /// clone the params in order to "reuse" the collection without getting the error that the same params have already been used
        /// </summary>
        public DBParams clone()
        {
            DBParams res = new DBParams();
            foreach (DBParam p in this)
            {
                res.Add(p.clone());
            }
            return res;
        }

    }

}

