using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ORM.exceptions;
using ORM.db_store;

namespace ORM
{
    [Serializable]
    /// <summary>
    /// Finds calsses in calling Assembly
    /// </summary>
    public class BasicClassFactory: IClassFactory
    {
        public virtual TableRow getInstance(TableName table, ORMContext context)
        {
            TableRow cls = this.getInstance(table.className, table.classNamespace, context);
            return cls;
            
        }
        public virtual TableRow getInstance(string className, string _namespace, ORMContext context)
        {
            return this.getInstance(className, _namespace, context, Assembly.GetCallingAssembly());
        }
        public virtual TableRow getInstance(string className, string _namespace, ORMContext context, Assembly a)
        {
            Type tp = null;
            foreach (Type t in a.GetTypes())
            {
                if (t.Name == ORMContext.fixName(className) && (_namespace == null || t.Namespace.EndsWith(_namespace)) )
                {
                    tp = t;
                    break;
                }
            }
            if (tp == null) throw new ORMException(string.Format("no class with name of {0} was found", className));
            TableRow cls = (TableRow)Activator.CreateInstance(tp, context);
            //cls.db = db;
            return cls;
        }

        public virtual TableRow getInstance(Type tp, ORMContext context)
        {
            TableRow cls = (TableRow)Activator.CreateInstance(tp, context);
            return cls;
        }
    }

}

