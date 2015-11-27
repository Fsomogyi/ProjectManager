using BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using App.Extensions;
using BusinessLogicLayer;

namespace ProjectManagerWebAPI.Controllers
{
    public class TasksController : ApiController
    {

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public void Create(int Id, [FromBody] TaskData task)
        {

            int userId = int.Parse(User.Identity.GetProjectUserId());

            TaskData data = new TaskData()
            {
                Name = task.Name,
                Description = task.Description,
                EstimatedWorkHours = task.EstimatedWorkHours,
                Priority = task.Priority,
                MaxDevelopers = task.MaxDevelopers,
            };

            new TaskManager().AddNewTask(Id, data);
        }
    }
}
