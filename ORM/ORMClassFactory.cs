using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ORM
{
    [Serializable]
    /// <summary>
    /// Finds classes in the given Assembly
    /// </summary>
    public class ORMClassFactory : BasicClassFactory
    {
        [NonSerialized]
        public Assembly assembly;

        public ORMClassFactory() : base() { }

        public ORMClassFactory(Assembly assembly)
        {
            this.assembly = assembly;
        }
        public override TableRow getInstance(string className, string _namespace,  ORMContext context)
        {
            return this.getInstance(className, _namespace, context, this.assembly);
        }
    }
}
