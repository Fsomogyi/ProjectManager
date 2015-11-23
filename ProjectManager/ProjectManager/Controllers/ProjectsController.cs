using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using BusinessLogicLayer;
using Microsoft.AspNet.Identity;
using ProjectManager.Models;
using App.Extensions;
using System.IO;
using BusinessLogicLayer.DTO;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        // GET: Projects
        public ActionResult Index()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            var projects = new ProjectUserManager().GetProjectsForUser(userId);

            return View(projects);
        }

        // GET: Project details
        public ActionResult Details(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            var project = new ProjectUserManager().GetProjectsForUser(userId).First(p => p.Id == Id);

            int pageId = 0;
            Object savedValue;
            if (TempData.TryGetValue("DetailsPage", out savedValue)){
                pageId = int.Parse(savedValue as string);
                TempData.Remove("DetailsPage");
            }

            ProjectDetailsViewModel model = new ProjectDetailsViewModel(project, pageId);

            return View(model);
        }

        // GET: Project details
        public ActionResult DetailsPage(int Id, int pageId)
        {
            switch (pageId)
            {
                case 0: return ShowOverview(Id);
                case 1: return ShowTasks(Id);
                case 2: return ShowDevelopers(Id);
                case 3: return ShowStatistics(Id);
            }

            return ShowOverview(Id);
        }

        // GET: Create dialog
        public ActionResult CreateDialog()
        {
            return PartialView("_CreateDialog");
        }

        // POST: Create project
        [HttpPost]
        public ActionResult Create()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            var name = Request.Form["projectName"];
            var description = Request.Form["projectDescription"];
            var deadlineString = Request.Form["deadline"];
            var deadline = DateTime.Parse(Request.Form["deadline"]);

            if (name == string.Empty || deadline <= DateTime.Now)
            {
                // TODO: display errors

                return Redirect(Request.UrlReferrer.ToString());
            }

            new ProjectUserManager().CreateNewProject(userId, new ProjectData()
            {
                Name = name,
                Description = description,
                Deadline = deadline,
                Done = false
            });

            return Redirect("Index");
        }

        // GET: Add developer dialog
        public ActionResult AddDeveloperDialog(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            var manager = new ProjectUserManager();
            List<ProjectUser> addableDevelopers = manager.GetAddableOrRemovableDevelopers(
                Id, userId, addable: true);
            List<ProjectUser> removableDevelopers = manager.GetAddableOrRemovableDevelopers(
                Id, userId, addable: false);

            return PartialView("_AddDeveloperDialog", new AddDeveloperDialogModel(Id, addableDevelopers, removableDevelopers));
        }

        // POST: Add developer
        [HttpPost]
        public ActionResult AddDeveloper()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());
            int projectId = int.Parse(Request.Form["projectId"]);

            if (Request.Form["addUserId"] != null)
            {
                int developerId = int.Parse(Request.Form["addUserId"]);
                TempData["DetailsPage"] = "2";

                var manager = new ProjectUserManager();
                if (manager.IsLeader(userId, projectId))
                {
                    manager.AddDeveloperToProject(developerId, projectId);
                }
            }

            return Redirect("Details/" + projectId);
        }

        // POST: Remove developer
        [HttpPost]
        public ActionResult RemoveDeveloper()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());
            int projectId = int.Parse(Request.Form["projectId"]);

            if (Request.Form["removeUserId"] != null)
            {
                int developerId = int.Parse(Request.Form["removeUserId"]);
                TempData["DetailsPage"] = "2";

                var manager = new ProjectUserManager();
                if (manager.IsLeader(userId, projectId))
                {
                    manager.RemoveDeveloperFromProject(developerId, projectId);
                }
            }

            return Redirect("Details/" + projectId);
        }

        private PartialViewResult ShowOverview(int projectId){
            OverviewModel model;

            int userId = int.Parse(User.Identity.GetProjectUserId());

            var managerTask = new TaskManager();
            var managerProject = new ProjectUserManager();
            var tasks = managerTask.GetTasksForProject(projectId);

            ViewData["isLeader"] = managerProject.IsLeader(userId, projectId);

            int tasksDone = tasks.Where(t => t.State == managerTask.GetDoneStateId()).Count();
            int tasksActive = tasks.Where(t => t.State == managerTask.GetActiveStateId()).Count();
            int tasksUnassigned = managerTask.GetUnassignedTasks(projectId).Count();
            double workHours = managerTask.GetAllWorkTimeForProject(projectId)
                .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);

            var users = managerProject.GetUsersForProject(projectId);
            List<string> devs = new List<string>();
            foreach (var u in users)
            {
                devs.Add(u.UserName);
            }

            var project = managerProject.GetProject(projectId);

            var projectLeaderName = managerProject.GetProjectLeader(projectId).UserName;

            model = new OverviewModel(
                project, devs,
                tasksDone, tasksActive, tasksUnassigned,
                (int)(workHours / 3600), projectLeaderName);

            return PartialView("_Overview", model);
        }

        private PartialViewResult ShowTasks(int projectId)
        {
            List<TaskListElement> model = new List<TaskListElement>();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var manager = new TaskManager();

            int deletedId = manager.GetDeletedStateId();

            ViewData["projectId"] = projectId;
            ViewData["deletedId"] = deletedId;
            ViewData["isLeader"] = new ProjectUserManager().IsLeader(userId, projectId);
            ViewData["isDone"] = new ProjectUserManager().GetProject(projectId).Done;

            var tasks = manager.GetTasksForProject(projectId);

            foreach (var t in tasks)
            {
                var name = manager.GetTaskStateName(t.Id);

                var isCommented = manager.IsCommented(t.Id);

                var workHours = manager.GetAllWorkTimeForTask(t.Id)
                    .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);

                var developers = manager.GetUsersForTask(t.Id);

                List<string> devs = new List<string>();
                foreach (var v in developers)
                {
                    devs.Add(v.UserName);
                }

                TaskListElement element = new TaskListElement(t, name, devs, (int)(workHours / 3600), isCommented);
                model.Add(element);
            }

            return PartialView("_Tasks", model);
        }

        private PartialViewResult ShowDevelopers(int projectId)
        {
            List<DeveloperListElement> model = new List<DeveloperListElement>();

            int userId = int.Parse(User.Identity.GetProjectUserId());

            var managerTask = new TaskManager();
            var managerProject = new ProjectUserManager();

            ViewData["projectId"] = projectId;
            ViewData["isLeader"] = managerProject.IsLeader(userId, projectId);
            ViewData["isDone"] = managerProject.GetProject(projectId).Done;

            var tasks = managerTask.GetTasksForProject(projectId);
            var users = managerProject.GetUsersForProject(projectId);

            foreach (var u in users)
            {
                var workHours = managerTask.GetAllWorkTimeForUser(u.Id, projectId)
                    .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);

                int tasksDone = tasks.Where(t => t.State == managerTask.GetDoneStateId()).Count();
                int tasksAssigned = managerTask.GetAssignedTasks(u.Id, projectId).Count();

                model.Add(new DeveloperListElement(u, (int)(workHours / 3600), tasksDone, tasksAssigned));
            }

            return PartialView("_Developers", model);
        }

        private PartialViewResult ShowStatistics(int projectId)
        {
            StatisticsModel model = new StatisticsModel();

            int userId = int.Parse(User.Identity.GetProjectUserId());

            return PartialView("_Statistics", model);
        }

        // POST: /Projects/Finish
        [HttpPost]
        public ActionResult Finish(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            var project = new ProjectUserManager().GetProject(Id);

            var manager = new TaskManager();
            var tasks = new TaskManager().GetTasksForProject(Id);

            if (tasks.Count == 0 ||
                tasks.All(t => t.State == manager.GetDeletedStateId() || t.State == manager.GetDoneStateId()))
            {
                new ProjectUserManager().FinishProject(Id);
            }
            else
            {
                // TODO: error display
            }

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}