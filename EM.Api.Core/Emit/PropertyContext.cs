using System;

namespace EM.Api.Core.Emit
{
    /// <summary>
    /// AdditionalProperty of an ApiObjects are provided with a PropertyContext in order to gain access to the chain of object hierachy<para></para>
    /// as well as the controller runtime etc.
    /// </summary>
    public class PropertyContext
    {
        public EMApiController Controller { get; set; }
        public ObjectAndParent Me { get; set; }
        public Object Object { get { return Me.Object; } }
        public ObjectAndParent Parent { get { return Me.Parent; } }

        public static PropertyContext Get(ObjectAndParent op, EMApiController contrl) { return new PropertyContext() { Me = op, Controller = contrl }; }
        //public static PropertyContext Get(object obj, object pObj, EMApiController contrl) { return new PropertyContext() { Object = obj, ParentObject = pObj, Controller = contrl }; }
    }
}