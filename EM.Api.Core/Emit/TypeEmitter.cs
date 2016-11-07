using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.Http;

namespace EM.Api.Core.Emit
{
    public class TypeEmitter
    {
        public ModuleEmitter ModuleEmmiter { get; set; }
        public TypeBuilder TypeBuilder { get; set; }
        public TypeEmitterCacheKey CacheKey { get; set; }
        public Type Type { get; set; }  //holds the actual emitted type
        public List<AdditionalProperty> AdditionalPropertiesValueProviders { get; set; }
        public List<Action> OnAfterEmit { get; set; }

        public TypeEmitter()
        {
            AdditionalPropertiesValueProviders = new List<AdditionalProperty>();
            OnAfterEmit = new List<Action>();
        }
        public void BuildProperty(string name, Type tp)
        {
            ModuleEmmiter.CodeEmiter.BuildProperty(TypeBuilder, name, tp);
        }
        public void EmitType()
        {            
            Type = TypeBuilder.CreateType();
            foreach (var action in OnAfterEmit)
            {
                action();
            }
            OnAfterEmit = new List<Action>();            
        }
    }


    public class ModuleEmitter
    {
        public CodeEmiter CodeEmiter { get; set; }
        public string ModulePrefix { get; set; }
        public AssemblyName AssemblyName { get; set; }
        public string DllName
        {
            get { return AssemblyName.Name + ".dll"; }
        }
        public AssemblyBuilder AssemblyBuilder { get; set; }
        public ModuleBuilder ModuleBuilder { get; set; }
        
        public override int GetHashCode()
        {
            var hash = (ModulePrefix != null ? ModulePrefix.GetHashCode() : 0) |
                       (AssemblyName != null ? AssemblyName.GetHashCode() : 0) |
                       (AssemblyBuilder != null ? AssemblyBuilder.GetHashCode() : 0) |
                       (ModuleBuilder != null ? ModuleBuilder.GetHashCode() : 0);                       
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ModuleEmitter)
            {
                var other = obj as ModuleEmitter;
                return ModulePrefix == other.ModulePrefix && AssemblyName == other.AssemblyName && AssemblyBuilder == other.AssemblyBuilder && ModuleBuilder == other.ModuleBuilder;
            }
            return false;
        }
        
        public TypeEmitterReturn GetTypeEmmiter(TypeEmitterCacheKey key)
        {
            return CodeEmiter.GetTypeEmitter(key);            
        }
        
        public void SaveAssembly()
        {
            //Save the assembly so it can be examined with Ildasm.exe, or referenced by a test program, etc..
            AssemblyBuilder.Save(DllName);
        }
    }

    public class TypeEmitterCacheKey  
    {
        public ModuleEmitter ModuleEmitter { get; set; }
        public string Name { get; set; }
        public virtual string FriendlyName { get { return Name; } }

        public override int GetHashCode()
        {
            var hash = (ModuleEmitter != null ? ModuleEmitter.GetHashCode() : 0) | 
                       (Name != null ? Name.GetHashCode() : 0);
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeEmitterCacheKey)
            {
                var other = obj as TypeEmitterCacheKey;
                return ModuleEmitter == other.ModuleEmitter && Name == other.Name;
            }
            return false;
        }
    }

}