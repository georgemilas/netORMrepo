
 
using EM.Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.OData;
using System.Web.Http.OData.Query;
using EM.Api.Core;
using EM.Api.Core.Emit;
using EM.Api.Core;

namespace EM.Api.Core.Controllers
{
    
    //HTTP Method Idempotent  Safe
    //OPTIONS	    yes     	yes
    //GET         yes     	yes
    //HEAD        yes     	yes
    //PUT         yes     	no
    //POST        no      	no
    //DELETE      yes         no
    //PATCH       no      	no
    //- See more at: http://restcookbook.com/HTTP%20Methods/idempotency/#sthash.T2SUFr4L.dpuf
          
    

    public class ProjectsEMApiController : EMApiController
    {
        public override string UrlVersionPrefix
        {
            get { return StaticUrlVersionPrefix; }
        }

        public const string StaticUrlVersionPrefix = "v1/";
    }


    //[Authorize]
    public class ProjectsController : ProjectsEMApiController
    {
        private readonly TodoProjectsProvider db = TodoProjectsProvider.Instance;

        [ApiExplorerSettings(IgnoreApi = true)]     //exclude from swagger docs
        [Route("v0/Todoes")]
        [EnableQuery]     //se more here: http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options
        public IQueryable<TodoProject> GetProjectsV0(ODataQueryOptions<TodoProject> queryOptions)
        {
            return db.Projects.AsQueryable();
        }

        [ApiExplorerSettings(IgnoreApi = true)]     //exclude from swagger docs
        [Route("v0/Todoes/{id}")]
        [EnableQuery]
        public SingleResult<TodoProject> GetProjectV0(int id)
        {
            return SingleResult.Create(db.Projects.Where(t => t.Id == id).AsQueryable());
        }




        /// <summary>
        /// List of all Todoes 
        /// </summary>
        /// <returns>A list of TodoProject Api Objects (see docs about api objects)</returns>
        [Route(StaticUrlVersionPrefix + "Todoes")]
        [ResponseType(typeof(IEnumerable<TodoProject>))]
        //[SPODataDataQueryOptions(typeof(TodoProject))]
        public IHttpActionResult GetProjects()
        {
            return GetResult(() => GetApiObjects(db.Projects));
        }

        /// <summary>
        /// TodoProject with Id {id}
        /// </summary>
        /// <param name="id">Unique identifier for project</param>
        /// <returns>A Project Api Object (see docs about api objects)</returns>
        [Route(StaticUrlVersionPrefix + "Todoes/{id}", Name = "GetTodoV1")]
        [ResponseType(typeof(TodoProject))]
        //[SPODataDataQueryOptions(typeof(TodoProject))]
        public IHttpActionResult GetProject(int id)  //, ODataQueryOptions<TodoProject> queryOptions)
        {
            return GetResult(() =>
            {
                TodoProject proj = db.Projects.Find(t => t.Id == id);
                if (proj == null)
                {
                    return ResultWrap.ApiObjectError(HttpStatusCode.NotFound, String.Format("TodoProject {0} could not be found", id));
                }
                return GetApiObject(proj);
            });
        }

        /// <summary>
        /// List of Tasks of a TodoProject with ID {id}
        /// </summary>
        /// <param name="id">Uniqie identifier for a project</param>
        /// <returns>A list of Tasks Api Objects (see docs about api objects)</returns>
        [Route(StaticUrlVersionPrefix + "Todoes/{id}/Tasks")]
        [ResponseType(typeof(IEnumerable<Task>))]
        //[SPODataDataQueryOptions(typeof(Task))]
        public IHttpActionResult GetProjectTasks(int id)  //, ODataQueryOptions<Task> queryOptions)
        {
            return GetResult(() =>
            {
                TodoProject proj = db.Projects.Find(t => t.Id == id);
                if (proj == null)
                {
                    return ResultWrap<IEnumerable<object>>.Action(NotFound());
                }
                return GetApiObjects(proj.Tasks, parent: new ObjectAndParent(proj));
            });
        }

        /// <summary>
        /// Task with ID {taskId} of a TodoProject with ID {id}
        /// </summary>
        /// <param name="id">Uniqie identifier for a project</param>
        /// <param name="taskId">Uniqie identifier for a task within a project</param>
        /// <returns>A Task Api Objects (see docs about api objects)</returns>
        [Route(StaticUrlVersionPrefix + "Todoes/{id}/Tasks/{taskId}")]
        [ResponseType(typeof(Task))]
        //[SPODataDataQueryOptions(typeof(Task))]
        public IHttpActionResult GetProjectTask(int id, int taskId) //, ODataQueryOptions<Task> queryOptions)
        {
            return GetResult(() =>
            {
                TodoProject prj = db.Projects.FirstOrDefault(p => p.Tasks != null && p.Id == id);
                if (prj == null)
                {
                    return ResultWrap.ApiObjectError(HttpStatusCode.NotFound, String.Format("TodoProject {0} could not be found", id));
                }
                Task task = prj.Tasks.FirstOrDefault(t => t.Id == taskId);
                if (task == null)
                {
                    return ResultWrap.ApiObjectError(HttpStatusCode.NotFound, String.Format("Task {0} of TodoProject {1} could not be found", taskId, id));
                }
                return GetApiObject(task, parent: new ObjectAndParent(prj));
            });
        }

        /// <summary>
        /// List of subtasks for Task with ID {taskId} of a TodoProject with ID {id}
        /// </summary>
        /// <param name="id">Uniqie identifier for a project</param>
        /// <param name="taskId">Uniqie identifier for a task within a project</param>
        /// <returns>A list of Task Api Objects (see docs about api objects)</returns>
        [Route(StaticUrlVersionPrefix + "Todoes/{id}/Tasks/{taskId}/Tasks")]
        [ResponseType(typeof(Task))]
        //[SPODataDataQueryOptions(typeof(Task))]
        public IHttpActionResult GetProjectTaskTasks(int id, int taskId) //, ODataQueryOptions<Task> queryOptions)
        {
            return GetResult(() =>
            {
                TodoProject prj = db.Projects.FirstOrDefault(p => p.Tasks != null && p.Id == id);
                if (prj == null)
                {
                    return ResultWrap.ApiObjectsError(HttpStatusCode.NotFound, String.Format("TodoProject {0} could not be found", id));
                }
                Task task = prj.Tasks.FirstOrDefault(t => t.Id == taskId);
                if (task == null)
                {
                    return ResultWrap.ApiObjectsError(HttpStatusCode.NotFound, String.Format("Task {0} of TodoProject {1} could not be found", taskId, id));
                }
                return GetApiObjects(task.Tasks, parent: new ObjectAndParent(task, prj));
            });
        }

        /// <summary>
        /// Task with ID {subTaskId} of a Task with ID {taskId} of a TodoProject with ID {id}
        /// </summary>
        /// <param name="id">Uniqie identifier for a project</param>
        /// <param name="taskId">Uniqie identifier for a task within a project</param>
        /// <param name="subTaskId">Uniqie identifier for a task within a task of a project</param>
        /// <returns>A Task Api Objects (see docs about api objects)</returns>
        [Route(StaticUrlVersionPrefix + "Todoes/{id}/Tasks/{taskId}/Tasks/{subTaskId}")]
        [ResponseType(typeof(Task))]
        //[SPODataDataQueryOptions(typeof(Task))]
        public IHttpActionResult GetProjectTaskTask(int id, int taskId, int subTaskId) //, ODataQueryOptions<Task> queryOptions)
        {
            return GetResult(() =>
            {

                TodoProject prj = db.Projects.FirstOrDefault(p => p.Tasks != null && p.Id == id);
                if (prj == null)
                {
                    return ResultWrap.ApiObjectError(HttpStatusCode.NotFound, String.Format("TodoProject {0} could not be found", id));
                }
                Task task = prj.Tasks.FirstOrDefault(t => t.Id == taskId);
                if (task == null)
                {
                    return ResultWrap.ApiObjectError(HttpStatusCode.NotFound, String.Format("Task {0} of TodoProject {1} could not be found", taskId, id));
                }
                Task subtask = task.Tasks.FirstOrDefault(t => t.Id == subTaskId);
                if (subtask == null)
                {
                    return ResultWrap.ApiObjectError(HttpStatusCode.NotFound, String.Format("Task {0} of Task {1} TodoProject {2} could not be found", subTaskId, taskId, id));
                }
                return GetApiObject(subtask, parent: new ObjectAndParent(task, prj));
            });
        }


        /// <summary>
        /// Person with ID {id}
        /// </summary>
        /// <param name="id">Uniqie identifier for a person</param>
        /// <returns>A Person Api Objects (see docs about api objects)</returns>
        [Route(StaticUrlVersionPrefix + "People/{id}", Name = "GetPerson")]
        [ResponseType(typeof(Person))]
        //[SPODataDataQueryOptions(typeof(Person))]
        public IHttpActionResult GetPerson(int id) //, ODataQueryOptions<Task> queryOptions)
        {
            return GetResult(() =>
            {
                Person person = db.People.FirstOrDefault(p => p.Id == id);
                if (person == null)
                {
                    return ResultWrap.ApiObjectError(HttpStatusCode.NotFound, String.Format("Person {0} could not be found", id));
                }
                return GetApiObject(person);
            });
        }


        // PUT: api/Projects/5
        /// <summary>
        /// Updates the TodoProject with given id using the data sent in the  PUT body
        /// </summary>
        /// <param name="id">Uniqiuely identifies the TodoProject to be updated</param>
        /// <param name="patch">A full or partial TodoProject object. For a partial object iclude all properties you want changed and exclude properties you want to be left untouched.</param>
        /// <returns>Nothing, status code 200 indicates sucess</returns>
        [HttpPut]       //this is "REST update" 
        [Route(StaticUrlVersionPrefix + "Todoes/{id}")]         //because we use config.MapHttpAttributeRoutes() we must have this [Route] or routing logic will not find it 
        public IHttpActionResult PutProject(int id, Delta<TodoProject> patch)
        {
            return ValidateAndUpdate(patch, (td) =>
            {
                if (td.Id != 0 && id != td.Id)
                {
                    return ResultWrap<TodoProject>.ErrorMessage(HttpStatusCode.BadRequest, "Id may not be changed");
                }

                var res = db.Update(id, patch);
                if (res == null)
                {
                    return ResultWrap<TodoProject>.ErrorMessage(HttpStatusCode.NotFound, "The TodoProject you want to change was not found");
                }

                //in order to be indempotent we should return nothing 
                //for example one update may change Title and another one change Notes, so the 2 updates would have diferent outcomes as in 2 calls 2 different outomes => not idempotent 
                return ResultWrap<TodoProject>.Action(StatusCode(HttpStatusCode.NoContent));  //succesfully updated

            });
        }


        // POST: api/Projects/5
        // PATCH: api/Projects/5
        //- in an ideal world this should be "REST create" but because of history of WEB I believe we should allow POST to actualy do updates as well, like PATCH
        //[HttpPost]    //not supported by swagger 2.0 to have 2 POSTs api/Projects/5 and api/Projects

        /// <summary>
        /// Updates the TodoProject with given id using the data sent in the  PATCH body
        /// </summary>
        /// <param name="id">Uniquely identifies a TodoProject to be updated</param>
        /// <param name="patch">A full or partial TodoProject object. For a partial object iclude all properties you want changed and exclude properties you want to be left untouched.</param>
        /// <returns>The full modified TodoProject API object (see docs about Api objects)</returns>
        [HttpPatch]
        [ResponseType(typeof(TodoProject))]
        [Route(StaticUrlVersionPrefix + "Todoes/{id}")]
        public IHttpActionResult PatchProject(int id, Delta<TodoProject> patch)
        {
            //almost equivalent with PUT implememtation except it does not need to be idempotent
            return ValidateAndUpdate(patch, (td) =>
            {
                if (td.Id != 0 && id != td.Id)
                {
                    return ResultWrap<TodoProject>.ErrorMessage(HttpStatusCode.BadRequest, "Id may not be changed");
                }

                var prj = db.Update(id, patch);
                if (prj == null)
                {
                    return ResultWrap<TodoProject>.ErrorMessage(HttpStatusCode.NotFound, "The TodoProject you want to change was not found");
                }
                return ResultWrap<TodoProject>.ResultData(prj);  //succesfully updated so reflect back the modified object
            });
        }


        // POST: api/Projects
        /// <summary>
        /// Creates a new TodoProject object
        /// </summary>
        /// <param name="todoProject">The full TodoProject object to be created. All object propreties must be included in your request.</param>
        /// <returns>The full full TodoProject Api object (see docs about Api objects). The URL of the new object can be found is in the resulting API object Href property as well as the "Location" http header</returns>
        [HttpPost]  //this is "REST create" 
        [ResponseType(typeof(TodoProject))]
        [Route(StaticUrlVersionPrefix + "Todoes")]
        public IHttpActionResult PostProject(TodoProject todoProject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Add(todoProject);
            var res = GetApiObject(todoProject, null);
            return HttpResultTypeNegotiator.GetCreatedAtRouteNegotiator("GetTodoV1", new { id = todoProject.Id }, res, this);
        }


        // DELETE: api/Projects/5
        /// <summary>
        /// Deletes the TodoProject with given id 
        /// </summary>
        /// <param name="id">identifies TodoProject to be deleted</param>
        /// <returns>The TodoProject object that was just deleted</returns>
        [ResponseType(typeof(TodoProject))]
        [Route(StaticUrlVersionPrefix + "Todoes/{id}")]
        public IHttpActionResult DeleteProject(int id)
        {
            TodoProject td = db.Delete(id);
            if (td == null)
            {
                return NotFound();
            }
            return Ok(td);
        }




    }
}



