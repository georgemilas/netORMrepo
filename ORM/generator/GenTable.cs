using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using EM.DB;
using EM.Collections;
using ORM;
using ORM.DBFields;
using System.Data;
using System.IO;
using ORM.db_store;
using System.Reflection;
using ORM.db_store.persitence;
using ORM.exceptions;

namespace ORM.generator
{
    public class GenTable : TableName
    {
        public ESet<DBConstraint> customLoad { get; set; }
        public ESet<DBConstraint> customSelect { get; set; }
        public ESet<DBConstraint> customDelete { get; set; }

        public GenTable(string dotString) 
        {
            TableName tn = TableName.fromDotString(dotString);
            this.catalog = tn.catalog;
            this.schema = tn.schema;
            this.table = tn.table;
            this.className = tn.className;
            this.interfaceName = tn.interfaceName;
            this.classNamespace = tn.classNamespace;            
        }
        

        public static GenTable fromDotString(string tn) { return fromDotString(tn, null, null); }
        public static GenTable fromDotString(string tn, string interfaceName) { return fromDotString(tn, interfaceName, null); }
        public static GenTable fromDotString(string tn, string interfaceName, string namespaceName)
        {
            GenTable t = new GenTable(tn);
            if (interfaceName != null) { t.interfaceName = interfaceName; }
            if (namespaceName != null) { t.classNamespace = namespaceName; }
            return t;
        }
        
    }

}
