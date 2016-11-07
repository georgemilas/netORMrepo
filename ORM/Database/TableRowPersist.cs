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
    public abstract class TableRowPersist : TableRow
    {
        public TableRowPersist() : base() { }
        public TableRowPersist(ORMContext context) : base(context)
        {            
        }

        public static new TablePersist<T> getInstancesFromDataTable<T>(ORMContext cx, DataTable tb) where T : TableRowPersist
        {
            TablePersist<T> res = new TablePersist<T>();
            res.db = cx.db;
            res.adoDataTable = tb;
            foreach (DataRow row in tb.Rows)
            {
                //TableRowPersist cls = (TableRowPersist)this.getInstance();
                T cls = (T)Activator.CreateInstance(typeof(T), cx);
                cls.setFromDataRow(row, tb.Columns);
                res.Add(cls);
            }
            return res;
        }
        public virtual new TablePersist<T> getInstancesFromDataTable<T>(DataTable tb) where T : TableRowPersist
        {
            return TableRowPersist.getInstancesFromDataTable<T>(this.context, tb);            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////// RELATIONS:
        //////////////////////////////////////////////////////////////////////////////////////////////

        //you may want to use this to create some real properties {get; set;} 
        //with a cache value in order to only query DB once

        public virtual DBRelation getRelationFromType(Type tp)
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
            if (rel == null) throw new ORMException(string.Format("no foreign key was defined in {0} for {1}", this.dbObjectName.sqlFromName, tp.FullName));
            return rel;
        }

        public virtual Type geTypeFromRelation(DBRelation rel)
        {
            string clsName = this.GetType().Namespace + "." + rel.tableThere.className;
            //ObjectHandle oh = Activator.CreateInstance(null, clsName);
            //Type tp = oh.GetType();
            Type tp = Type.GetType(clsName);
            return tp;
        }


        /// <summary>
        /// Ex:  cust c = cust();
        ///      cust_adr ca = (cust_adr)c.getOneToOne(typeof(cust_adr));
        ///      cust_adr ca = (cust_adr)c.getOneToOne(this.fk[0]);
        /// </summary>
        public virtual TableRowPersist getOneToOne(Type tp) { return getOneToOne(tp, getRelationFromType(tp)); }
        public virtual TableRowPersist getOneToOne(DBRelation rel) { return getOneToOne(geTypeFromRelation(rel), rel); }
        public abstract TableRowPersist getOneToOne(Type tp, DBRelation rel);
        
        /// <summary>
        /// Ex:  cust c = cust();
        ///      EList&lt;TableRow> calst = (cust_adr)c.getOneToMany(typeof(cust_adr));
        ///      EList&lt;TableRow> calst = (cust_adr)c.getOneToMany(this.fk[0]);
        ///      foreach(TableRow t in calst) 
        ///      {
        ///          cust_adr ca = (cust_adr)t;
        ///      }
        /// </summary>
        public virtual TablePersist<T> getOneToMany<T>(Type tp) where T : TableRowPersist { return getOneToMany<T>(tp, getRelationFromType(tp)); }
        public virtual TablePersist<T> getOneToMany<T>(DBRelation rel) where T : TableRowPersist { return getOneToMany<T>(geTypeFromRelation(rel), rel); }
        public abstract TablePersist<T> getOneToMany<T>(Type tp, DBRelation rel) where T : TableRowPersist;
        
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// DATA STORAGE
        //////////////////////////////////////////////////////////////////////////////////////////////
        
        public abstract int countAll();
        
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
            bool isInsert = IsSaveInsert();
            if (isInsert) return insert();
            return update();
        }

        public bool IsSaveInsert()
        {
            bool isInsert = true;
            foreach (GenericField f in this.fields.Values)
            {
                if (f.oldValue != null)  //oldValueSafe may be null even if loaded from DB
                {
                    isInsert = false;
                    break;
                }
            }
            return isInsert;
        }

        public abstract int insert(); 
        public abstract bool update();       
        public abstract bool delete();        

    }

}
