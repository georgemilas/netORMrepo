using System;

namespace EM.Api.Core.Emit
{
    /// <summary>
    /// Represents one additional property to be added to an ApiObject via ApiObjectAttribute attribute
    /// </summary>
    public class AdditionalProperty
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public Func<PropertyContext, object> PropertyValueProvider { get; set; }


        /////////////////////// Static Helpers:  ///////////////////////
        public static AdditionalProperty MakeProperty<T>(string name, Func<PropertyContext, object> getValue)
        {
            return new AdditionalProperty() { PropertyName = name, PropertyType = typeof(T), PropertyValueProvider = getValue };
        }
        public static AdditionalProperty Href(Func<PropertyContext, object> getHref) { return MakeProperty<string>("Href", getHref); }
        public static AdditionalProperty First(Func<PropertyContext, string> getFirst) { return MakeProperty<string>("First", getFirst); }
        public static AdditionalProperty Previous(Func<PropertyContext, string> getPrevious) { return MakeProperty<string>("Previous", getPrevious); }
        public static AdditionalProperty Next(Func<PropertyContext, string> getNext) { return MakeProperty<string>("Next", getNext); }
        public static AdditionalProperty Last(Func<PropertyContext, string> getLast) { return MakeProperty<string>("Last", getLast); }
    }
}