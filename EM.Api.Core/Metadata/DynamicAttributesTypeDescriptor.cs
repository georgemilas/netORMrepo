using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EM.Api.Core.Metadata
{
    /// <summary>
    /// Holds custom attribtes for a particular type T as well as for properties of T
    /// </summary>
    public class DynamicAttributesTypeDescriptor<T> : CustomTypeDescriptor
    {
        private readonly Dictionary<string, PropertyDescriptor> newReplacementProperties = new Dictionary<string, PropertyDescriptor>();
        private readonly List<Attribute> classAttributes = new List<Attribute>();
        private Type type { get; set; }

        public DynamicAttributesTypeDescriptor(): base(TypeDescriptor.GetProvider(typeof(T)).GetTypeDescriptor(typeof(T)))
        {
            type = typeof(T);
        }

        public void AddPropertyAttributes(string propertyName, params Attribute[] attributes)
        {
            var pd = TypeDescriptor.GetProperties(type).Find(propertyName, false);
            var pd2 = TypeDescriptor.CreateProperty(type, pd, attributes);
            AddReplacementProperty(pd2);            
        }
        public void AddReplacementProperty(PropertyDescriptor pd)
        {
            newReplacementProperties[pd.Name] = pd;
        }

        public void AddClassAttributes(params Attribute[] attributes)
        {
            classAttributes.AddRange(attributes);            
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            object o = base.GetPropertyOwner(pd);
            return o ?? this;            
        }

        private PropertyDescriptorCollection GetProperties(PropertyDescriptorCollection pdc)
        {
            var props = from PropertyDescriptor pd in pdc
                        select newReplacementProperties.ContainsKey(pd.Name) ? newReplacementProperties[pd.Name] : pd;
            return new PropertyDescriptorCollection(props.ToArray());
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(base.GetProperties());
        }
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties(base.GetProperties(attributes));
        }

        public override AttributeCollection GetAttributes()
        {
            return new AttributeCollection(classAttributes.ToArray());
        }

    }
}