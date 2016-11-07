using System;
using System.Collections.Generic;
using System.Text;
using EM.DB;
using ORM.db_store.persitence;
using EM.Collections;

namespace ORM.db_store
{
    [Serializable]
    public class TableName: IComparable
    {
        public string table { get; set; }
        public string schema { get; set; }
        public string catalog { get; set; }

        /// <summary>
        /// how to name a property accesor for a field in the table 
        ///     - for example there is a field called "string" but we can't name something "string" in C#
        /// </summary>
        public OrderedDictionary<string, string> customFieldPropertyName { get; set; }

        /// <summary>
        /// by default type gets assigned by doing reflection in database but in case you want to overrite
        /// (for example the database might say Nullable&lt;Int32&gt; but we want Int32 instead)
        /// </summary>
        public OrderedDictionary<string, string> customFieldPropertyType { get; set; }


        /// <summary>
        /// how to name a property accesor for a field in the table 
        ///     - for example there is a field called "string" but we can't name something "string" in C#
        /// </summary>
        public virtual string getCustomFieldPropertyName(string fldName)
        {
            if (customFieldPropertyName == null)
            {
                return ORMContext.fixName(fldName);
            }
            
            if (customFieldPropertyName.ContainsKey(fldName))
            {
                return customFieldPropertyName[fldName];
            }
            else 
            {
                return ORMContext.fixName(fldName);
            }
        }

        
        protected string _sqlFrom;  //example: schema.name  (SD_BILL.tbl_cdr)  or something else if the user assigns it 
        protected ORMContext _context;

        #region contructors

        public TableName() : base() { this.customFieldPropertyType = new OrderedDictionary<string, string>(); }

        public TableName(string catalog, string schema, string name)
        {
            this.table = name;
            this.catalog = catalog;
            this.schema = schema;
            this._sqlFrom = null;
            this.customFieldPropertyType = new OrderedDictionary<string, string>();
        }
        public TableName(string catalog, string schema, string name, string className)
            : this(catalog,schema,name)
        {
            this._className = className;
        }

        public TableName(string catalog, string schema, string name, string className, string _namespace)
            : this(catalog, schema, name)
        {
            this._className = className;
            this.classNamespace = _namespace;
        }

        public TableName(string catalog, string schema, string name, string className, string _namespace, string interfaceName)
            : this(catalog, schema, name, className,_namespace)
        {
            this.interfaceName = interfaceName;
        }

        #endregion contructors

        private static void nameAliasPair(string nameWithAlias, out string name, out string alias)
        {
            nameWithAlias = nameWithAlias.Trim();

            if (nameWithAlias.Contains("]"))
            {
                name = nameWithAlias.Substring(0, nameWithAlias.IndexOf("]")+1).Trim();
                alias = nameWithAlias.Replace(name, "").Trim();
            }
            else
            {
                string[] parts = nameWithAlias.Split(new char[] { ' ' });
                if (parts.Length == 2)
                {
                    name = parts[0].Trim();
                    alias = parts[1].Trim();
                }
                else
                {
                    name = parts[0].Trim();
                    alias = null;
                }
            }
        }
        public static TableName fromDotString(string tn) { return fromDotString(tn, null, null); }
        public static TableName fromDotString(string tn, string interfaceName) { return fromDotString(tn, interfaceName, null); } 
        public static TableName fromDotString(string tn, string interfaceName, string namespaceName) 
        {
            string[] parts = tn.Split(new char[] {'.'});
            string name = null;
            string alias = null;
            TableName t;
            
            if (parts.Length == 3)
            {
                nameAliasPair(parts[2], out name, out alias);
                t = new TableName(parts[0].Trim(), parts[1].Trim(), name);
            }
            else if (parts.Length == 2)
            {
                nameAliasPair(parts[1], out name, out alias);
                t = new TableName(null, parts[0].Trim(), name);                
            }
            else 
            {
                nameAliasPair(tn, out name, out alias);
                t = new TableName(null, null, name);                
            }

            if (alias != null && alias.Trim() != "")
            {
                t.sqlFromName = alias.Trim();
            }

            if (interfaceName != null) { t.interfaceName = interfaceName; }
            if (namespaceName != null) { t.classNamespace = namespaceName; }
            return t;
        }


        /// <summary>
        /// By default is [schema].[name]  or just [name] if no schema is given
        ///   - assign it manualy to skip the rules and use something else for example a table alias
        ///   - assign null to use the default rules again
        /// </summary>
        public string sqlFromName
        {
            get
            {
                if (this._sqlFrom == null)
                {
                    if (this.schema != null)
                    {
                        try 
                        {
                            this._sqlFrom = this.db.escape(this.schema) + "." + this.db.escape(this.table);
                        }
                        catch
                        {
                            this._sqlFrom = this.schema + "." + this.table;
                        }
                    }
                    else
                    {
                        try
                        {
                            this._sqlFrom = this.db.escape(this.table);
                        }
                        catch
                        {
                            this._sqlFrom = this.table;
                        }
                    }
                }
                return this._sqlFrom;
            }
            set { this._sqlFrom = value; }
        }

        public ORMContext context
        {
            get
            {
                if (_context == null)
                {
                    this._context = new ORMContext();
                }
                return _context;
            }
            set { this._context = value; }
        }

        public GenericDatabase db
        {
            get
            {
                return this.context.db;
            }
        }

        public bool useCustomClassName { get; set; }
        protected string _className;
        public string className
        {
            get 
            {
                if (_className == null)
                {
                    return ORMContext.fixName(this.table);
                }
                return _className;
            }
            set { this._className = value; }
        }

        private string _classNamespace;
        public string classNamespace
        {
            get { return _classNamespace; }
            set { _classNamespace = value; }
        }

        protected string _interfaceName;
        public string interfaceName
        {
            get
            {
                if (_interfaceName == null && className != null)
                {
                    _interfaceName = "I" + className;
                }
                return _interfaceName;
            }
            set { this._interfaceName = value; }
        }

        public TableRow classInstance
        {
            get
            {
                return this.context.classFactory.getInstance(this, this.context);
            }
        }
        

        public int CompareTo(object other)
        {
            //leave classNamespace alone, it's only used by ClassFactory at runtime 
            //and we compare at "compile-time aka generation-time"
            TableName tn = (TableName)other;
            // don't also do this.interfaceName == tn.interfaceName  because they should be alowed to be diferent
            if (this.catalog == tn.catalog && this.schema == tn.schema && this.table == tn.table) return 0;
            return 1;
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        /*
        public static bool operator ==(TableName t1, TableName t2)
        {
            return t1.Equals(t2);
        }
        public static bool operator !=(TableName t1, TableName t2)
        {
            return !t1.Equals(t2);
        }
        */

        public override int GetHashCode()
        {
            // don't also do  "| this.interfaceName.GetHashCode()" because they should be alowed to be diferent
            return this.catalog.GetHashCode() | this.schema.GetHashCode() | this.table.GetHashCode(); // | this.className.GetHashCode();    //faster then above
        }

        public override string ToString()
        {
            return this.sqlFromName;
        }

    }
}
