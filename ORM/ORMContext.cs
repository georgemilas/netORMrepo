using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EM.Collections;
using EM.DB;
using ORM.db_store.persitence;

namespace ORM
{
    [Serializable]
    public class ORMContext : IORMContext, IDisposable
    {
        public ORMContext()
        {
            this.UseConnectionPerCommand = true;
        }

        /////////////////////////////////////////////////////////////////////////////////
        /////////////// public interface
        /////////////////////////////////////////////////////////////////////////////////
        
        private bool _useConnectionPerCommand = true;
        public bool UseConnectionPerCommand  
        { 
            get { return _useConnectionPerCommand; }
            set { this._useConnectionPerCommand = value; }
        }

        protected GenericDatabase _db;
        public virtual GenericDatabase db
        {
            get 
            {
                if (this._db == null)
                {
                    if (this.UseConnectionPerCommand)
                    {
                        string con = System.Configuration.ConfigurationManager.AppSettings["CONN_STR"];
                        var dbe = new SqlServerDBWorker(con);
                        this._db = new SQLServerDatabase(dbe);
                    }
                    else
                    {
                        string con = System.Configuration.ConfigurationManager.AppSettings["CONN_STR"];
                        var dbe = new SqlServerDBWorkerOneConnection(con);
                        this._db = new SQLServerDatabase(dbe);
                    }
                }
                
                return this._db;

            }
            set 
            {
                //if (this._db == null) { this._db = value; }     //don't let multiple threads to override this                                
                this._db = value;
            }
        }

        
        public void StartTransaction() 
        {
            if (this.db.db.currentTransaction == null)
            {
                this.db.db.currentTransaction = this.db.db.startTransaction();
            }
            else
            {
                throw new InvalidOperationException("A new transaction can not be started, there is already a current transaction associated with the object.");
            } 
        }
        public void StartTransaction(IsolationLevel level)
        {
            if (this.db.db.currentTransaction == null)
            {
                this.db.db.currentTransaction = this.db.db.startTransaction(level);
            }
            else
            {
                throw new InvalidOperationException("A new transaction can not be started, there is already a current transaction associated with the object.");
            }
        }

        private object _tranlock = new object();
        public void CommitCurrentTransaction()
        {
            lock (_tranlock)
            {
                if (this.db.db.currentTransaction != null)
                {
                    try { this.db.db.currentTransaction.Commit(); }
                    //catch { this.db.db.currentTransaction = null; }
                    finally { this.db.db.currentTransaction = null; }
                }
            }
        }
        public void RollbackCurrentTransaction()
        {
            if (this.db.db.currentTransaction != null)
            {
                lock (_tranlock)
                {
                    try { this.db.db.currentTransaction.Rollback(); }
                    catch { this.db.db.currentTransaction = null; }
                    finally { this.db.db.currentTransaction = null; }
                }
            }
        }

        public IClassFactory classFactory { get; set; }
    
        ////////////////////////////////////////////////////////////////////////////////////////
        //////// STATIC
        public static string fixName(string name)
        {
            return name.Replace(" ", "");
        }

        public static string camelCaseName(string name)
        {
            string nm = name.Replace("  ", " ");
            nm = nm.Replace("\t", " ");
            string[] parts = nm.Split(new char[] { '_', ' ' });
            StringBuilder res = new StringBuilder();
            foreach (string part in parts)
            {
                bool wasLow = true;
                bool atStart = true;
                StringBuilder camelizedPart = new StringBuilder();
                foreach (char c in part.ToCharArray())
                {
                    if (char.IsUpper(c))
                    {
                        if (wasLow) 
                        { 
                            camelizedPart.Append(c); 
                            wasLow = false; 
                        }
                        else 
                        { 
                            camelizedPart.Append(char.ToLower(c));
                            //wasLow = true; 
                        }                        
                    }
                    else
                    {
                        if (atStart)
                        {
                            camelizedPart.Append(char.ToUpper(c));
                            wasLow = false;
                        }
                        else
                        {
                            camelizedPart.Append(c);
                            wasLow = true; 
                        }
                    }
                    atStart = false;
                }

                res.Append(camelizedPart.ToString());
            }
            return res.ToString();
        }

        public static string GetMethodName_ParamsPart(List<string> fields)
        {
            EList<string> nameLst = new EList<string>();
            foreach (string f in fields)
            {
                nameLst.Add(ORMContext.camelCaseName(f));
            }
            string name = nameLst.join("_");
            return name;
        }


        public void Dispose()
        {
            if (this._db != null)
            {
                this._db.Dispose();
            }            
        }

    }
}
