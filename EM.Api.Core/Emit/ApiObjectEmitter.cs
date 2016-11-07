using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using EM.Api.Core.OData;
using System.Web.Http;
using EM.Api.Core.Emit.Serialize;
using EM.Api.Core.Metadata;
using EM.Api.Core.Models.Exceptions;


namespace EM.Api.Core.Emit
{

    

    /// <summary>
    /// - Given any object it returns an ApiObject with only the properties that are specified by the $select and $expand rules<para></para>
    /// - It adds additional properties such as Href for navigation discoverability or Version for concurent updates support etc. based on MetadataConfiguration or Attrributes configuration of classes and properties<para></para>
    /// - It replaces instances of DateTime with DateTimeOffset in order to ensure serializing the date with a timezone offset specified<para></para>
    /// </summary>
    public class ApiObjectEmitter
    {
        public string ModuleName { get; set; }
        public List<AdditionalProperty> AdditionalProperties { get; set; }
        private ModuleEmitter ModuleEmitter { get; set; }
        public EMApiController Controller { get; set; }
        public Func<Type, TypeMetadataStrategy> TypeMetadataStrategy { get; set; }
        public bool UseHierachicalNames { get; set; }
        //public ICustomSerializerTypeResolver SerializerTypeResolver { get; set; }

        /// <summary>
        /// Initializes the emmiter with a module name consisting of BCL and UserName
        /// </summary>
        /// <param name="controller"></param>
        public ApiObjectEmitter(EMApiController controller): this(MediaFormatter.ToCLSCompliantIdentifier(controller.ApiUser + "ApiObject"), controller) { }
        public ApiObjectEmitter(string moduleName = null, EMApiController controller = null, bool useHierachicalNames = true)
        {
            UseHierachicalNames = useHierachicalNames;
            ModuleName = moduleName ?? "ApiObject";
            AdditionalProperties = new List<AdditionalProperty>();
            var code = new CodeEmiter(ModuleName);
            ModuleEmitter = code.ModuleEmitter;
            Controller = controller;
            if (controller != null && controller.ApiObjectMetadataStrategy != null)
            {
                TypeMetadataStrategy = controller.ApiObjectMetadataStrategy;
            }
            else
            {
                TypeMetadataStrategy = tp => new TypeDescriptorBasedTypeMetadataStrategy(tp);
                //new ReflectionBasedTypeMetadataStrategy(tp);
            }
        }
        private ApiObjectEmitter(ModuleEmitter moduleEmitter, EMApiController controller, Func<Type, TypeMetadataStrategy> typeMetadataStrategy, bool useHierachicalNames)
        {
            ModuleName = moduleEmitter.ModulePrefix;
            AdditionalProperties = new List<AdditionalProperty>();
            ModuleEmitter = moduleEmitter;
            Controller = controller;
            TypeMetadataStrategy = typeMetadataStrategy;
            UseHierachicalNames = useHierachicalNames;
        }


        /// <summary>
        /// Register the Type with current SPXmlSerializerMediaFormatter. 
        /// The XMLSerializer need to have prior knowledge of the types involved
        /// </summary>
        public void ResolveTypeForRegisteredSerializerFormatter(Type tp)
        {
            if (Controller != null)
            {
                //var formatter = (ICustomSerializerTypeResolver)Controller.Configuration.Formatters.First(f => f is ICustomSerializerTypeResolver);
                var formatter = Controller.ApiObjectXmlFormatter;
                if (formatter != null)
                {
                    formatter.ResolveType(tp);
                }
            }
        }

        private enum VisitType { Select, Expand };
        private void VisitTypeProperties(Type tp, SelectExpandResult se, Action<PropertyMetadata, VisitType, SelectExpandItem> visitor)
        {
            var strategy = TypeMetadataStrategy(tp);
            foreach (var p in strategy.GetProperties())
            {
                //Expanded must be processed first in order to handle posible contradiction with ApiObjAttribute(PropertyExpand=false)   
                if (se.Expanded.Count > 0)
                {
                    var ese = se.GetExpandedItem(p.Name);
                    if (ese != null)
                    {
                        visitor(p, VisitType.Expand, ese);
                        continue;
                    }
                }

                if (se.IsAllSelected)
                {
                    var qse = se.GetQueryItem(p.Name);  //we must handle query
                    if (qse != null)   
                    {
                        //we have a query at the element but expand was not manually called for the element
                        //so we check Property Attribute Configuration if we specified to expand by default or not
                        var attr = p.GetCustomAttribute<ApiObjectAttribute>();
                        var attrExpand = attr != null ? attr.PropertyExpand : false;
                        if (!attrExpand)
                        {
                            throw new ApiParameterParsingException(String.Format("$query can only be performed on expanded elements. Element {0} is not expanded, use $expand on {0}", p.Name));
                        }
                    }
                    visitor(p, VisitType.Select, qse);
                    continue;
                }

                if (se.Selected.Count > 0)
                {
                    var sse = se.GetSelectedItem(p.Name);                                        
                    if (sse != null)
                    {
                        visitor(p, VisitType.Select, sse);
                        continue;
                    }
                }
                
            }
        }

        /// <summary>
        /// Emit a new ApiObject wrapper for type "tp" and return an instance of it based on original "data"
        /// </summary>        
        public object BuildObject(object data, Type tp, SelectExpandResult se, ObjectAndParent parent = null)
        {
            TypeEmitter apiTypeEmitter = BuildType(tp, se, null);
            ObjectAndParent op = new ObjectAndParent(data, parent);
            var inst = GetInstance(tp, op, apiTypeEmitter, se);
            ResolveTypeForRegisteredSerializerFormatter(inst.GetType());
            return inst;
        }

        /// <summary>
        /// Emit a new ApiObject wrapper for type "tp" and return an enumeration of instances of it based on original "data" collection
        /// </summary>
        public IEnumerable<object> BuildCollection<T>(IEnumerable<T> data, Type tp, SelectExpandResult se, ObjectAndParent parent = null)
        {
            List<object> res = new List<object>();
            TypeEmitter apiTypeEmitter = BuildType(tp, se, null);
            foreach (var obj in data)
            {
                ObjectAndParent op = new ObjectAndParent(obj, parent);
                var inst = GetInstance(tp, op, apiTypeEmitter, se);
                res.Add(inst);
            }
            ResolveTypeForRegisteredSerializerFormatter(res.GetType());
            return res;
            
        }

        private void AddAditionalProprieties(Type tp, TypeEmitter typeEmitter)
        {
            typeEmitter.AdditionalPropertiesValueProviders.AddRange(this.AdditionalProperties);
            foreach (var additional in AdditionalProperties)
            {
                typeEmitter.BuildProperty(additional.PropertyName, additional.PropertyType);
            }

            var strategy = TypeMetadataStrategy(tp);
            var tpAttr = strategy.GetCustomAttribute<ApiObjectAttribute>();
            if (tpAttr != null)
            {
                //typeEmitter.AdditionalPropertiesValueProviders.AddRange(tpAttr.AdditionalProperties);
                foreach (var additional in tpAttr.AdditionalProperties)
                {
                    if (!AdditionalProperties.Exists(p => p.PropertyName == additional.PropertyName))
                    {
                        typeEmitter.AdditionalPropertiesValueProviders.Add(additional);
                        typeEmitter.BuildProperty(additional.PropertyName, additional.PropertyType);
                    }
                }
            }
        }

        /// <summary>
        /// Emit ApiObject wrapper for given type tp using the select/expand rules provided
        /// </summary>
        public TypeEmitter BuildType(Type tp, SelectExpandResult se, TypeEmitter parentTypeEmitter)
        {
            
            var typeCacheKey = ApiObjectTypeEmitterCacheKey.GetApiObjectTypeEmitterCacheKey(ModuleEmitter, tp, se, parentTypeEmitter, UseHierachicalNames);
            var typeEmitterReturn = ModuleEmitter.GetTypeEmmiter(typeCacheKey);
            if (!typeEmitterReturn.InCache && typeEmitterReturn.TypeEmitter.Type == null)
            {
                var typeEmitter = typeEmitterReturn.TypeEmitter;
                BuildTypeImpl(tp, se, typeEmitter);
            }
            return typeEmitterReturn.TypeEmitter;
            //em.SaveAssembly();            
        }

        private void BuildTypeImpl(Type tp, SelectExpandResult se, TypeEmitter typeEmitter)
        {
            AddAditionalProprieties(tp, typeEmitter);

            VisitTypeProperties(tp, se, (p, visitType, sePseItm) =>
            {
                var sePse = sePseItm != null ? sePseItm.SelectExpand : null;
                var attr = p.GetCustomAttribute<ApiObjectAttribute>();
                if (attr != null || sePse != null)  //property or underlying type is decorated with ApiObject  
                {                    
                    var attrExpand = attr != null ? attr.PropertyExpand : false;
                    var attrAdditionalProps = attr != null ? attr.AdditionalProperties : new List<AdditionalProperty>();

                    var pse = new SelectExpandResult(p.PropertyType.FullName)
                    {
                        IsAllSelected = attrExpand || visitType == VisitType.Expand
                    };
                    var attrAndExpandBasedAllSelected = pse.IsAllSelected;
                    if (sePse != null) //can't do this for circular reference type
                    {
                        pse.IsAllSelected = sePse.Selected.Count > 0 ? sePse.IsAllSelected : pse.IsAllSelected;
                        pse.Selected = sePse.Selected;
                        pse.Expanded = sePse.Expanded;
                        pse.Query = sePse.Query;
                    }
                    
                    var pem = new ApiObjectEmitter(ModuleEmitter, Controller, TypeMetadataStrategy, UseHierachicalNames);
                    bool isGenericCollectionType = IsGenericEnumerableType(p.PropertyType);
                    if (isGenericCollectionType) //ex: List<Employee>
                    {
                        if (attrAndExpandBasedAllSelected)
                        {
                            BuildTypeInCollection(typeEmitter, p, pem, pse);
                        }
                        else
                        {
                            //build an enumerable type with just additional proprieties (ex:    [ {href}, {href}, {href}... ] )
                            pem.AdditionalProperties.AddRange(attrAdditionalProps);  //add the Href and whatever else was declared to the list itself  
                            var apiTypeEmitter = pem.BuildType(typeof(object), pse, typeEmitter);
                            typeEmitter.BuildProperty(p.Name, apiTypeEmitter.Type);
                        }
                    }
                    else
                    {
                        pem.AdditionalProperties.AddRange(attrAdditionalProps);     //add the Href and whatever else was declared 
                        TypeEmitter apiTypeEmitter = pem.BuildType(p.PropertyType, pse, typeEmitter);
                        //if (apiTypeEmitter.Type == null)   //circular reference detected
                        //typeEmitter.BuildProperty(p.Name, apiTypeEmitter.TypeBuilder); //build circular reference property

                        //build all complex properties using a dynamic type instead of static type in order to not have to deal with circular references or pseudo circular references  (A.B and B.pseudoA)
                        typeEmitter.BuildProperty(p.Name, typeof(object));  //typeEmitter.BuildProperty(p.Name, apiTypeEmitter.Type);                        
                    }
                }
                else //not decorated with ApiObject  
                {
                    var pse = new SelectExpandResult(p.PropertyType.FullName) { IsAllSelected = true };
                    var pem = new ApiObjectEmitter(ModuleEmitter, Controller, TypeMetadataStrategy, UseHierachicalNames);
                        
                    if (IsGenericEnumerableType(p.PropertyType)) //ex: List<Employee>
                    {
                        BuildTypeInCollection(typeEmitter, p, pem, pse);                 
                    }
                    else
                    {
                        if (IsTypeForApiObject(p.PropertyType))
                        {
                            //any non system class 
                            TypeEmitter apiTypeEmitter = pem.BuildType(p.PropertyType, pse, typeEmitter);  //build it 
                            //if (apiTypeEmitter.Type == null)  { typeEmitter.BuildProperty(p.Name, apiTypeEmitter.TypeBuilder); }  //circular reference detected so build circular reference property
                            typeEmitter.BuildProperty(p.Name, typeof (object));  //but use "dynamic" bounding to handle circular references 
                        }
                        else
                        {
                            //primitive types etc.
                            var ptp = p.PropertyType == typeof(DateTime) ? typeof(DateTimeOffset) 
                                                                         : p.PropertyType == typeof(Nullable<DateTime>) ? typeof(Nullable<DateTimeOffset>) 
                                                                                                                        : p.PropertyType;
                            typeEmitter.BuildProperty(p.Name, ptp);        
                        }
                    }                    
                }
            });

            typeEmitter.EmitType();
            ResolveTypeForRegisteredSerializerFormatter(typeEmitter.Type);            
            
        }
        
        private Type GetGenericListType(Type typeInCollection)
        {
            Type lst = typeof(List<>);
            Type[] lstArgs = { typeInCollection };
            Type lstType = lst.MakeGenericType(lstArgs);
            return lstType;
        }

        private bool IsGenericEnumerableType(Type tp)
        {
            return tp != typeof(string)   
                   && (((tp.IsGenericTypeDefinition || tp.IsGenericType) && tp.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                         || tp.GetInterfaces().Any(t =>
                          //t.GetGenericTypeDefinition() == typeof(IEnumerable) ||
                          (t.IsGenericTypeDefinition || t.IsGenericType)
                          && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                   );
        }

        private void BuildTypeInCollection(TypeEmitter typeEmitter, PropertyMetadata enumerableProperty, ApiObjectEmitter pem, SelectExpandResult pse)
        {
            Type typeInCollection = enumerableProperty.PropertyType.GenericTypeArguments[0]; //ex: Employee out of List<Employee>
            TypeEmitter typeInCollectionTypeEmitter = pem.BuildType(typeInCollection, pse, typeEmitter);
            //build ApiObject wrapper for the actual type in collection 
            Action resolveList = () =>
            {
                Type lstType = GetGenericListType(typeInCollectionTypeEmitter.Type);
                ResolveTypeForRegisteredSerializerFormatter(lstType);
                //typeEmitter.BuildProperty(enumerableProperty.Name, lstType);
            };
            if (typeInCollectionTypeEmitter.Type == null)
            {
                typeInCollectionTypeEmitter.OnAfterEmit.Add(resolveList);
            }
            else
            {
                resolveList();
            }
            typeEmitter.BuildProperty(enumerableProperty.Name, typeof(object));
        }

        private bool IsTypeForApiObject(Type tp)
        {   
            //for nullable types, check the ineer type not Nullable itself
            if ((tp.IsGenericTypeDefinition || tp.IsGenericType) && tp.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                tp = tp.GenericTypeArguments[0]; //ex: DateTime out of Nullable<DateTime>    
            }

            Func<string, bool> isSystem = str => str.StartsWith("System.") || str.StartsWith("Microsoft."); 
            //IsPrimitive: Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single                        
            return tp.IsClass //&& !tp.IsInterface
                   && !tp.IsPrimitive
                   && tp != typeof (string)
                   && !tp.IsEnum                   
                   //&& tp != typeof(DateTime);            
                   //&& !tp.IsNotPublic  //must be public
                   && !isSystem(tp.Namespace);                   
        }

        private object GetApiObjectInstance(ObjectAndParent valueOP, PropertyMetadata p, ApiObjectEmitter pem, SelectExpandResult pse, TypeEmitter parentTypeEmitter)
        {
            //TypeEmitter propApiTypeEmitter = pem.BuildType(p.PropertyType, pse);
            ApiObjectTypeEmitterCacheKey typeCacheKey = ApiObjectTypeEmitterCacheKey.GetApiObjectTypeEmitterCacheKey(pem.ModuleEmitter, p.PropertyType, pse, parentTypeEmitter, UseHierachicalNames);
            TypeEmitter propApiTypeEmitter = pem.ModuleEmitter.GetTypeEmmiter(typeCacheKey).TypeEmitter;
            return GetInstance(p.PropertyType, valueOP, propApiTypeEmitter, pse);
        }

        private object GetEnumerableApiObjectInstance(IEnumerable origValue, ObjectAndParent parentOP, PropertyMetadata enumerableProperty, ApiObjectEmitter pem, SelectExpandResult pse, TypeEmitter parentTypeEmitter, SelectExpandItem pseItm)
        {
            Type typeInCollection = enumerableProperty.PropertyType.GenericTypeArguments[0];   //ex: Employee out of List<Employee>
            var typeInCollectionCacheKey = ApiObjectTypeEmitterCacheKey.GetApiObjectTypeEmitterCacheKey(pem.ModuleEmitter, typeInCollection, pse, parentTypeEmitter, UseHierachicalNames);
            TypeEmitter typeInCollectionTypeEmitter = pem.ModuleEmitter.GetTypeEmmiter(typeInCollectionCacheKey).TypeEmitter;   //it is already built, get from cache
            Type lstType = GetGenericListType(typeInCollectionTypeEmitter.Type);    //Items = List<EmployeeApiObject> not List<Employee>
            var resLst = (IList)Activator.CreateInstance(lstType);
            if (origValue == null)
            {
                resLst = null;
            }
            else
            {
                foreach (var v in origValue)
                {
                    var vop = new ObjectAndParent(v, parentOP);
                    if (pseItm != null && pseItm.RawQuery != null && pseItm.Name == enumerableProperty.Name)
                    {
                        var qitm = pseItm; //pse.GetQueryItem(enumerableProperty.Name);
                        if (qitm != null && qitm.EvaluateQuery(v))
                        {
                            var vinst = GetInstance(typeInCollection, vop, typeInCollectionTypeEmitter, pse);
                            resLst.Add(vinst);
                        }                        
                    }
                    else
                    {
                        var vinst = GetInstance(typeInCollection, vop, typeInCollectionTypeEmitter, pse);
                        resLst.Add(vinst);
                    }
                }
            }
            return resLst;
        }


        /// <summary>
        /// Return an instance of an ApiObject defined in apiTypeEmitter using data from ObjectAndParent "op"  
        /// </summary>
        public object GetInstance(Type origType, ObjectAndParent op, TypeEmitter apiTypeEmitter, SelectExpandResult se)
        {
            var orig = op.Object;
            if (orig == null)
            {
                return null;
            }

            if (apiTypeEmitter.Type == null)   
            {
                //a circular type not yet emited because of an instance of SelectExpand not encountered during BuildType  
                //for example Expand = false deeper in hierarchy
                BuildTypeImpl(origType, se, apiTypeEmitter);
            }


            object apiObj = Activator.CreateInstance(apiTypeEmitter.Type);

            VisitTypeProperties(origType, se, (p, visitType, sePseItm) =>
            {
                var sePse = sePseItm != null ? sePseItm.SelectExpand : null;
                var attr = p.GetCustomAttribute<ApiObjectAttribute>();
                var origP = orig.GetType().GetProperty(p.Name);

                if (attr != null || sePse != null)  //property or underlying type is decorated with ApiObject  
                {
                    var attrExpand = attr != null ? attr.PropertyExpand : false;
                    var attrAdditionalProps = attr != null ? attr.AdditionalProperties : new List<AdditionalProperty>();

                    var pse = new SelectExpandResult(p.PropertyType.FullName) { IsAllSelected = attrExpand || visitType == VisitType.Expand };
                    var attrAndExpandBasedAllSelected = pse.IsAllSelected;
                    if (sePse != null) //can't do this for circular reference type
                    {
                        pse.IsAllSelected = sePse.Selected.Count > 0 ? sePse.IsAllSelected : pse.IsAllSelected;
                        pse.Selected = sePse.Selected;
                        pse.Expanded = sePse.Expanded;
                        pse.Query = sePse.Query;
                    }
                    var pem = new ApiObjectEmitter(ModuleEmitter, Controller, TypeMetadataStrategy, UseHierachicalNames);
                    bool isGenericCollectionType = IsGenericEnumerableType(p.PropertyType);
                    object inst = origP.GetValue(orig);
                    if (isGenericCollectionType)      //ex: List<Employee>
                    {                                                
                        if (attrAndExpandBasedAllSelected)      
                        {
                            //we have actual enumerable<T> to return so we return a List<ApiObject wrap for T>  [ { Href, ...}, { Href, ... } ]
                            inst = GetEnumerableApiObjectInstance((IEnumerable)inst, op, p, pem, pse, apiTypeEmitter, sePseItm);                            
                        }
                        else    
                        {
                            //we are not instatiating an enumerable [{},{},..] but an instead we
                            //instantiate an empty type with just additional proprieties (ex: {href}                             
                            var resLstOp = new ObjectAndParent(inst, op);
                            var typeCacheKey = ApiObjectTypeEmitterCacheKey.GetApiObjectTypeEmitterCacheKey(pem.ModuleEmitter, typeof(object), pse, apiTypeEmitter, UseHierachicalNames);
                            var propApiTypeEmitter = pem.ModuleEmitter.GetTypeEmmiter(typeCacheKey).TypeEmitter;    //it is already built, get from cache
                            inst = GetAdditionalPropertiesOnlyInstance(resLstOp, propApiTypeEmitter);
                        }                        
                    }
                    else
                    {
                        inst = GetApiObjectInstance(new ObjectAndParent(inst, op), p, pem, pse, apiTypeEmitter);
                    }

                    apiTypeEmitter.Type.InvokeMember(p.Name, BindingFlags.SetProperty, null, apiObj, new[] { inst });                        

                }
                else  //not decorated with ApiObject  
                {
                    var pse = new SelectExpandResult(p.PropertyType.FullName) { IsAllSelected = true };
                    var pem = new ApiObjectEmitter(ModuleEmitter, Controller, TypeMetadataStrategy, UseHierachicalNames);

                    object inst = origP.GetValue(orig);

                    if (IsGenericEnumerableType(p.PropertyType)) //ex: List<Employee>
                    {
                        inst = GetEnumerableApiObjectInstance((IEnumerable)inst, op, p, pem, pse, apiTypeEmitter, sePseItm);
                    }
                    else
                    {
                        if (IsTypeForApiObject(p.PropertyType))  //any non system class 
                        {                            
                            inst = GetApiObjectInstance(new ObjectAndParent(inst, op), p, pem, pse, apiTypeEmitter);
                        }
                        else  //primitive types etc.
                        {
                            if (inst is DateTime)  //this also captures nullable DateTime with values  
                            {
                                inst = new DateTimeOffset((DateTime)inst);
                            }                            
                        }
                    }

                    apiTypeEmitter.Type.InvokeMember(p.Name, BindingFlags.SetProperty, null, apiObj, new[] { inst });
                    //var v = apiType.InvokeMember(p.Name, BindingFlags.GetProperty, null, apiObj, new object[] { }); // as p.PropertyType;
                    
                }
            });

            foreach (var additional in apiTypeEmitter.AdditionalPropertiesValueProviders)
            {
                apiTypeEmitter.Type.InvokeMember(additional.PropertyName, BindingFlags.SetProperty, null, apiObj, new[] { additional.PropertyValueProvider(PropertyContext.Get(op, Controller)) });
            }

            return apiObj;            

        }

        private object GetAdditionalPropertiesOnlyInstance(ObjectAndParent op, TypeEmitter apiTypeEmitter)
        {
            var orig = op.Object;
            if (orig == null)
            {
                return null;
            }

            object apiObj = Activator.CreateInstance(apiTypeEmitter.Type);

            foreach (var additional in apiTypeEmitter.AdditionalPropertiesValueProviders)
            {
                apiTypeEmitter.Type.InvokeMember(additional.PropertyName, BindingFlags.SetProperty, null, apiObj, new[] { additional.PropertyValueProvider(PropertyContext.Get(op, Controller)) });
            }

            return apiObj;
        }




    }














}