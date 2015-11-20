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

            return View(project);
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

            using (var db = new ProjectManagerDBEntities())
            {
                Project project;

                var res = from p in db.Project
                            join role in db.Role on p.Id equals role.ProjectId
                            where role.ProjectUserId == userId && p.Id == projectId
                            select p;
                project = res.First();


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


            using (var db = new ProjectManagerDBEntities())
            {
                Project project;

                var res = from p in db.Project
                          join role in db.Role on p.Id equals role.ProjectId
                          where role.ProjectUserId == userId && p.Id == projectId
                          select p;
                project = res.First();

                foreach (var dev in project.Role){
                    Debug.WriteLine("User " + dev.ProjectUserId);

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