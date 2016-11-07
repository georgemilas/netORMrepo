using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Collections
{

 

    /// <summary>
    ///Here is some usage Example: 
    ///public enum EmployeeStatus { FullTime, PartTime, Temporary, Unknown }
    ///public class EmployeeStatusManager : EnumManager<EmployeeStatus, string>
    ///{
    ///    private Dictionary<string, EmployeeStatus> _map = new Dictionary<string, EmployeeStatus>()
    ///            {
    ///                {"F", EmployeeStatus.FullTime}, {"P", EmployeeStatus.PartTime},
    ///                {"T1", EmployeeStatus.Temporary}, {"T2", EmployeeStatus.Temporary}    
    ///            };
    ///    public override EmployeeStatus defaultEnum { get { return EmployeeStatus.Unknown; } }
    ///    public override string getValue(EmployeeStatus theEnum)
    ///    {
    ///        var v = base.GetValue(theEnum);
    ///        return v == "T2" ? "T1" : v;    //always default Temporary to T1
    ///   }
    ///    protected override Dictionary<string, EmployeeStatus> valueMap { get { return _map; } }
    ///    public static EmployeeStatusManager instance = new EmployeeStatusManager();
    ///}
    /// </summary>
    [Serializable]
    public abstract class EnumManager<TEnum, TV>
    {
        protected virtual IDictionary<TV, TEnum> valueMap { get; set; }
        protected virtual IDictionary<TEnum, TV> enumMap { get; set; }
        
        /// <summary>
        /// if there are two or more values that map to the same enum
        /// this will return the last value as defined in the value to enum dictionary
        /// </summary>
        public virtual TV getValue(TEnum theEnum)
        {
            if (this.enumMap == null && this.valueMap == null) { throw new NotSupportedException("Cannot use EnumManager instance without providing one of valueMap/enumMap dictionaries"); }
            if (this.enumMap == null && this.valueMap != null)
            {
                this.enumMap = new OrderedDictionary<TEnum, TV>();
                foreach (var val in this.valueMap.Keys)
                {
                    this.enumMap[this.valueMap[val]] = val;                    
                }
            }
            return this.enumMap[theEnum];            
        }

        /// <summary>
        /// some manager instances might want to overrite this to allow for mapping one enum to multiple values
        /// </summary>
        public virtual TEnum getEnum(TV value)
        {
            if (this.enumMap == null && this.valueMap == null) { throw new NotSupportedException("Cannot use EnumManager instance without providing one of valueMap/enumMap dictionaries"); }
            if (this.valueMap == null && this.enumMap != null)
            {
                this.valueMap = new OrderedDictionary<TV, TEnum>();
                foreach (var enm in this.enumMap.Keys)
                {
                    this.valueMap[this.enumMap[enm]] = enm;
                }
            }

            //keys may not be null in a C# dictionary but 
            //we will alow mapping between a null value to a default enum
            if (value != null && this.valueMap.ContainsKey(value))
            {
                return this.valueMap[value];
            }
            else
            {
                return this.defaultEnum;
            }
        }

        /// <summary>
        /// some manager instances might want to overrite this to allow for having a default enum for values that are not known or are null
        /// </summary>
        public virtual TEnum defaultEnum
        {
            get
            {
                throw new KeyNotFoundException("There is no default enum defined for unknown values");
            }
        }

             
    }


    /// <summary>
    ///Here is some usage Example: 
    ///public enum EmployeeStatus { FullTime, PartTime, Temporary, Unknown }
    ///public class EmployeeStatusManager : DictEnumManager<string, EmployeeStatus>
    ///{
    ///    public static EmployeeStatusManager instance = getInstance<EmployeeStatusManager>(new Dictionary<string, EmployeeStatus>()
    ///    {
    ///         {"F", EmployeeStatus.FullTime}, {"P", EmployeeStatus.PartTime},
    ///         {"T1", EmployeeStatus.Temporary}, {"T2", EmployeeStatus.Temporary}    
    ///    };);
    ///    public override EmployeeStatus defaultEnum { get { return EmployeeStatus.Unknown; } }
    ///    public override string getValue(EmployeeStatus theEnum)
    ///    {
    ///        var v = base.GetValue(theEnum);
    ///        return v == "T2" ? "T1" : v;    //always default Temporary to T1
    ///    }
    ///}
    /// </summary>
    [Serializable]
    public class DictEnumManager<TV, TEnum> : EnumManager<TEnum, TV>
    {
        protected IDictionary<TV, TEnum> _map;
        protected override IDictionary<TV, TEnum> valueMap { get { return _map; } }
        private static EnumManager<TEnum, TV> _instance;
        private static object _lockObj = new object();
        protected static TInst getInstance<TInst>(IDictionary<TV, TEnum> valueMap) where TInst : DictEnumManager<TV, TEnum>, new()
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    TInst instance = new TInst();
                    instance._map = valueMap;
                    _instance = instance;
                }
            }
            return (TInst)_instance;      
        }
    }


    [Serializable]
    public class DictEnumManagerByEnum<TEnum, TV> : EnumManager<TEnum, TV>
    {
        protected IDictionary<TEnum, TV> _map;
        protected override IDictionary<TEnum, TV> enumMap { get { return _map; } }
        private static EnumManager<TEnum, TV> _instance;
        private static object _lockObj = new object();
        protected static TInst getInstance<TInst>(IDictionary<TEnum, TV> enumMap) where TInst : DictEnumManagerByEnum<TEnum, TV>, new()
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    TInst instance = new TInst();
                    instance._map = enumMap;
                    _instance = instance;
                }
            }
            return (TInst)_instance;
        }
    }



    /// <summary>
    /// Make sure the casing of values do not matter so if we have this maping 
    ///      Dictionary<string, ENUM>() { {"Red", ENUM.RED},  {"Blue", ENUM.BLUE}, {"Green", ENUM.GREEN});
    ///      searching for Blue, blue, BLUE, bLuE etc. will all yeald ENUM.BLUE
    /// </summary>
    [Serializable]
    public abstract class EnumManagerCaseInsensitiveStringValues<TEnum> : EnumManager<TEnum, string>
    {
        protected IDictionary<string, TEnum> _map;
        protected override IDictionary<string, TEnum> valueMap { get { return _map; } }

        public override TEnum getEnum(string value)
        {
            if (value != null && this.valueMap.ContainsKey(value.ToLower()))
            {
                return this.valueMap[value.ToLower()];
            }
            else
            {
                return this.defaultEnum;
            }
        }

        private static EnumManagerCaseInsensitiveStringValues<TEnum> _instance;
        private static object _lockObj = new object();
        protected static TInst getInstance<TInst>(IDictionary<string, TEnum> valueMap) where TInst : EnumManagerCaseInsensitiveStringValues<TEnum>, new()
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    TInst instance = new TInst();
                    instance._map = new Dictionary<string, TEnum>();
                    foreach (var k in valueMap.Keys)
                    {
                        instance._map[k.ToLower()] = valueMap[k];
                    }
                    _instance = instance;
                }
            }
            return (TInst)_instance;            
        }
    }

}
