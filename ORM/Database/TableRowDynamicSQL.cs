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
using ORM.db_store;
using ORM.db_store.persitence;

//using System.Runtime.Remoting;

namespace ORM
{
    [Serializable]
    public class TableRowDynamicSQL : TableRowPersist
    {
        public TableRowDynamicSQL() : base() { }
        public TableRowDynamicSQL(ORMContext context) : base(context)
        {
            
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// DYNAMIC STUFF - RELATIONS & INSTANCES
        //////////////////////////////////////////////////////////////////////////////////////////////

        ////////// RELATIONS:
        //you may want to use this to create some real properties {get; set;} 
        //with a cache value in order to only query DB once
        
        /// <summary>
        /// Ex:  cust c = cust();
        ///      cust_adr ca = (cust_adr)c.getOneToOne(typeof(cust_adr));
        ///      cust_adr ca = (cust_adr)c.getOneToOne(this.fk[0]);
        /// </summary>
        public override TableRowPersist getOneToOne(Type tp, DBRelation rel) 
        {
            DBParams params_ = new DBParams();
            EList<string> whereLst = new EList<string>();
            for(int i=0; i<rel.fieldsThere.Count; i++)
            {
                whereLst.Add(string.Format("{0} = @{1}", db.escape(rel.fieldsThere[i]), ORMContext.fixName(rel.fieldsHere[i]) ));
                params_.Add(new DBParam("@" + ORMContext.fixName(rel.fieldsHere[i]), this.fields[rel.fieldsHere[i]].value));
            }

            TableRowDynamicSQL cls = (TableRowDynamicSQL)Activator.CreateInstance(tp, this.context);

            SQLStatement stm = new SQLStatement(cls);
            stm.where = whereLst.join(" and ");
            cls.setFromDB(stm, params_);
            
            return cls;
        }
        public virtual T getOneToOne<T>(Type tp, DBRelation rel) where T : TableRowDynamicSQL
        {
            TableRow cls = this.getOneToOne(tp, rel);
            return (T)cls;
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
        public override TablePersist<T> getOneToMany<T>(Type tp, DBRelation rel) 
        {
            DBParams params_ = new DBParams();
            EList<string> whereLst = new EList<string>();
            for (int i = 0; i < rel.fieldsThere.Count; i++)
            {
                whereLst.Add(string.Format("{0} = @{1}", db.escape(rel.fieldsThere[i]), ORMContext.fixName(rel.fieldsHere[i])));
                params_.Add(new DBParam("@" + ORMContext.fixName(rel.fieldsHere[i]), this.fields[rel.fieldsHere[i]].value));
            }
            
            TableRowDynamicSQL cls = (TableRowDynamicSQL)Activator.CreateInstance(tp, this.context);
            
            SQLStatement stm = new SQLStatement(cls);
            stm.where = whereLst.join(" and ");

            TablePersist<T> res = cls.select<T>(stm, params_);
            return res;                       
        }

        public virtual DataTable getOneToMany(Type tp, DBRelation rel)
        {
            DBParams params_ = new DBParams();
            EList<string> whereLst = new EList<string>();
            for (int i = 0; i < rel.fieldsThere.Count; i++)
            {
                whereLst.Add(string.Format("{0} = @{1}", db.escape(rel.fieldsThere[i]), ORMContext.fixName(rel.fieldsHere[i])));
                params_.Add(new DBParam("@" + ORMContext.fixName(rel.fieldsHere[i]), this.fields[rel.fieldsHere[i]].value));
            }

            TableRowDynamicSQL cls = (TableRowDynamicSQL)Activator.CreateInstance(tp, this.context);

            SQLStatement stm = new SQLStatement(cls);
            stm.where = whereLst.join(" and ");

            DataTable res = cls.selectDataTable(stm, params_);
            return res;
        }



        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// COUNT
        //////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void setSQLAttributes(ref SQLStatement stm)
        {
            if (stm.fields == null || stm.fields.Trim() == "") stm.fields = "*";
            if (stm.from == null || stm.from.Trim() == "") stm.from = this.dbObjectName.sqlFromName;
        }

        public override int countAll()
        {
            SQLStatement stm = new SQLStatement(this);
            this.setSQLAttributes(ref stm);
            if (stm.group == null)
            {
                stm.fields = "count(*)";
                stm.order = null;
                stm.having = null;
                string sql = stm.selectSql();
                return (int)db.db.executeScalar(sql);
            }
            else
            {
                stm.order = null;
                string sql = stm.selectSql();
                sql = string.Format(" select count(*) from ( {0} ) as AA", sql);
                return (int)db.db.executeScalar(sql);
            }
        }
        public virtual int countAll(SQLStatement stm, DBParams param)
        {
            string fields = stm.fields;
            this.setSQLAttributes(ref stm);
            if (stm.group == null)
            {
                if (fields == null || fields.Trim() == "") stm.fields = "count(*)";
                stm.order = null;
                stm.having = null;
                string sql = stm.selectSql();
                return (int)db.db.executeScalar(sql, param);
            }
            else
            {
                stm.order = null;
                string sql = stm.selectSql();
                sql = string.Format(" select count(*) from ( {0} ) as AA", sql);
                return (int)db.db.executeScalar(sql, param);
            }            
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// SELECT
        //////////////////////////////////////////////////////////////////////////////////////////////
        public virtual TablePersist<T> select<T>() where T : TableRowPersist
        {
            return select<T>(new SQLStatement(this), null);
        }
        /// <summary>
        /// SQLStatement are ignored in the case of stored procedures
        /// </summary>
        public virtual TablePersist<T> select<T>(SQLStatement atr) where T : TableRowPersist
        {
            return select<T>(atr, null);
        }
        /// <summary>
        /// SQLStatement are ignored in the case of stored procedures
        /// </summary>
        public virtual TablePersist<T> select<T>(SQLStatement atr, DBParams param) where T : TableRowPersist
        {
            this.setSQLAttributes(ref atr);
            DataTable tb = db.select(this, atr, param);     //ABSTRACT STORAGE         
            return this.getInstancesFromDataTable<T>(tb);
        }

        /// <summary>
        /// use this in case you have to much data and instances of actual TableRow objects would consume to much memory
        /// </summary>
        public virtual DataTable selectDataTable() { return selectDataTable(new SQLStatement(this), null); }
        public virtual DataTable selectDataTable(SQLStatement atr) { return selectDataTable(atr, null); }
        public virtual DataTable selectDataTable(SQLStatement atr, DBParams param)
        {
            this.setSQLAttributes(ref atr);
            return db.select(this, atr, param);     //ABSTRACT STORAGE         
        }

        

        
        /// <summary>
        /// does a select on database then calls setFromDataRow
        /// </summary>
        public virtual void setFromDB(SQLStatement atr) { setFromDB(atr, null); }
        public virtual void setFromDB(SQLStatement atr, DBParams param)
        {
            this.setSQLAttributes(ref atr);
            DataTable tb = db.select(this, atr, param);     //ABSTRACT STORAGE         
            this.setFromOneRowDataTable(param, tb);        
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
        public override int insert() 
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Insert is not supported to Read Only TableRows");
            }
            if (!this.validate()) 
            {
                throw new TableValidationExceptions("Data could not be inserted, there were validation errors:", this.dbObjectName.ToString(), this.validationExceptions);
            }

            SQLStatement atr = new SQLStatement(this);
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
                    //if ((this.fields[fld].value == null || this.fields[fld].value.ToString() == "") && this.fields[fld].defaultValue != null)
                    if (this.fields[fld].value == null && this.fields[fld].defaultValue != null)
                    {
                        fieldsNames.Add(string.Format("{0}", this.db.escape(fld)));
                        fieldsValues.Add(string.Format("@i_{0}", ORMContext.fixName(fld)));
                        //p.Add(new DBParam("@i_" + ORMContext.fixName(fld), this.fields[fld].defaultValue ));
                        p.Add(getDBParam(fld, "@i_"));
                        continue;
                    }

                    fieldsNames.Add(string.Format("{0}", this.db.escape(fld)));
                    fieldsValues.Add(string.Format("@i_{0}", ORMContext.fixName(fld)));
                    p.Add(getDBParam(fld, "@i_"));                    
                    
                }
                atr.fields = "(" + fieldsNames.join(", ") + ") VALUES (" + fieldsValues.join(", ") + ")";
            }
            if (atr.from == null || atr.from.Trim() == "") atr.from = this.dbObjectName.sqlFromName;

            object dbres = this.db.insert(this, atr, p);    //ABSTRACT STORAGE         

            int res = 0;
            int.TryParse(dbres.ToString(), out res); 
            if ( !(dbres is bool) )       //table has identity column so we have a new insert id
            {
                if (res > 0)
                {
                    foreach (string fld in this.fields.Keys)
                    {
                        if (this.fields[fld].isIdentity)
                        {
                            this.fields[fld].value = dbres;
                        }
                        this.fields[fld].oldValue = this.fields[fld].value;
                    }

                }                
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
        
        protected void _setUpdateWhere(ref SQLStatement atr, ref DBParams p) 
        {
            if (this.pk.Count == 0) { throw new ValidationException(string.Format("{0} can not be updated using update() because no Primary Key was defined for it. To update use update(SQLAttribute)", this.dbObjectName.sqlFromName)); }
            EList<string> where = new EList<string>();
            
            foreach (string fld in this.pk)
            {
                where.Add(string.Format("{0}=@w_{1}", db.escape(fld), ORMContext.fixName(fld)));
                object val = this.fields[fld].oldValue;
                if (val == null)  //if wasn't loaded from DB yet
                {
                    val = this.fields[fld].value;
                }
                p.Add(new DBParam("@w_" + ORMContext.fixName(fld), val));
            }
            atr.where = where.join(" and ");
        }
        
        public override bool update()
        {
            SQLStatement atr = new SQLStatement(this);
            DBParams p = new DBParams();
            this._setUpdateWhere(ref atr, ref p);
            return update(atr, p); 
        }
        /// <summary>
        /// you may want to give a custom where clause
        /// </summary>
        public bool update(SQLStatement atr) { return update(atr, null); }
        /// <summary>
        /// you may want to give a custom where clause
        /// </summary>
        public bool update(SQLStatement atr, DBParams p) 
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Update is not supported to Read Only TableRows");
            }

            if (!this.validate())
            {
                throw new TableValidationExceptions("Data could not be updated, there were validation errors:", this.dbObjectName.ToString(), this.validationExceptions);
            }

            if (atr.fields == null || atr.fields.Trim() == "")
            {
                EList<string> fields = getListOfFieldsToUpdate(p);
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
            if (atr.from == null || atr.from.Trim() == "") atr.from = this.dbObjectName.sqlFromName;

            
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

        public EList<string> getListOfFieldsToUpdate(DBParams p)
        {
            EList<string> fields = new EList<string>();

            var chaged = this.getFieldsValueChanged();
            foreach(GenericField f in chaged.valueUseDefaultValue)
            {
                fields.Add(string.Format("{0}=@u_{1}", this.db.escape(f.name), ORMContext.fixName(f.name)));
                //p.Add(new DBParam("@u_" + ORMContext.fixName(fld), this.fields[fld].defaultValue));
                p.Add(getDBParam(f.name, "@u_"));
                f.value = f.defaultValue;                    
            }
            foreach(GenericField f in chaged.valueChaged)
            {
                fields.Add(string.Format("{0}=@u_{1}", this.db.escape(f.name), ORMContext.fixName(f.name)));
                p.Add(getDBParam(f.name, "@u_"));               
            }
            
            return fields;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// DELETE
        //////////////////////////////////////////////////////////////////////////////////////////////
        
        public override bool delete() 
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Delete is not supported to Read Only TableRows");
            }

            SQLStatement atr = new SQLStatement(this);
            if (this.pk.Count == 0) { throw new ValidationException(string.Format("{0} can not be deleted using delete() because no Primary Key was defined for it. To delete use delete(SQLAttribute)", this.dbObjectName.sqlFromName)); }
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
        public bool delete(SQLStatement atr) { return delete(atr, null); }        
        public bool delete(SQLStatement atr, DBParams p) 
        {
            if (atr.from == null || atr.from.Trim() == "") atr.from = this.dbObjectName.sqlFromName;
            return this.db.delete(this, atr, p);      //ABSTRACT STORAGE     
        }

    }

}
