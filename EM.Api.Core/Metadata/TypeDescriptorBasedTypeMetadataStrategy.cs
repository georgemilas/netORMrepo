using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EM.Api.Core.Metadata
{
    public class TypeDescriptorBasedTypeMetadataStrategy : TypeMetadataStrategy
    {
        public TypeDescriptorBasedTypeMetadataStrategy(Type type) : base(type) { }

        public override IEnumerable<PropertyMetadata> GetProperties()
        {
            foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(Type))
            {
                yield return new PropertyMetadata()
                {
                    Name = p.Name,
                    PropertyType = p.PropertyType,
                    Actual = p,
                    TypeMetadataProvider = this
                };
            }
        }

        private T GetAttribute<T>(AttributeCollection attrs) where T : Attribute
        {
            foreach (Attribute attr in attrs)
            {
                if (attr is T)
                {
                    return (T) attr;
                }
            }
            return null;
        }

        /// <summary>
        /// Get class metadata 
        /// </summary>
        public override T GetCustomAttribute<T>()
        {
            var attrs = TypeDescriptor.GetAttributes(Type);
            return GetAttribute<T>(attrs);
        }

        /// <summary>
        /// Get ptoperty p metadata
        /// </summary>
        public override T GetCustomAttribute<T>(PropertyMetadata p)
        {
            var attrs = ((PropertyDescriptor) p.Actual).Attributes;
            return GetAttribute<T>(attrs);
        }
    }
}