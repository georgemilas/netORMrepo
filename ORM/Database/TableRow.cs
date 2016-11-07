using System;
using EM.Collections;
using EM.DB;
using ORM.DBFields;
using System.Data;
using ORM.exceptions;
using ORM.db_store;
using ORM.db_store.persitence;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Generic;

//using System.Runtime.Remoting;

namespace ORM
{

    [Serializable]
    public abstract class TableRow : ITableRow
    {
        public TableName dbObjectName { get; set; }         //table name    
        protected bool _isReadOnly; //
        protected ORMContext _context;

        //lowerCaseFields will get set in the generated constructor after the fields dictionary is all set
        protected Dictionary<string, GenericField> lowerCaseFields { get; set; }
        //the fields dictionary may need to be a OrderedDictionary type in order to preserve fields order for automatic display purposes (like ORM/render api) but the set[] operation in the dictinary may be to slow given the addional list it has to mainten for the order
        public Dictionary<string, GenericField> fields  { get; set; }  

        public PKInfo pk { get; set; }
        public FKInfo fk { get; set; }                          //meta-data
        public OneToManyInfo oneToMany { get; set; }     //meta-data
        public delegate bool ValidatorFunc();
        public EList<ValidatorFunc> validators { get; set; }      //custom validators
        protected ESet<ValidationException> _validatorsExceptions;  //all exceptions go in only one time
        /// <summary>
        /// by default this is false, but you can use it to force a call to validate() to always return true 
        /// </summary>
        public bool bypassValidation { get; set; }

        protected EDictionary<Type, object> cashList = new EDictionary<Type, object>();
        protected EDictionary<Type, object> cashObject = new EDictionary<Type, object>();
        
        /// <summary>
        /// the DataTable from the last call of setFromDB()
        /// </summary>
        //protected internal DataTable adoDataTable { get; set; }
        public DataTable adoDataTable { get; set; }

        public TableRow() { }
        public TableRow(ORMContext context) 
        {
            this.context = context;
            this.lowerCaseFields = new Dictionary<string, GenericField>();
            this.fields = new Dictionary<string, GenericField>();
            this._validatorsExceptions = new ESet<ValidationException>();
            this.validators = new EList<ValidatorFunc>();
            this.bypassValidation = false;
            //this.renderAttributes = new RenderAttributes();
            this._isReadOnly = false;
        }

        protected void setLowerCaseFields()
        {
            this.lowerCaseFields = new Dictionary<string, GenericField>();
            foreach (string key in this.fields.Keys)
            {
                this.lowerCaseFields[key.ToLower()] = this.fields[key];
            }
        }

        /// <summary>
        /// alow a subclass to be partial and use init() in it's contructor (to alow for customization) 
        /// without necesarly writing the init method in a diferent partial file but only if wanted to
        /// actualy customize something 
        /// </summary>
        protected virtual void init()   //not abstract, see it's doc
        { }
 
        public virtual TableRow getInstance()
        {
            return this.context.classFactory.getInstance(this.GetType(), this.context);
        }

        /// <summary>
        /// Clones a TableRow, will exclude identity columns
        /// </summary>
        public virtual T clone<T>() where T : TableRow
        {
            T cls = (T)Activator.CreateInstance(typeof(T), this.context);
            foreach (string f in this.fields.Keys)
            {
                if (!this.fields[f].isIdentity)
                {
                    cls.fields[f].value = this.fields[f].value;
                }
            }
            return cls;
        }

        public static T getInstance<T>(ORMContext cx) where T : TableRow
        {
            T cls = (T)Activator.CreateInstance(typeof(T), cx);
            return cls;
        }

        public static T getInstance<T>(ORMContext cx, DataRow dr) where T : TableRow
        {
            T cls = (T)Activator.CreateInstance(typeof(T), cx);
            cls.setFromDataRow(dr);
            return cls;
        }

        /// <summary>
        /// supose we have tblUser getnerated and a business entity User inheriting from it
        /// and tblProjetct has FK to tblUser, then
        /// allow tblProjetct.tblUser to actually be User
        /// </summary>
        public static TableRow getInstance(Type type, ORMContext context)
        {
            //get parent or children instance ( either tblUser or User )
            TableRow tr = context.classFactory.getInstance(type, context);
            //now make sure we alow getting the children instance ( User )
            return context.classFactory.getInstance(tr.dbObjectName, context);     
        }

        public bool isReadOnly { get { return _isReadOnly; } }

        public ORMContext context
        {
            get
            {
                if (_context == null) { throw new NullReferenceException("ORM Context was not suplied"); }
                return _context;
            }
            set { 
                this._context = value;
            }
        }
        public GenericDatabase db
        {
            get
            {
                return this.context.db;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// VALIDATE
        //////////////////////////////////////////////////////////////////////////////////////////////
        public bool validate()
        {
            if (bypassValidation) { return true; }

            this._validatorsExceptions = new ESet<ValidationException>();
            bool res = true;
            foreach (string key in this.fields.Keys)
            {
                res = this.fields[key].validate() && res;
            }
            foreach (ValidatorFunc v in this.validators)
            {
                //if v() is to fail, it should add a new ValidationException to the list of exceptions and return false
                res = v() && res;
            }

            if (!res)   //add all field level validation errors
            {
                foreach (GenericField f in this.fields.Values)
                {
                    foreach (ValidationException e in f.validationErrors)
                    {
                        this._validatorsExceptions.Add(e);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// a list of all validation errors from both table level and all the fields 
        /// </summary>
        public ESet<ValidationException> validationExceptions
        {
            get
            {
                return this._validatorsExceptions;
            }
        }


        /// <summary>
        /// object encapsulating the list of all validation errors from both table level and all the fields 
        /// </summary>
        public TableValidationExceptions tableValidationExceptions
        {
            get
            {
                return new TableValidationExceptions("Validation Errors:", this.dbObjectName.ToString(), this._validatorsExceptions);
            }
        }

        public static Table<T> getInstancesFromDataTable<T>(ORMContext cx, DataTable tb) where T : TableRow
        {
            Table<T> res = new Table<T>();
            res.db = cx.db;
            res.adoDataTable = tb;
            foreach (DataRow row in tb.Rows)
            {
                //TableRow cls = this.getInstance();
                T cls = (T)Activator.CreateInstance(typeof(T), cx);
                cls.setFromDataRow(row, tb.Columns);
                res.Add(cls);
            }
            return res;
        }
        public virtual Table<T> getInstancesFromDataTable<T>(DataTable tb) where T : TableRow 
        {
            return TableRow.getInstancesFromDataTable<T>(this.context, tb);
        }


        protected string getQueryParams(DBParams param)
        {
            EList<string> paramsMsg = new EList<string>();
            if (this.dbObjectName != null)
            {
                paramsMsg.Add("obj=" + this.dbObjectName.sqlFromName);
            }
            if (param != null)
            {
                foreach (DBParam p in param)
                {
                    paramsMsg.Add(p.param.ParameterName + "=" + p.param.Value.ToString());
                }
            }
            return paramsMsg.join(",");
        }

        public void setFromOneRowDataTable(DBParams param, DataTable tb)
        {
            if (tb.Rows.Count > 1)
            {
                throw new BusinessLogicError("more then one data row was returned by your query " + this.getQueryParams(param));
            }
            if (tb.Rows.Count == 0)
            {
                throw new BusinessLogicError("No data was found for your criteria " + this.getQueryParams(param));
            }
            this.adoDataTable = tb;
            DataRow row = tb.Rows[0];
            this.setFromDataRow(row, tb.Columns);
        }

        public void setFromOneRowDataTableCaseInsensitive(DBParams param, DataTable tb)
        {
            if (tb.Rows.Count > 1)
            {
                throw new BusinessLogicError("more then one data row was returned by your query " + this.getQueryParams(param));
            }
            if (tb.Rows.Count == 0)
            {
                throw new BusinessLogicError("No data was found for your criteria " + this.getQueryParams(param));
            }
            this.adoDataTable = tb;
            DataRow row = tb.Rows[0];
            this.setFromDataRowCaseInsensitive(row, tb.Columns);
        }


        /// <summary>
        /// set instance fields values to DataRow values
        /// </summary>
        public virtual void setFromDataRow(DataRow row) { setFromDataRow(row, row.Table.Columns); }


        public virtual void setFromDataRow(DataRow row, DataColumnCollection columns)
        {
            this.setFromDataRowCaseInsensitive(row, columns);            
        }

        public virtual void setFromDataReader(DbDataReader reader)
        {
            foreach (string colName in this.fields.Keys)
            {
                object val = reader[colName];
                GenericField gf = this.fields[colName];
                val = getValueSafe(val, gf);
                if (gf.isComputed)
                {
                    //.value can not be set on computed fields
                    gf._value = val;
                }
                else
                {
                    gf.value = val;
                }
                gf.oldValue = val;                
            }
        }

        public virtual void setFromDataRowCaseInsensitive(DataRow row) { setFromDataRowCaseInsensitive(row, row.Table.Columns); }
        public virtual void setFromDataRowCaseInsensitive(DataRow row, DataColumnCollection columns)
        {
            if (this.lowerCaseFields == null || this.lowerCaseFields.Count != this.fields.Count)
            {
                setLowerCaseFields();
            }

            foreach (DataColumn c in columns)
            {
                //doing it this way (rather then loop over this.fields) we can select only a few fields 
                //stm.fields = "fld_id, fld_cust_id"   instead of stm.fields = "*"
                //but when we do do "*", if table structure changed we check this.fields.ContainsKey to avoid errors
                string colName = c.ColumnName.ToLower();
                if (c.ColumnName != "RowsNumberCount" && this.lowerCaseFields.ContainsKey(colName))
                {
                    //object val = getValueFromDatabase(row, c.ColumnName);
                    var gf = this.lowerCaseFields[colName];
                    object val = getValueSafe(row[c.ColumnName], gf);
                    if (gf.isComputed)
                    {
                        //.value can not be set on computed fields
                        gf._value = val;
                    }
                    else
                    {
                        gf.value = val;
                    }
                    gf.oldValue = val;
                    //gf.defaultValue = c.DefaultValue;       //does not work
                }
            }
        }

        public virtual void setFromDataRowCaseSensitive(DataRow row) { setFromDataRowCaseSensitive(row, row.Table.Columns); }
        public virtual void setFromDataRowCaseSensitive(DataRow row, DataColumnCollection columns)
        {
            foreach (DataColumn c in columns)
            {
                //doing it this way (rather then loop over this.fields) we can select only a few fields 
                //stm.fields = "fld_id, fld_cust_id"   instead of stm.fields = "*"
                //but when we do do "*", if table structure changed we check this.fields.ContainsKey to avoid errors
                string colName = c.ColumnName;
                if (c.ColumnName != "RowsNumberCount" && this.fields.ContainsKey(colName))
                {
                    var gf = this.fields[colName];
                    object val = getValueSafe(row[c.ColumnName], gf);                    
                    if (gf.isComputed)
                    {
                        //.value can not be set on computed fields
                        gf._value = val;
                    }
                    else
                    {
                        gf.value = val;
                    }
                    gf.oldValue = val;
                    //gf.defaultValue = c.DefaultValue;       //does not work
                }
            }
        }

        protected object getValueFromDatabase(DataRow row, string key)
        {
            object val = row[key];
            GenericField gf = this.lowerCaseFields[key.ToLower()];
            return getValueSafe(val, gf);
        }

        /// <summary>
        /// handle SQL server behaviour for CHAR fields etc.
        /// </summary>
        protected object getValueSafe(object val, GenericField gf)
        {
            //support the wrong and stupid SQL Server behaviour of doing what they please with CHAR fields 
            //They are adding / removing spaces to a char field to either 
            //allow inserting something like "AA" to a char(5) 
            //or returning "AA" or maybe returning "AA " instead "AA   "
            //the value that was actualy added into the database 
            //(so they added 3 spaces for insert and they are removing 2 for retreival for example)
            //If in TEST we do SELECT LEN('*'+LIVE_STATE+'*'), len(LIVE_STATE), [LIVE_STATE] FROM DN WHERE BCL_code = 'T3DJ' AND e1_id = 3
            //we get "3,0, " 
            //so have len == 3 instead of 4 (LIVE_STATE defined as char(2) NULL)
            if (gf.GetType() == typeof(FChar) && val != DBNull.Value && val != null)
            {
                FChar cf = (FChar)gf;
                string sval = (string)val;
                if (sval.Length < cf.length)
                {
                    sval = sval.PadRight(cf.length, ' ');
                }
                return sval;
            }
            return val;
        }

        protected DBParam getDBParam(string fld) { return getDBParam(fld, "@"); }
        protected virtual DBParam getDBParam(string fld, string prefix)
        {
            //for insert ?
            //if ((this.fields[fld].value == null || this.fields[fld].value.ToString() == "") && this.fields[fld].defaultValue != null)  
            //for update ?
            //if (this.fields[fld].value == null && this.fields[fld].defaultValue != null)        
            if (this.fields[fld].value == null && this.fields[fld].defaultValue != null)
            {
                return this.db.getDBParam(this.fields[fld], this.fields[fld].defaultValue, prefix + ORMContext.fixName(fld));
            }
            return this.db.getDBParam(this.fields[fld], this.fields[fld].value, prefix + ORMContext.fixName(fld));            
        }

        public class FieldsValueChanged
        {
            public EList<GenericField> valueChaged = new EList<GenericField>();
            public EList<GenericField> valueUseDefaultValue = new EList<GenericField>();
        }
        /// <summary>
        /// get a list of fields for which the value has changed
        /// </summary>
        public FieldsValueChanged getFieldsValueChanged()
        {
            FieldsValueChanged res = new FieldsValueChanged();
            foreach (string fld in this.fields.Keys)
            {
                GenericField f = this.fields[fld];
                if (f.isIdentity) continue;
                if (f.isComputed) continue;
                //if column value not modified: continue
                if (f.value != null && f.oldValueSafe != null && f.value.ToString() == f.oldValue.ToString()) continue;
                if (f.value == null && f.oldValueSafe == null) continue;
                if (f.value == null && f.oldValue is string && (string)f.oldValue == "") continue;

                if (f.value == null && f.hasDefaultConstraint) continue;
                if (f.value == null && f.defaultValue != null)
                {
                    res.valueUseDefaultValue.Add(f);
                    continue;
                }
                res.valueChaged.Add(f);                
            }
            return res;
        }


    }
    

}
