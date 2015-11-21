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

namespace ProjectManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        // GET: Projects
        public ActionResult Index()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId()); 

            List<Project> projects = new List<Project>();
            using (var db = new ProjectManagerDBEntities())
            {
                var res = from project in db.Project
                          join role in db.Role on project.Id equals role.ProjectId
                          where role.ProjectUserId == userId
                          select project;
                projects = res.ToList();

            }
                
            return View(projects);
        
        }

        // GET: Project details
        public ActionResult Details(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            Project project;
            using (var db = new ProjectManagerDBEntities())
            {
                var res = from p in db.Project
                          join role in db.Role on p.Id equals role.ProjectId
                          where role.ProjectUserId == userId && p.Id == Id
                          select p;
                project = res.First();

            }

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

            using (var db = new ProjectManagerDBEntities())
            {
                Project project = new Project();
                project.Name = Request.Form["projectName"];
                project.Description = Request.Form["projectDescription"];
                project.Deadline = DateTime.Parse(Request.Form["deadline"]);
                project.Done = false;

                db.Project.Add(project);
                db.SaveChanges();

                Role role = new Role();
                role.ProjectUserId = userId;
                role.Type = (from rn in db.RoleName where rn.Name.Trim().Equals("Project Leader") select rn).First().Id;
                role.Project = project;
  
                db.Role.Add(role);
                db.SaveChanges();
            }

            return Redirect("Index");
        }

        // GET: Add developer dialog
        public ActionResult AddDeveloperDialog(int Id)
        {
            AddDeveloperDialogModel model;
            List<ProjectUser> devs = new List<ProjectUser>();

            int userId = int.Parse(User.Identity.GetProjectUserId());

            using (var db = new ProjectManagerDBEntities())
            {
                foreach (var user in (from u in db.ProjectUser select u))
                {
                    if (user.Id != userId)
                    {
                        var res = from r in db.Role where r.ProjectUserId == user.Id && r.ProjectId == Id select r;
                        if (res.Count() == 0)
                        {
                            devs.Add(user);
                        }
                    }
                }
            }

            model = new AddDeveloperDialogModel(Id, devs);
            return PartialView("_AddDeveloperDialog", model);
        }

        // POST: Create project
        [HttpPost]
        public ActionResult AddDeveloper()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            int projectId = int.Parse(Request.Form["projectId"]);
            if (Request.Form["userId"] != null)
            {
                int developerId = int.Parse(Request.Form["userId"]);

                TempData["DetailsPage"] = "2";

                using (var db = new ProjectManagerDBEntities())
                {
                    int leaderId = (from rn in db.RoleName where rn.Name.Trim().Equals("Project Leader") select rn).First().Id;

                    var isleader = from r1 in db.Role
                                   join p1 in db.Project on r1.ProjectId equals p1.Id
                                   where r1.Type == leaderId && p1.Id == projectId && r1.ProjectUserId == userId
                                   select r1;

                    if (isleader.Count() > 0)
                    {
                        var res = from r in db.Role where r.ProjectUserId == developerId && r.ProjectId == projectId select r;
                        if (res.Count() == 0)
                        {
                            Role role = new Role();
                            role.ProjectUserId = developerId;
                            role.Type = (from rn in db.RoleName where rn.Name.Trim().Equals("Developer") select rn).First().Id;
                            role.ProjectId = projectId;

                            db.Role.Add(role);
                            db.SaveChanges();
                        }
                    }

                }
            }

            return Redirect("Details/" + projectId);
        }

        private PartialViewResult ShowOverview(int projectId){
            OverviewModel model;

            int userId = int.Parse(User.Identity.GetProjectUserId());


            using (var db = new ProjectManagerDBEntities())
            {
                Project project;

                var res = from p in db.Project
                            join role in db.Role on p.Id equals role.ProjectId
                            where role.ProjectUserId == userId && p.Id == projectId
                            select p;
                project = res.First();


                int tasksDone = (from td in project.Task
                                 where td.TaskState.Name.Contains("Done")
                                 select td).Count();

                int tasksActive = (from ta in project.Task
                                   where ta.TaskState.Name.Contains("Active")
                                   select ta).Count();

                int tasksUnassigned = (from tu in project.Task
                                       where tu.Assignment.Count == 0
                                       select tu).Count();

                var workHours = (from hours in db.Worktime
                                 join task in db.Task on hours.TaskId equals task.Id
                                 join proj in db.Project on task.ProjectId equals project.Id
                                 where proj.Id == project.Id
                                 select hours);

                double sumHour = 0;
                foreach (var time in workHours)
                {
                    sumHour += time.EndTime.Subtract(time.StartTime).TotalSeconds;
                }

                var developers = from role in project.Role
                                 select new { role.ProjectUser.UserName };

                List<string> devs = new List<string>();
                foreach (var v in developers)
                {
                    devs.Add(v.UserName);
                }

                model = new OverviewModel(
                    project, devs,
                    tasksDone, tasksActive, tasksUnassigned,
                    (int)(sumHour / 3600));
            }


            return PartialView("_Overview", model);
        }

        private PartialViewResult ShowTasks(int projectId)
        {
            List<TaskListElement> model = new List<TaskListElement>();

            int userId = int.Parse(User.Identity.GetProjectUserId());

            ViewData["projectId"] = projectId;

            using (var db = new ProjectManagerDBEntities())
            {
                Project project;

                var res = from p in db.Project
                            join role in db.Role on p.Id equals role.ProjectId
                            where role.ProjectUserId == userId && p.Id == projectId
                            select p;
                project = res.First();

                int leaderId = (from rn in db.RoleName where rn.Name.Trim().Equals("Project Leader") select rn).First().Id;

                var isleader = from r1 in db.Role
                           join p1 in db.Project on r1.ProjectId equals p1.Id
                           where r1.Type == leaderId && p1.Id == project.Id && r1.ProjectUserId == userId
                           select r1;
                if (isleader.Count() > 0)
                {
                    ViewData["isLeader"] = "Leader";
                }
                else
                {
                    ViewData["isLeader"] = "NoLeader";
                }


                foreach (Task task in project.Task){
                    var workHours = from hours in task.Worktime
                        select hours;

                    double sumHour = 0;
                    foreach (var time in workHours)
                    {
                        sumHour += time.EndTime.Subtract(time.StartTime).TotalSeconds;
                    }

                    var developers = from a in task.Assignment
                                     select new { a.ProjectUser.UserName };

                    List<string> devs = new List<string>();
                    foreach (var v in developers)
                    {
                        devs.Add(v.UserName);
                    }

                    TaskListElement element = new TaskListElement(task, task.TaskState.Name.Trim(), devs, (int)(sumHour/3600), task.Comment.Count > 0);
                    model.Add(element);
                }
            }


            return PartialView("_Tasks", model);
        }

        private PartialViewResult ShowDevelopers(int projectId)
        {
            List<DeveloperListElement> model = new List<DeveloperListElement>();

            int userId = int.Parse(User.Identity.GetProjectUserId());

            ViewData["projectId"] = projectId;

            using (var db = new ProjectManagerDBEntities())
            {
                Project project;

                var res = from p in db.Project
                          join role in db.Role on p.Id equals role.ProjectId
                          where role.ProjectUserId == userId && p.Id == projectId
                          select p;
                project = res.First();

                foreach (var dev in project.Role){

                    var workHours = from task in project.Task
                              join a in db.Assignment on task.Id equals a.TaskId
                              join hours in db.Worktime on task.Id equals hours.TaskId
                              where a.ProjectUserId == dev.ProjectUserId && hours.ProjectUserId == dev.ProjectUserId
                              select hours;

                    double sumHour = 0;
                    foreach (var time in workHours)
                    {
                        sumHour += time.EndTime.Subtract(time.StartTime).TotalSeconds;
                    }

                    int tasksDone = (from task in project.Task
                                          join assignment in db.Assignment on task.Id equals assignment.TaskId
                                          where task.TaskState.Name.Contains("Done") && assignment.ProjectUserId == dev.ProjectUserId
                                          select assignment).Count();



                    int tasksAssigned = (from task in project.Task
                                     join assignment in db.Assignment on task.Id equals assignment.TaskId
                                         where !task.TaskState.Name.Contains("Done") && !task.TaskState.Name.Contains("Deleted") && assignment.ProjectUserId == dev.ProjectUserId
                                     select assignment).Count();

                    int leaderId = (from rn in db.RoleName where rn.Name.Trim().Equals("Project Leader") select rn).First().Id;
                    var isleader = from r1 in db.Role
                                   join p1 in db.Project on r1.ProjectId equals p1.Id
                                   where r1.Type == leaderId && p1.Id == projectId && r1.ProjectUserId == userId
                                   select r1;
                    if (isleader.Count() > 0)
                    {
                        ViewData["isLeader"] = "Leader";
                    }
                    else
                    {
                        ViewData["isLeader"] = "NoLeader";
                    }

                    model.Add(new DeveloperListElement(dev.ProjectUser, (int)(sumHour / 3600), tasksDone, tasksAssigned));
                }

            }


            return PartialView("_Developers", model);
        }

        private PartialViewResult ShowStatistics(int projectId)
        {
            StatisticsModel model = new StatisticsModel();

            int userId = int.Parse(User.Identity.GetProjectUserId());


            using (var db = new ProjectManagerDBEntities())
            {
                Project project;

                var res = from p in db.Project
                          join role in db.Role on p.Id equals role.ProjectId
                          where role.ProjectUserId == userId && p.Id == projectId
                          select p;
                project = res.First();

            }

            return PartialView("_Statistics", model);
        }
    }
}