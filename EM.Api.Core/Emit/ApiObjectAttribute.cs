using System;
using System.Collections.Generic;

namespace EM.Api.Core.Emit
{

    /// <summary>
    /// Represents a configuration on a class/property to define the following:<para></para>
    /// - each of these classes/properties may have an Href additional propriety<para></para>
    /// - besides Href they also may have additional proprieties<para></para>
    /// - a property may be defined as expandable by default, meaning that the return object will inline the entire proriety or just it's Href attribute<para></para>
    ///     aka for example { ... Address: { Href,City,State,Zip }} vs { ... Address: { Href }}<para></para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ApiObjectAttribute : Attribute 
    {
        public ApiObjectAttribute()
        {
            PropertyExpand = false;
            AdditionalProperties = new List<AdditionalProperty>();            
            AdditionalProperties.Add(AdditionalProperty.Href(null));
        }

        /// <summary>
        /// Specifies if the type for the property should by default be expanded or not 
        /// If not expanded then only Href would be exposed 
        /// </summary>
        public bool PropertyExpand { get; set; }

        /// <summary>
        /// Callback to get href    
        /// </summary>
        public Func<PropertyContext, object> Href 
        {
            get { return this.AdditionalProperties.Find(p => p.PropertyName == "Href").PropertyValueProvider; } 
            set { this.AdditionalProperties.Find(p => p.PropertyName == "Href").PropertyValueProvider = value; } 
        }

        /// <summary>
        /// Additional Properties
        /// </summary>
        public List<AdditionalProperty> AdditionalProperties { get; set; }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /////// Chain-ing API 
        ////////////////////////////////////////////////////////////////////////////////////////////////////// 

        public ApiObjectAttribute ClearAdditionalProperties()
        {
            AdditionalProperties = new List<AdditionalProperty>();
            return this;
        }
        public ApiObjectAttribute AddAdditionalProperty(AdditionalProperty additional)
        {
            AdditionalProperties.Add(additional);
            return this;
        }
        public ApiObjectAttribute AddAdditionalProperties(params AdditionalProperty[] additional)
        {
            AdditionalProperties.AddRange(additional);
            return this;
        }

        public ApiObjectAttribute SetHref(Func<PropertyContext, object> href)
        {
            Href = href;
            return this;
        }
        public ApiObjectAttribute SetPropertyExpand(bool propertyExpand)
        {
            PropertyExpand = propertyExpand;
            return this;
        }

    }
}