using System;
using System.ComponentModel;
using EM.Api.Core.Emit;

namespace EM.Api.Core.Metadata
{
    /*
     * - Istead of decorating Entity Objects at design time with attributes related to API needs 
     *    we can employ this method at run time 
     *    and leave the entity object clean of metadata 
     * - This will allow entities to be more reusable across multiple contexts or across multiple APIs etc.
      
     * *******************************************************************************************
     * So instead of this:
     *      
            //we must create a new Attribute class otherwise we can't pass the custom Href 
            public class PersonApiObjectAttribute : ApiObjectAttribute
            {
                public PersonApiObjectAttribute()
                {
                    Href = p => p.Controller.RootUrl + "People/" + ((Person)p.Object).Id;
                }
            }
            
            //Anotate the object with ApiObject related metadata   
            [PersonApiObject]   
            public class Person
            {
                [Key]
                public int Id { get; set; }
                public string Name { get; set; }
                
                [PersonApiObject]
                public Person Parent { get; set; }
            }
     
     * *******************************************************************************************
     * We can do this: 
     *      //Keep the class free of ApiObject related metadata   
            public class Person
            {
                [Key]
                public int Id { get; set; }
                public string Name { get; set; }
                public Person Parent { get; set; }
            }
            //And add the following to WebApiConfig.cs  
            var personAtribute = new ApiObjectAttribute() { Href = p => p.Controller.RootUrl + "People/" + ((Person)p.Object).Id };
            new MetadataConfiguration<Person>()
     *          .AddClassAttributes(personAtribute)
     *          .AddPropertyAttributes("Parent", personAtribute);
            
     * 
     */
    public class MetadataConfiguration<T> 
    {
        public DynamicAttributesTypeDescriptor<T> CustomAttributesDescriptor { get; set; }
        public Type Type { get; set; }

        public MetadataConfiguration()
        {
            Type = typeof(T);
            CustomAttributesDescriptor = new DynamicAttributesTypeDescriptor<T>();
            TypeDescriptor.AddProvider(new AttributesProviderTypeDescriptor(CustomAttributesDescriptor), Type);
        }

        public MetadataConfiguration<T> AddHrefClassAttribute(Func<PropertyContext, object> href)
        {
            return AddClassAttributes(new ApiObjectAttribute { Href = href });
        }
        public MetadataConfiguration<T> AddClassAttributes(params Attribute[] attributes)
        {
            CustomAttributesDescriptor.AddClassAttributes(attributes);
            return this;
        }
        public MetadataConfiguration<T> AddHrefPropertyAttribute(string propertyName, Func<PropertyContext, object> href)
        {
            return AddPropertyAttributes(propertyName, new ApiObjectAttribute { Href = href });            
        }
        public MetadataConfiguration<T> AddPropertyAttributes(string propertyName, params Attribute[] attributes)
        {
            CustomAttributesDescriptor.AddPropertyAttributes(propertyName, attributes);
            return this;
        }

        

    }
}