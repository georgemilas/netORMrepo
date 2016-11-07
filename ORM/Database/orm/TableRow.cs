using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using EM.DB;
using ORM.DBFields;
using System.Data;
using System.Reflection;

using ORM.exceptions;
using ORM.render;
using ORM.generator;

//using System.Runtime.Remoting;

namespace ORM
{
    public abstract class TableRow
    {
        public TableName name;         //table name    
        protected bool _isReadOnly; //
        protected ORMContext _context;

        public OrderedDictionary<string, GenericField> fields;
        //public IFormRenderer _renderer;
        public PKInfo pk;
        public FKInfo fk;                          //meta-data
        public EList<DBRelation> oneToMany;     //meta-data
        public delegate bool ValidatorFunc();
        public EList<ValidatorFunc> validators;      //custom validators
        protected ESet<ValidationException> _validatorsExceptions;  //all exceptions go in only one time
        //public RenderAttributes renderAttributes;

        /// <summary>
        /// the DataTable from the last call of setFromDB()
        /// </summary>
        public DataTable adoDataTable;


        public TableRow(ORMContext context) 
        {
            this.context = context;
            this.fields = new OrderedDictionary<string, GenericField>();
            this._validatorsExceptions = new ESet<ValidationException>();
            this.validators = new EList<ValidatorFunc>();
            //this.renderAttributes = new RenderAttributes();
            this._isReadOnly = false;
        }
        
        /// <summary>
        /// alow a subclass to be partial and use init() in it's contructor (to alow for customization) 
        /// without necesarly writing the init method in a diferent partial file but only if wanted to
        /// actualy customize something 
        /// </summary>
        protected virtual void init()   //not abstract, see it's doc
        { }
        //protected virtual void initPresentation()   //not abstract, see it's doc
        //{ }

        //public abstract TableRow getInstance();
        public virtual TableRow getInstance()
        {
            //return TableRow.getInstance(this.GetType(), this.db);
            return this.context.classFactory.getInstance(this.GetType(), this.context);
        }
        public static TableRow getInstance(Type type, ORMContext context)
        {
            //return TableRow.getInstance(this.GetType(), this.db);
            TableRow tr = context.classFactory.getInstance(type, context); //get parent or children instance
            return context.classFactory.getInstance(tr.name, context);            //now make sure we get the children instance            
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
        ////////////////////// RENDER
        //////////////////////////////////////////////////////////////////////////////////////////////
        //public IFormRenderer renderer
        //{
        //    get 
        //    {
        //        //if (_renderer == null) return ORMContext.instance.formRenderer;
        //        if (_renderer == null) { throw new NullReferenceException("Renderer was not initialized"); }
        //        else { return _renderer; }
        //    }
        //    set { _renderer = value; }
        //}
        //protected RenderAttributes getRenderAttributes(RenderAttributes renderAttributes)
        //{
        //    return RenderAttributes.combine(renderAttributes, this.renderAttributes);
        //}
        //public Object render() { return this.renderer.render(this, this.renderAttributes); }
        //public Object render(string atrDictLiteral) { return this.render(RenderAttributes.fromStr(atrDictLiteral)); }
        //public Object render(RenderAttributes atr) { return this.renderer.render(this, RenderAttributes.combine(atr, this.renderAttributes)); }



        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// VALIDATE
        //////////////////////////////////////////////////////////////////////////////////////////////
        public bool validate()
        {
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
        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// DYNAMIC STUFF - RELATIONS & INSTANCES
        //////////////////////////////////////////////////////////////////////////////////////////////

        ////////// REFLECTIONS:
        public DBRelation getRelationFromType(Type tp)
        {
            PropertyInfo[] props = tp.GetProperties();
            DBRelation rel = null;
            foreach (DBRelation r in this.fk)
            {
                if (r.tableThere.className == tp.Name)
                {
                    rel = r;
                    break;
                }
            }
            if (rel == null) throw new ORMException(string.Format("no foreign key was defined in {0} for {1}", this.name.sqlFromName, tp.FullName));
            return rel;           
        }

        public Type geTypeFromRelation(DBRelation rel)
        {
            string clsName = this.GetType().Namespace + "." + rel.tableThere.className;
            //ObjectHandle oh = Activator.CreateInstance(null, clsName);
            //Type tp = oh.GetType();
            Type tp = Type.GetType(clsName);
            return tp;            
        }

        ////////// RELATIONS:
        //you may want to use this to create some real properties {get; set;} 
        //with a cache value in order to only query DB once
        
        /// <summary>
        /// Ex:  cust c = cust();
        ///      cust_adr ca = (cust_adr)c.getOneToOne(typeof(cust_adr));
        ///      cust_adr ca = (cust_adr)c.getOneToOne(this.fk[0]);
        /// </summary>
        public TableRow getOneToOne(Type tp) { return getOneToOne(tp, getRelationFromType(tp)); }
        public TableRow getOneToOne(DBRelation rel) { return getOneToOne(geTypeFromRelation(rel), rel); }
        public TableRow getOneToOne(Type tp, DBRelation rel)
        {
            DBParams params_ = new DBParams();
            EList<string> whereLst = new EList<string>();
            for(int i=0; i<rel.fieldsThere.Count; i++)
            {
                whereLst.Add(string.Format("{0} = @{1}", db.escape(rel.fieldsThere[i]), ORMContext.fixName(rel.fieldsHere[i]) ));
                params_.Add(new DBParam("@" + ORMContext.fixName(rel.fieldsHere[i]), this.fields[rel.fieldsHere[i]].value));
            }
            
            TableRow cls = (TableRow)Activator.CreateInstance(tp, this.context);
            
            SQLAttributes atr = new SQLAttributes(cls);
            atr.where = whereLst.join(" and ");

            cls.setFromDB(atr, params_);
            return cls;
        }

        /// <summary>
        /// Ex:  cust c = cust();
        ///      EList&lt;TableRow> calst = (cust_adr)c.getOneToMany(typeof(cust_adr));
        ///      EList&lt;TableRow> calst = (cust_adr)c.getOneToMany(this.fk[0]);
        ///      foreach(TableRow t in calst) 
        ///      {
        ///          cust_adr ca = (cust_adr)t;
        ///      }
        /// </summary>
        public TableRowList<T> getOneToMany<T>(Type tp) where T : TableRow { return getOneToMany<T>(tp, getRelationFromType(tp)); }
        public TableRowList<T> getOneToMany<T>(DBRelation rel) where T : TableRow { return getOneToMany<T>(geTypeFromRelation(rel), rel); }
        public TableRowList<T> getOneToMany<T>(Type tp, DBRelation rel) where T : TableRow 
        {
            DBParams params_ = new DBParams();
            EList<string> whereLst = new EList<string>();
            for (int i = 0; i < rel.fieldsThere.Count; i++)
            {
                whereLst.Add(string.Format("{0} = @{1}", db.escape(rel.fieldsThere[i]), ORMContext.fixName(rel.fieldsHere[i])));
                params_.Add(new DBParam("@" + ORMContext.fixName(rel.fieldsHere[i]), this.fields[rel.fieldsHere[i]].value));
            }
            
            TableRow cls = (TableRow)Activator.CreateInstance(tp, this.context);
            
            SQLAttributes atr = new SQLAttributes(cls);
            atr.where = whereLst.join(" and ");

            TableRowList<T> res = cls.select<T>(atr, params_);
            return res;                       
        }

        


        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// COUNT
        //////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void setSQLAttributes(ref SQLAttributes atr)
        {
            if (atr.fields == null || atr.fields.Trim() == "") atr.fields = "*";
            if (atr.from == null || atr.from.Trim() == "") atr.from = this.name.sqlFromName;
        }

        public virtual int countAll()
        {
            SQLAttributes atr = new SQLAttributes(this);
            this.setSQLAttributes(ref atr);
            if (atr.group == null)
            {
                atr.fields = "count(*)";
                atr.order = null;
                atr.having = null;
                string sql = atr.selectSql();
                return (int)db.db.executeScalar(sql);
            }
            else
            {
                atr.order = null;
                string sql = atr.selectSql();
                sql = string.Format(" select count(*) from ( {0} ) as AA", sql);
                return (int)db.db.executeScalar(sql);
            }
        }
        public virtual int countAll(SQLAttributes atr, DBParams param)
        {
            string fields = atr.fields;
            this.setSQLAttributes(ref atr);
            if (atr.group == null)
            {
                if (fields == null || fields.Trim() == "") atr.fields = "count(*)";
                atr.order = null;
                atr.having = null;
                string sql = atr.selectSql();
                return (int)db.db.executeScalar(sql, param);
            }
            else
            {
                atr.order = null;
                string sql = atr.selectSql();
                sql = string.Format(" select count(*) from ( {0} ) as AA", sql);
                return (int)db.db.executeScalar(sql, param);
            }            
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// SELECT
        //////////////////////////////////////////////////////////////////////////////////////////////
        public virtual TableRowList<T> select<T>() where T : TableRow 
        {
            return select<T>(new SQLAttributes(this), null);
        }
        /// <summary>
        /// SQLAttributes are ignored in the case of stored procedures
        /// </summary>
        public virtual TableRowList<T> select<T>(SQLAttributes atr) where T : TableRow 
        {
            return select<T>(atr, null);
        }
        /// <summary>
        /// SQLAttributes are ignored in the case of stored procedures
        /// </summary>
        public virtual TableRowList<T> select<T>(SQLAttributes atr, DBParams param) where T : TableRow 
        {
            this.setSQLAttributes(ref atr);
            DataTable tb = db.select(this, atr, param);     //ABSTRACT STORAGE         
            return this.getInstancesFromDataTable<T>(tb);
        }

        /// <summary>
        /// use this in case you have to much data and instances of actual TableRow objects would consume to much memory
        /// </summary>
        public virtual DataTable selectDataTable() { return selectDataTable(new SQLAttributes(this), null); }
        public virtual DataTable selectDataTable(SQLAttributes atr) { return selectDataTable(atr, null); }
        public virtual DataTable selectDataTable(SQLAttributes atr, DBParams param)
        {
            this.setSQLAttributes(ref atr);
            return db.select(this, atr, param);     //ABSTRACT STORAGE         
        }

        public TableRowList<T> getInstancesFromDataTable<T>(DataTable tb) where T : TableRow 
        {
            TableRowList<T> res = new TableRowList<T>();
            res.db = this.db;
            res.adoDataTable = tb;
            foreach (DataRow row in tb.Rows)
            {
                TableRow cls = this.getInstance();
                cls.setFromDataRow(row, tb.Columns);    
                res.Add((T)cls);
            }
            return res;
        }

        protected string getQueryParams(DBParams param) 
        {
            EList<string> paramsMsg = new EList<string>();
            if (this.name != null)
            {
                paramsMsg.Add("obj=" + this.name.sqlFromName);
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

        /// <summary>
        /// set instance fields values to DataRow values
        /// </summary>
        public virtual void setFromDataRow(DataRow row) { setFromDataRow(row, row.Table.Columns); }
        public virtual void setFromDataRow(DataRow row, DataColumnCollection columns)
        {
            
            foreach (DataColumn c in columns)
            {
                //doing it this way (rather then loop over this.fields) we can select only a few fields 
                //atr.fields = "fld_id, fld_cust_id"   instead of atr.fields = "*"
                //but when we do do "*" and if table structure has changed then we may have error of this.fields KeyNotFind err
                //so we need to check this.fields.ContainsKey 
                if (c.ColumnName != "RowsNumberCount" && this.fields.ContainsKey(c.ColumnName))
                {
                    this.fields[c.ColumnName].value = row[c];
                    this.fields[c.ColumnName].oldValue = row[c];
                    //this.fields[c.ColumnName].defaultValue = c.DefaultValue;       //does not work
                }
            }
        }

        
        /// <summary>
        /// does a select on database then calls setFromDataRow
        /// </summary>
        public virtual void setFromDB(SQLAttributes atr) { setFromDB(atr, null); }
        public virtual void setFromDB(SQLAttributes atr, DBParams param)
        {
            this.setSQLAttributes(ref atr);

            DataTable tb = db.select(this, atr, param);     //ABSTRACT STORAGE         
            this.adoDataTable = tb;

            if (tb.Rows.Count > 1)
            {
                throw new BusinessLogicError("more then one data row was returned by your query " + this.getQueryParams(param));
            }
            if (tb.Rows.Count == 0)
            {
                throw new BusinessLogicError("No data was found for your criteria " + this.getQueryParams(param));
            }
            DataRow row = tb.Rows[0];
            this.setFromDataRow(row, tb.Columns);            
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// DATA STORAGE
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// will do either an insert or update depending if the object was already select-ed  from db or not
        /// </summary>
        /// <returns>object which you can cast to int for insert or bool for update</returns>
        public virtual object save()
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Save is not supported to Read Only TableRows");
            }
            bool isInsert = true;
            foreach (GenericField f in this.fields.Values)
            {
                if (f.oldValue != null)
                {
                    isInsert = false;
                    break;
                }
            }
            if (isInsert) return insert();
            return update();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// INSERT
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// execute database insert for current values of the object instance
        /// </summary>
        /// <returns>
        /// - returns identity value is the PK is autonumber (-1 if it failed and engine was set to not throw the errors)
        /// - if PK is not autonumber returns 0 for success and -1 for failure
        /// </returns>
        public int insert() 
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Insert is not supported to Read Only TableRows");
            }
            if (!this.validate()) { throw new ValidationException("data could not be inserted, there were validation errors"); }

            SQLAttributes atr = new SQLAttributes(this);
            DBParams p = new DBParams();
            
            if (atr.fields == null || atr.fields.Trim() == "")
            {
                EList<string> fieldsNames = new EList<string>();
                EList<string> fieldsValues = new EList<string>();
                foreach (string fld in this.fields.Keys)
                {
                    if (this.fields[fld].isIdentity) continue;
                    if (this.fields[fld].isComputed) continue;
                    if ((this.fields[fld].value == null || this.fields[fld].value.ToString() == "") && this.fields[fld].hasDefaultConstraint) continue;
                    if ((this.fields[fld].value == null || this.fields[fld].value.ToString() == "") && this.fields[fld].defaultValue != null)
                    {
                        fieldsNames.Add(string.Format("{0}", this.db.escape(fld)));
                        fieldsValues.Add(string.Format("@i_{0}", ORMContext.fixName(fld)));
                        p.Add(new DBParam("@i_" + ORMContext.fixName(fld), this.fields[fld].defaultValue ));
                        continue;
                    }

                    fieldsNames.Add(string.Format("{0}", this.db.escape(fld)));
                    fieldsValues.Add(string.Format("@i_{0}", ORMContext.fixName(fld)));
                    p.Add(new DBParam("@i_" + ORMContext.fixName(fld), this.fields[fld].value!=null? this.fields[fld].value: System.DBNull.Value));
                }
                atr.fields = "(" + fieldsNames.join(", ") + ") VALUES (" + fieldsValues.join(", ") + ")";
            }
            if (atr.from == null || atr.from.Trim() == "") atr.from = this.name.sqlFromName;

            object dbres = this.db.insert(this, atr, p);    //ABSTRACT STORAGE         

            int res;
            if (dbres is int)       //table has identity column so we have a new insert id
            {
                if ((int)dbres > 0)
                {
                    this.fields[this.pk[0]].value = (int)dbres;
                    foreach (string fld in this.fields.Keys)
                    {
                        this.fields[fld].oldValue = this.fields[fld].value;
                    }
                }
                res = (int)dbres;
            }
            else
            {
                //bool
                if ((bool)dbres)
                {
                    foreach (string fld in this.fields.Keys)
                    {
                        this.fields[fld].oldValue = this.fields[fld].value;
                    }
                }
                res = (bool)dbres ? 0 : -1;
            }
            //int res = -1;
            return res;
            
            
            
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// UPDATE
        //////////////////////////////////////////////////////////////////////////////////////////////
        
        protected void _setUpdateWhere(ref SQLAttributes atr, ref DBParams p) 
        {
            if (this.pk.Count == 0) { throw new ValidationException(string.Format("{0} can not be updated using update() because no Primary Key was defined for it. To update use update(SQLAttribute)", this.name.sqlFromName)); }
            EList<string> where = new EList<string>();
            
            foreach (string fld in this.pk)
            {
                where.Add(string.Format("{0}=@w_{1}", db.escape(fld), ORMContext.fixName(fld)));
                p.Add(new DBParam("@w_" + ORMContext.fixName(fld), this.fields[fld].oldValue));
            }
            atr.where = where.join(" and ");
        }
        
        public bool update()
        {
            SQLAttributes atr = new SQLAttributes(this);
            DBParams p = new DBParams();
            this._setUpdateWhere(ref atr, ref p);
            return update(atr, p); 
        }
        /// <summary>
        /// you may want to give a custom where clause
        /// </summary>
        public bool update(SQLAttributes atr) { return update(atr, null); }
        /// <summary>
        /// you may want to give a custom where clause
        /// </summary>
        public bool update(SQLAttributes atr, DBParams p) 
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Update is not supported to Read Only TableRows");
            }

            if (!this.validate()) { throw new ValidationException("data could not be updated, there were validation errors"); }

            if (atr.fields == null || atr.fields.Trim() == "")
            {
                EList<string> fields = new EList<string>();
                foreach (string fld in this.fields.Keys)
                {
                    if (this.fields[fld].isIdentity) continue;
                    if (this.fields[fld].isComputed) continue;
                    //if column value not modified: continue
                    if (this.fields[fld].value != null && this.fields[fld].oldValue != null && this.fields[fld].value.ToString() == this.fields[fld].oldValue.ToString()) continue;
                    if (this.fields[fld].value == null && this.fields[fld].oldValue == null) continue;
                    if (this.fields[fld].value == null && this.fields[fld].oldValue is string && (string)this.fields[fld].oldValue == "") continue;

                    if ( this.fields[fld].value == null && this.fields[fld].hasDefaultConstraint ) continue;
                    if ( this.fields[fld].value == null && this.fields[fld].defaultValue != null )
                    {
                        fields.Add(string.Format("{0}=@u_{1}", this.db.escape(fld), ORMContext.fixName(fld)));
                        p.Add(new DBParam("@u_" + ORMContext.fixName(fld), this.fields[fld].defaultValue));
                        this.fields[fld].value = this.fields[fld].defaultValue;
                        continue;
                    }
                    fields.Add(string.Format("{0}=@u_{1}", this.db.escape(fld), ORMContext.fixName(fld)));
                    p.Add(new DBParam("@u_" + ORMContext.fixName(fld), this.fields[fld].value != null ? this.fields[fld].value : System.DBNull.Value));
                }
                atr.fields = fields.join(", ");
            }
            if (atr.fields == "")
            {
                return true;    //nothing changed so nothing to do
            }
            if (atr.where == null || atr.where.Trim() == "")
            {
                this._setUpdateWhere(ref atr, ref p);
            }
            if (atr.from == null || atr.from.Trim() == "") atr.from = this.name.sqlFromName;

            
            bool res = this.db.update(this, atr, p);   //ABSTRACT STORAGE         
            
            //bool res = false;
            if (res)
            {
                foreach (string fld in this.fields.Keys)
                {
                    this.fields[fld].oldValue = this.fields[fld].value;
                }
            }
            return res;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// DELETE
        //////////////////////////////////////////////////////////////////////////////////////////////
        
        public bool delete() 
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Delete is not supported to Read Only TableRows");
            }

            SQLAttributes atr = new SQLAttributes(this);
            if (this.pk.Count == 0) { throw new ValidationException(string.Format("{0} can not be deleted using delete() because no Primary Key was defined for it. To delete use delete(SQLAttribute)", this.name.sqlFromName)); }
            EList<string> where = new EList<string>();
            DBParams p = new DBParams();
            foreach(string fld in this.pk)
            {
               where.Add(string.Format("{0}=@{1}",db.escape(fld), ORMContext.fixName(fld)));
               p.Add(new DBParam("@"+ORMContext.fixName(fld), this.fields[fld].value));
            }
            atr.where = where.join(" and ");
            return delete(atr, p); 
        }
        public bool delete(SQLAttributes atr) { return delete(atr, null); }        
        public bool delete(SQLAttributes atr, DBParams p) 
        {
            if (atr.from == null || atr.from.Trim() == "") atr.from = this.name.sqlFromName;
            return this.db.delete(this, atr, p);      //ABSTRACT STORAGE     
        }

    }

}
