using System;
using System.Collections.Generic;
using System.Reflection;

namespace EM.Api.Core.Metadata
{
    /// <summary>
    /// Object configuration using reflection for [Attribute] annotations  
    /// </summary>
    public class ReflectionBasedTypeMetadataStrategy : TypeMetadataStrategy
    {
        /// <summary>
        /// Define object configuration for objects of type "type" using reflection for [Attribute] annotation on the type "type" 
        /// </summary>
        public ReflectionBasedTypeMetadataStrategy(Type type) : base(type) { }

        public override IEnumerable<PropertyMetadata> GetProperties()
        {
            foreach (var p in Type.GetProperties())
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

        public override T GetCustomAttribute<T>()
        {
            return Type.GetCustomAttribute<T>();
        }

        public override T GetCustomAttribute<T>(PropertyMetadata p)
        {
            return ((PropertyInfo) p.Actual).GetCustomAttribute<T>();
        }

    }
}