using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Xml;

namespace EM.Api.Core.Emit.Serialize
{
    public interface ICustomSerializerTypeResolver
    {
        void ResolveType(Type tpEmit);
        //void ResolveType(TypeEmitter tpEmit);        
    }
}