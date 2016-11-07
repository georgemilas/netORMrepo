using System;
using System.ComponentModel;

namespace EM.Api.Core.Metadata
{
    /// <summary>
    /// Type Descriptor Provider using custom type descriptors (such as DynamicAttributesTypeDescriptor)
    /// </summary>
    public class AttributesProviderTypeDescriptor : TypeDescriptionProvider
    {
        private readonly ICustomTypeDescriptor ctd;

        public AttributesProviderTypeDescriptor(ICustomTypeDescriptor ctd)
        {
            this.ctd = ctd;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return ctd;
        }
    }
}