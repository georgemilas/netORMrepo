using System;
using System.Linq;

namespace EM.Api.Core.Emit
{
    /// <summary>
    /// Represents an object hierarchy used by PropertyContext of AdditionalAttributes of ApiObjects<para></para> 
    /// such that in for example Payroll/Employees[ix]/Address, the address object knows for what employee and for what payroll   
    /// </summary>
    public class ObjectAndParent
    {
        public object Object { get; set; }
        public ObjectAndParent Parent { get; set; }
        public ObjectAndParent(Object obj) { Object = obj; }
        public ObjectAndParent(Object obj, ObjectAndParent parent) { Object = obj; Parent = parent; }

        /// <summary>
        /// The Object, 
        /// it's parent, 
        /// it's parent parent,
        /// it's parent parent parent ...
        /// </summary>
        public ObjectAndParent(Object obj, params Object[] parents)
        {
            Object = obj;
            if (parents.Length == 1)
            {
                Parent = new ObjectAndParent(parents[0]);
            }
            else if (parents.Length > 1)
            {
                Parent = new ObjectAndParent(parents[0], parents.Skip(1).ToArray());
            }
        }
    }
}