
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EM.Api.Core.Emit;
using EM.Api.Core.Metadata;
using System.Web.OData;

namespace EM.Api.Core.Models
{

    //#region ApiObjectAttributes

    ////[ApiObject(Href = p => ... )]  //Doesn't work to bind functions at decoration time. 
    ////So in order to bind functions at decoration point so we must use a separate class with the function already specified 
    //public class PersonApiObjectAttribute : ApiObjectAttribute
    //{
    //    public PersonApiObjectAttribute()
    //    {
    //        Href = p => p.Controller.RootUrl + "People/" + ((Person)p.Object).Id;
    //    }
    //}
    //public class PeopleApiObjectAttribute : ApiObjectAttribute
    //{
    //    public PeopleApiObjectAttribute()
    //    {
    //        AdditionalProperties = new List<AdditionalProperty>();  //no href
    //    }
    //}
    //public class TasksListApiObjectAttribute : ApiObjectAttribute
    //{
    //    public TasksListApiObjectAttribute()
    //    {
    //        Href = p => p.Controller.RootUrl + "Projects/" + ((TodoProject)p.Parent.Object).Id + "/Tasks";
    //    }
    //}
    //public class TaskApiObjectAttribute : ApiObjectAttribute
    //{
    //    public TaskApiObjectAttribute()
    //    {
    //        Href = p =>
    //        {
    //            //even tough the entiry can go arbitrary deep here for example Tasks.SubTasks.SubTasks.SubTasks.SubTasks etc.
    //            //we will only have controller that goes   TodoProject/id/Tasks/id/SubTasks/id
    //            if (p.Parent.Parent != null && p.Parent.Parent.Object is TodoProject)   
    //            {
    //                //it's a SubTask of a Task of a TodoProject
    //                return p.Controller.RootUrl + "Projects/" + ((TodoProject) p.Parent.Parent.Object).Id + "/Tasks/" + ((Task) p.Parent.Object).Id + "/Tasks/" + ((Task) p.Object).Id;
    //            }
    //            //it's a Task of a TodoProject
    //            return p.Controller.RootUrl + "Projects/" + ((TodoProject) p.Parent.Object).Id + "/Tasks/" + ((Task) p.Object).Id;                
    //        };
    //    }
    //}
    //public class TaskSubTasksApiObjectAttribute : ApiObjectAttribute
    //{
    //    public TaskSubTasksApiObjectAttribute()
    //    {
    //        Href = p =>
    //        {
    //            var prj = (TodoProject)p.Parent.Parent.Object;
    //            var tsk = (Task)p.Parent.Object;
    //            var href = p.Controller.RootUrl + "Projects/" + prj.Id + "/Tasks/" + tsk.Id + "/Tasks/";
    //            return href;
    //        };
    //    }
    //}
    //public class ProjectApiObjectAttribute : ApiObjectAttribute
    //{
    //    public ProjectApiObjectAttribute()
    //    {
    //        Href = p => p.Controller.RootUrl + "Projects/" + ((TodoProject)p.Object).Id;
    //        AdditionalProperties.Add(AdditionalProperty.MakeProperty<int>("VersionId", p => p.Object.GetHashCode()));
    //    }
    //}

    //#endregion ApiObjectAttribute

    //[PersonApiObject]
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        //[PersonApiObject]
        public Person Parent { get; set; }
    }

    //[TaskApiObject]
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }

        //[PersonApiObject]
        public Person HelpPerson { get; set; }

        //[TaskSubTasksApiObject(PropertyExpand = false)]
        public List<Task> Tasks { get; set; }

        //[PeopleApiObject(PropertyExpand = true)]  //no href so expand by default
        public List<Person> People { get; set; }
    }

    //[ProjectApiObject]
    public class TodoProject
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        public string Notes { get; set; }
        public bool IsDone { get; set; }

        //[TasksListApiObject]
        public List<Task> Tasks { get; set; }

        public DateTimeOffset Date { get; set; }      //should get translated into a DateTimeOffset in the resulting ApiObject

        //[PersonApiObject]
        public Person HelpPerson { get; set; }
    }



    //public class Product
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string Description { get; set; }
    //    public Order Order { get; set; }
    //}

    //public class Order
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public double Amount { get; set; }
    //    public Customer Customer { get; set; }
    //}
    //public class Customer
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public Product Product { get; set; }
    //}

    

    public partial class EntityAttributesConfiguration
    {
        public EntityAttributesConfiguration ConfigureProjectRelatedEntities()
        {
            Func<PropertyContext, object> personHref = p => p.Controller.RootUrl + "People/" + ((Person) p.Object).Id;
            
            new MetadataConfiguration<Person>()
                .AddHrefClassAttribute(personHref)
                //.AddHrefPropertyAttribute("Parent", personHref)
                ;
            

            new MetadataConfiguration<Task>()
                .AddHrefClassAttribute(p =>
                    {
                        //even tough the entiry can go arbitrary deep here for example Tasks.SubTasks.SubTasks.SubTasks.SubTasks etc.
                        //we will only have controller that goes   TodoProject/id/Tasks/id/SubTasks/id
                        if (p.Parent.Parent != null && p.Parent.Parent.Object is TodoProject)
                        {
                            //it's a SubTask of a Task of a TodoProject
                            return p.Controller.RootUrl + 
                                    "Todoes/" + ((TodoProject) p.Parent.Parent.Object).Id +
                                    "/Tasks/" + ((Task) p.Parent.Object).Id + 
                                    "/Tasks/" + ((Task) p.Object).Id;
                        }
                        //it's a Task of a TodoProject
                        return p.Controller.RootUrl + 
                                "Todoes/" + ((TodoProject) p.Parent.Object).Id + 
                                "/Tasks/" + ((Task) p.Object).Id;
                    })
                .AddHrefPropertyAttribute("HelpPerson", personHref)
                .AddHrefPropertyAttribute("Tasks", p =>
                    {
                        var prj = (TodoProject)p.Parent.Parent.Object;
                        var tsk = (Task)p.Parent.Object;
                        var href = p.Controller.RootUrl + "Todoes/" + prj.Id + "/Tasks/" + tsk.Id + "/Tasks/";
                        return href;
                    })
                .AddPropertyAttributes("People", new ApiObjectAttribute()
                                                           .ClearAdditionalProperties()  //no href
                                                           .SetPropertyExpand(false));
            
            new MetadataConfiguration<TodoProject>()
                .AddClassAttributes(new ApiObjectAttribute()
                    .SetHref(p => p.Controller.RootUrl + "Todoes/" + ((TodoProject)p.Object).Id)
                    .AddAdditionalProperty(AdditionalProperty.MakeProperty<int>("VersionId", p => p.Object.GetHashCode())))
                .AddHrefPropertyAttribute("Tasks", p => p.Controller.RootUrl + "Projects/" + ((TodoProject)p.Parent.Object).Id + "/Tasks")
                .AddHrefPropertyAttribute("HelpPerson", personHref);
            
            //new MetadataConfiguration<Product>()
            //    //projectConfig.AddHrefClassAttribute(p => p.Controller.RootUrl + "Products/" + ((Product)p.Object).Id);
            //    .AddClassAttributes(new ApiObjectAttribute().ClearAdditionalProperties().SetPropertyExpand(true));
            //new MetadataConfiguration<Customer>()
            //    .AddClassAttributes(new ApiObjectAttribute().ClearAdditionalProperties().SetPropertyExpand(false));
            //    //projectConfig.AddHrefClassAttribute(p => p.Controller.RootUrl + "Customers/" + ((Customer)p.Object).Id);
            //new MetadataConfiguration<Order>()
            //    .AddClassAttributes(new ApiObjectAttribute().ClearAdditionalProperties().SetPropertyExpand(true));
            //    //projectConfig.AddHrefClassAttribute(p => p.Controller.RootUrl + "Orders/" + ((Order)p.Object).Id);
            
            return this;
        }        

    }



    public class TodoProjectsDatabase
    {
        public List<TodoProject> Projects;
        public List<Person> People = new List<Person>()
        {
            new Person() { Id = 0, Name = "Dummy Index pushet"},
            new Person() { Id = 1, Name = "Gheorghe Milas"},
            new Person() { Id = 2, Name = "Maria Milas" },
            new Person() { Id = 3, Name = "Andrew Milas"},
            new Person() { Id = 4, Name = "Anthony Milas" },
            new Person() { Id = 5, Name = "Bunica" },
            new Person() { Id = 6, Name = "Strabunica" },
        };
        //public List<Product> Products = new List<Product>()
        //{
        //    new Product() { Id = 0, Description = "Chicken"},
        //    new Product() { Id = 1, Description = "Bread"},
        //    new Product() { Id = 2, Description = "Computer" },
        //    new Product() { Id = 3, Description = "Book"},
        //};
        //public List<Customer> Customers = new List<Customer>()
        //{
        //    new Customer() { Id = 0, Name = "George"},
        //    new Customer() { Id = 1, Name = "Mimi"},
        //    new Customer() { Id = 2, Name = "Andi" },
        //};
        //public List<Order> Orders = new List<Order>()
        //{
        //    new Order() { Id = 0, Amount = 1234.5 },
        //    new Order() { Id = 1, Amount = 789.11},
        //};

        public TodoProjectsDatabase()
        {
            //Products[0].Order = Orders[0];
            //Products[1].Order = Orders[1];
            //Orders[0].Customer = Customers[0];
            //Orders[1].Customer = Customers[2];
            //Customers[0].Product = Products[0];
            //Customers[1].Product = Products[1];
            People[3].Parent = People[2];
            People[4].Parent = People[2];
            People[2].Parent = People[5];
            People[5].Parent = People[6];
            Projects = new List<TodoProject>()
            {
                new TodoProject() { Id=1, Title = "shop", Date = DateTime.Now, HelpPerson = People[2]},
                new TodoProject() { Id=2, Title = "do odata stuff", Date = new DateTime(2015, 11, 21, 10, 21, 00) },
                new TodoProject() { Id=3, Title = "do api conference stuff", Date = DateTime.Now, HelpPerson = People[1],
                                Tasks = new List<Task>() { 
                                        new Task { Id = 1, Description = "Key Note", People = new List<Person> { People[3] }}, 
                                        new Task { Id = 2, Description = "Dev forum", HelpPerson = People[2], 
                                                People = new List<Person> { People[4], People[2] },
                                                Tasks = new List<Task>() { 
                                                        new Task { Id = 1, Description = "Key Note", People = new List<Person> { People[3] }}, 
                                                        new Task { Id = 2, Description = "Dev forum", HelpPerson = People[2], 
                                                                People = new List<Person> { People[4] }}}
                                        },
                                        new Task { Id = 2, Description = "Tech forum"} } },
                new TodoProject() { Id=4, Title = "go to work", Date = DateTime.Now, IsDone = true, HelpPerson = People[3],
                                Tasks = new List<Task>() { 
                                        new Task { Id = 3, Description = "Do API", HelpPerson = People[1],
                                                People = new List<Person> { People[1], People[3] }}, 
                                        new Task { Id = 4, Description = "Do Odata API", 
                                                People = new List<Person> { People[1] }} }},
                new TodoProject() { Id=5, Title = "go to conference", Date = DateTime.Now },
                new TodoProject() { Id=6, Title = "do match", Date = DateTime.Now, HelpPerson = People[1]},
                new TodoProject() { Id=7, Title = "go to conference 3", Date = DateTime.Now },
                new TodoProject() { Id=8, Title = "go to conference 4", Date = DateTime.Now }
            };
        }
    }

    public class TodoProjectsProvider
    {
        public static TodoProjectsProvider Instance = new TodoProjectsProvider();
        public TodoProjectsDatabase Database = new TodoProjectsDatabase();
        public List<TodoProject> Projects { get { return Database.Projects; } }
        public List<Person> People { get { return Database.People; } }

        public void Add(TodoProject td)
        {
            td.Id = Projects.Max(t => t.Id) + 1;
            if (td.Date == default(DateTimeOffset))
            {
                td.Date = DateTime.Now;
            }
            Projects.Add(td);
        }

        public TodoProject Delete(int id)
        {
            var td = Projects.Find(t => t.Id == id);
            if (td != null)
            {
                Projects.Remove(td);
                return td;
            }
            return null;
        }

        public TodoProject Update(int id, Delta<TodoProject> patch)
        {
            var orig = Projects.Find(t => t.Id == id);
            if (orig != null)
            {
                patch.Patch(orig);
                return orig;
            }
            return null;
        }

        public TodoProject Overwrite(int id, Delta<TodoProject> patch)
        {
            var orig = Projects.Find(t => t.Id == id);
            if (orig != null)
            {
                patch.Put(orig);
                return orig;
            }
            return null;
        }


    }





}



