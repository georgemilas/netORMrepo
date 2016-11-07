using System;

namespace EM.Api.Core.Metadata
{
    /// <summary>
    /// A wrapper for PropertyInfo / PropertyDescryptor depending on TypeMetadataStrategy where 
    /// the Actual property contains the reference to PropertyInfo / PropertyDescryptor
    /// </summary>
    public class PropertyMetadata
    {
        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public object Actual { get; set; }
        public TypeMetadataStrategy TypeMetadataProvider { get; set; }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            return TypeMetadataProvider.GetCustomAttribute<T>(this);
        }
    }
}