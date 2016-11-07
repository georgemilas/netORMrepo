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
    public class TableRowStoredProcBased : TableRowPersist
    {
        public TableRowStoredProcBased() : base() { }
        public TableRowStoredProcBased(ORMContext context)
            : base(context)
        {
                
        }

        public TableRowStoredProcBased(ORMContext context, TableRowStoredProcedures storedProcs)
            : base(context)
        {
            this.storedProcedures = storedProcs;
            this.storedProcedures.name = this.dbObjectName;
        }

        private TableRowStoredProcedures _storedProcedures;
        public TableRowStoredProcedures storedProcedures
        {
            get { return _storedProcedures; }
            set { _storedProcedures = value; }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////
        ////// TABE ROW customisation:
        ////////////////////////////////////////////////////////////////////////////////////////////////
        
        public override TableRowPersist getOneToOne(Type tp, DBRelation rel) 
        {
            throw new NotSupportedException();
        }
        public virtual T getOneToOne<T>(Type tp, DBRelation rel) where T : TableRowStoredProcBased
        {
            TableRow cls = this.getOneToOne(tp, rel);
            return (T)cls;
        }


        public override TablePersist<T> getOneToMany<T>(Type tp, DBRelation rel) 
        {
            throw new NotSupportedException();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////// COUNT
        //////////////////////////////////////////////////////////////////////////////////////////////

        public override int countAll()
        {
            string proc = this.storedProcedures.countAll.sqlFromName;
            return (int)this.db.db.executeScalar(proc, this.db.db.raise, null, CommandType.StoredProcedure);            
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
                //throw new TableValidationExceptions("Data could not be inserted, there were validation errors:", this.dbObjectName.ToString(), this.validationExceptions);
            }

            DBParams p = new DBParams();
            
            foreach (string fld in this.fields.Keys)
            {
                if (this.fields[fld].isIdentity) continue;
                if (this.fields[fld].isComputed) continue;
                //if ((this.fields[fld].value == null || this.fields[fld].value.ToString() == "") && this.fields[fld].defaultValue != null)
                //{
                //    p.Add(new DBParam("@" + ORMContext.fixName(fld), this.fields[fld].defaultValue ));
                //    //p.Add(getDBParam(fld);
                //    continue;
                //}
                p.Add(getDBParam(fld));               
            }

            object dbres = null;
            bool hasIdentity = false;
            foreach (string f in this.fields.Keys)  
            {
                if (this.fields[f].isIdentity)
                {
                    hasIdentity = true;
                    break;
                }
            }
                
            if ( hasIdentity ) 
            {
                dbres = this.db.db.executeScalar(this.storedProcedures.insert.sqlFromName, this.db.db.raise, p, CommandType.StoredProcedure);
            }
            else
            {
                dbres = this.db.db.executeQuery(this.storedProcedures.insert.sqlFromName, p, CommandType.StoredProcedure);
            }

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
        
        public override bool update()
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Update is not supported to Read Only TableRows");
            }

            if (!this.validate())
            {
                throw new TableValidationExceptions("Data could not be updated, there were validation errors:", this.dbObjectName.ToString(), this.validationExceptions);                
            }

            if (this.pk.Count == 0) { throw new ValidationException(string.Format("{0} can not be updated using update() because no Primary Key was defined for it. ", this.dbObjectName.sqlFromName)); }

            DBParams p = new DBParams();
            
            bool isUpdated = false;
            
            foreach (string fld in this.fields.Keys)
            {
                if (!this.pk.Contains(fld) && this.fields[fld].isIdentity) continue;
                if (this.fields[fld].isComputed) continue;

                if (this.pk.Contains(fld))
                {
                    //with stored proc version we don't support stuff like 
                    //update table_1 set id = 5 where id = 3 but with dynamic sql we do 
                    //so we will not use oldValue for the where clause anymore like we do in the dynamic sql version 
                    p.Add(new DBParam("@" + ORMContext.fixName(fld), this.fields[fld].value));
                    continue;
                }

                if (!isUpdated)
                {
                    //modified or not we have to send it
                    if (this.fields[fld].value != null && this.fields[fld].oldValueSafe == null) { isUpdated = true; }
                    if (this.fields[fld].value == null && this.fields[fld].oldValueSafe != null) { isUpdated = true; }
                    if (this.fields[fld].value != null && this.fields[fld].oldValueSafe != null && this.fields[fld].value != this.fields[fld].oldValue) { isUpdated = true; }
                }

                if ( this.fields[fld].value == null && this.fields[fld].defaultValue != null )
                {
                    //p.Add(new DBParam("@" + ORMContext.fixName(fld), this.fields[fld].defaultValue));
                    p.Add(getDBParam(fld));
                    this.fields[fld].value = this.fields[fld].defaultValue;
                    continue;
                }                
                p.Add(getDBParam(fld));
            }
            
            
            if (!isUpdated)
            {
                return true;    //nothing changed so nothing to do
            }
            
            
            bool res = this.db.db.executeQuery(this.storedProcedures.updateByPK.sqlFromName, p, CommandType.StoredProcedure);   
            
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
        
        public override bool delete() 
        {
            if (this.isReadOnly)
            {
                throw new ORMException("Delete is not supported to Read Only TableRows");
            }

            if (this.pk.Count == 0) { throw new ValidationException(string.Format("{0} can not be deleted using delete() because no Primary Key was defined for it. ", this.dbObjectName.sqlFromName)); }
            DBParams p = new DBParams();
            foreach(string fld in this.pk)
            {
               p.Add(new DBParam("@"+ORMContext.fixName(fld), this.fields[fld].value));
            }
            return this.db.db.executeQuery(this.storedProcedures.deleteByPK.sqlFromName, p, CommandType.StoredProcedure); 
        }
        

    }

}
