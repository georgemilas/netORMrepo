using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;

namespace EM.Api.Core.Emit
{

    public class TypeEmitterReturn
    {
        public bool InCache { get; set; }
        public TypeEmitter TypeEmitter { get; set; }
    }


    //https://msdn.microsoft.com/en-us/library/vstudio/system.reflection.emit.propertybuilder(v=vs.100).aspx
    public class CodeEmiter
    {
        public string Name { get; private set; } 
        public CodeEmiter(string name)
        {
            Name = name;
        }
        private object moduleLockObj = new object();
        private object typeLockObj = new object();

        //public static ConcurrentDictionary<string, ModuleEmitter> ModuleEmitterCache = new ConcurrentDictionary<string, ModuleEmitter>();
        public ConcurrentDictionary<TypeEmitterCacheKey, TypeEmitter> TypeEmitterCache = new ConcurrentDictionary<TypeEmitterCacheKey, TypeEmitter>();

        private ModuleEmitter _ModuleEmitter;
        public ModuleEmitter ModuleEmitter
        {
            get
            {
                if (_ModuleEmitter == null)
                {
                    lock (moduleLockObj)
                    {
                        //if (ModuleEmitterCache.ContainsKey(name))
                        //{
                        //    return ModuleEmitterCache[name];
                        //}
                        ModuleEmitter em = new ModuleEmitter();
                        AppDomain domain = Thread.GetDomain();
                        em.CodeEmiter = this;
                        em.ModulePrefix = Name;
                        em.AssemblyName = new AssemblyName { Name = Name + "Assembly" + Guid.NewGuid().ToString("N") };
                        em.AssemblyBuilder = domain.DefineDynamicAssembly(em.AssemblyName, AssemblyBuilderAccess.RunAndSave);
                        em.ModuleBuilder = em.AssemblyBuilder.DefineDynamicModule(em.AssemblyName.Name, em.DllName);
                        //ModuleEmitterCache[name] = em;
                        _ModuleEmitter = em;
                    }
                }
                return _ModuleEmitter;
            }
        }

        public TypeEmitterReturn GetTypeEmitter(TypeEmitterCacheKey key)
        {
            lock (typeLockObj)
            {
                if (TypeEmitterCache.ContainsKey(key))
                {
                    return new TypeEmitterReturn() { InCache = true, TypeEmitter = TypeEmitterCache[key] };                    
                }
                var typeBuilder = key.ModuleEmitter.ModuleBuilder.DefineType(key.FriendlyName, TypeAttributes.Public);
                var res = new TypeEmitter() { ModuleEmmiter = key.ModuleEmitter, TypeBuilder = typeBuilder, Type = null, CacheKey = key };
                TypeEmitterCache[key] = res;
                return new TypeEmitterReturn(){ InCache = false, TypeEmitter = res};
            }
        }

        public void BuildProperty(TypeBuilder typeBuilder, string name, Type tp)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + name, tp, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, tp, null);
            MethodAttributes getSetAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + name, getSetAttributes, tp, Type.EmptyTypes);
            ILGenerator iLBuilderGetMethod = getMethodBuilder.GetILGenerator();
            iLBuilderGetMethod.Emit(OpCodes.Ldarg_0);
            iLBuilderGetMethod.Emit(OpCodes.Ldfld, fieldBuilder);
            iLBuilderGetMethod.Emit(OpCodes.Ret);

            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + name, getSetAttributes, null, new Type[] { tp });
            ILGenerator iLBuilderSetMethod = setMethodBuilder.GetILGenerator();
            iLBuilderSetMethod.Emit(OpCodes.Ldarg_0);
            iLBuilderSetMethod.Emit(OpCodes.Ldarg_1);
            iLBuilderSetMethod.Emit(OpCodes.Stfld, fieldBuilder);
            iLBuilderSetMethod.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}