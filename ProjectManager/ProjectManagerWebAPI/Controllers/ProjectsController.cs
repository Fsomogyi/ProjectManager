using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using App.Extensions;
using ProjectManagerWebAPI.Models;

namespace ProjectManagerWebAPI.Controllers
{

    [Authorize]
    public class ProjectsController : ApiController
    {

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<ProjectData> Projects()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            List<ProjectData> res = new List<ProjectData>();
            var projectUserManager = new ProjectUserManager();
            foreach (Project p in projectUserManager.GetProjectsForUser(userId))
            {
                ProjectData data = new ProjectData();
                data.Id = p.Id;
                data.Name = p.Name;
                data.Description = p.Description;
                data.Deadline = p.Deadline;
                data.Done = p.Done;
                res.Add(data);
            }

            return res;
        }

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public OverviewModel ProjectOverview(int Id)
        {
            OverviewModel model;

            int userId = int.Parse(User.Identity.GetProjectUserId());

            var managerTask = new TaskManager();
            var managerProject = new ProjectUserManager();
            var tasks = managerTask.GetTasksForProject(Id);

            int tasksDone = tasks.Where(t => t.State == managerTask.GetDoneStateId()).Count();
            int tasksActive = tasks.Where(t => t.State == managerTask.GetActiveStateId()).Count();
            int tasksUnassigned = managerTask.GetUnassignedTasks(Id).Count();
            double workHours = managerTask.GetAllWorkTimeForProject(Id)
                .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);

            var users = managerProject.GetUsersForProject(Id);
            List<string> devs = new List<string>();
            foreach (var u in users)
            {
                devs.Add(u.UserName);
            }

            var project = managerProject.GetProject(Id);

            var projectLeaderName = managerProject.GetProjectLeader(Id).UserName;

            model = new OverviewModel(
                project.Id, project.Name, project.Description, project.Deadline, devs,
                tasksDone, tasksActive, tasksUnassigned,
                (int)(workHours / 3600), projectLeaderName);

            return model;
        }

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<TaskListElement> TaskList(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            List<TaskListElement> res = new List<TaskListElement>();
            var manager = new TaskManager();
            foreach (Task t in manager.GetTasksForProject(Id))
            {
                string stateName = manager.GetTaskStateName(t.State);

                IEnumerable<ProjectUser> devUsers = manager.GetUsersForTask(t.Id);
                List<string> devs = new List<string>();
                foreach (var v in devUsers)
                {
                    devs.Add(v.UserName);
                }

                var workHours = manager.GetAllWorkTimeForTask(t.Id)
                    .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);

                bool hasComments = manager.IsCommented(t.Id);

                res.Add(new TaskListElement(t.Id,t.Name, stateName, devs, (int)(workHours / 3600), hasComments));
            }

            return res;
        }
    }
}
