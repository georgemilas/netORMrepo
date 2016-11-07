using EM.Api.Core.OData;
using System;

namespace EM.Api.Core.Emit
{
    public class ApiObjectTypeEmitterCacheKey : TypeEmitterCacheKey
    {
        public static ApiObjectTypeEmitterCacheKey GetApiObjectTypeEmitterCacheKey(ModuleEmitter em, Type tp, SelectExpandResult se, TypeEmitter parrentTypeEmitter, bool useHierachicalNames = true)
        {
            var key = new ApiObjectTypeEmitterCacheKey(em, tp, se, parrentTypeEmitter, useHierachicalNames);
            if (!useHierachicalNames)
            {
                return key;
            }
            var origKey = key.GetCircularReferenceType();
            return origKey ?? key;
        }

        private ApiObjectTypeEmitterCacheKey(ModuleEmitter em, Type tp, SelectExpandResult se, TypeEmitter parrentTypeEmitter, bool useHierachicalNames)
        {
            CacheType = tp;
            ParrentTypeEmitter = parrentTypeEmitter;
            SelectExpandResult = se;
            ModuleEmitter = em;
            
            string parentName = "";
            string friendlyParentName = "";
            if (useHierachicalNames && parrentTypeEmitter != null && parrentTypeEmitter.CacheKey != null)
            {
                parentName = "|" + parrentTypeEmitter.CacheKey.Name;
                friendlyParentName = parrentTypeEmitter.CacheKey.FriendlyName + "_";                
            }
            var name = tp.FullName + "|" + em.ModulePrefix + "|" + se.GetHashString() + "|" + se.GetHashCode();
            //Name = SPMediaFormatter.ToCLSCompliantIdentifier(name);
            Name = name + parentName;
            _friendlyName = friendlyParentName + tp.Name + "_" + Guid.NewGuid().ToString("N").ToUpper();
        }

        private string _friendlyName;
        public override string FriendlyName { get { return _friendlyName; } }
        public Type CacheType { get; set; }
        public TypeEmitter ParrentTypeEmitter { get; set; }
        public SelectExpandResult SelectExpandResult { get; set; }

        public ApiObjectTypeEmitterCacheKey GetCircularReferenceType()
        {
            return GetCircularReferenceType(ParrentTypeEmitter);
        }
        private ApiObjectTypeEmitterCacheKey GetCircularReferenceType(TypeEmitter tpEmitter)
        {
            //search up the parent hierarchy and see if we have same name
            //like Adress in Employee.Address.Person.Monther.Address
            if (tpEmitter != null && tpEmitter.CacheKey != null)
            {
                if (tpEmitter.CacheKey is ApiObjectTypeEmitterCacheKey)
                {
                    var parent = tpEmitter.CacheKey as ApiObjectTypeEmitterCacheKey;
                    if (parent.CacheType == CacheType)
                    {
                        return parent;
                    }
                    if (parent.ParrentTypeEmitter != null)
                    {
                        return GetCircularReferenceType(parent.ParrentTypeEmitter);
                    }
                }                    
            }
            return null;
        }

        public override int GetHashCode()
        {
            var hash =  (ModuleEmitter != null ? ModuleEmitter.GetHashCode() : 0) | 
                        (Name != null ? Name.GetHashCode() : 0) | 
                        (SelectExpandResult != null ? SelectExpandResult.GetHashCode() : 0);
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj is ApiObjectTypeEmitterCacheKey)
            {
                var other = obj as ApiObjectTypeEmitterCacheKey;
                var eq = ModuleEmitter == other.ModuleEmitter && 
                         Name == other.Name && 
                         SelectExpandResult.Equals(other.SelectExpandResult);
                return eq;
            }
            return false;
        }
    }
}