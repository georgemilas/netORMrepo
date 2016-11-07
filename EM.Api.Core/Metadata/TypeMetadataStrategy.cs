using System;
using System.Collections.Generic;

namespace EM.Api.Core.Metadata
{
    /// <summary>
    /// contract for metadata discovery 
    /// </summary>
    public abstract class TypeMetadataStrategy
    {
        public Type Type { get; set; }

        protected TypeMetadataStrategy(Type type)
        {
            Type = type;
        }

        public abstract IEnumerable<PropertyMetadata> GetProperties();
        /// <summary>
        /// Get class metadata
        /// </summary>
        public abstract T GetCustomAttribute<T>() where T : Attribute;
        /// <summary>
        /// Get Property metadata
        /// </summary>
        public abstract T GetCustomAttribute<T>(PropertyMetadata p) where T : Attribute;
    }
}