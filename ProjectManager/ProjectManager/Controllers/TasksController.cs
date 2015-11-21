using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Extensions;
using BusinessLogicLayer;
using ProjectManager.Models;
using System.Diagnostics;

namespace ProjectManager.Controllers
{

    [Authorize]
    public class TasksController : Controller
    {
        // GET: Task details
        public ActionResult Details(int Id)
        {
            TaskDetailsModel model;

            int userId = int.Parse(User.Identity.GetProjectUserId());

            using (var db = new ProjectManagerDBEntities())
            {
                var res = from t in db.Task
                          where t.Id == Id
                          select t;
                Task task = res.First();

                if ((from a in db.Assignment where a.ProjectUserId == userId && a.TaskId == task.Id select a).Count() < 1)
                {
                    model = new TaskDetailsModel(task, "", new List<string>(), new Dictionary<string, int>(), new List<CommentViewModel>());
                    return PartialView("_Details", model);
                }

                string state = task.TaskState.Name.Trim();

                var developers = from a in task.Assignment
                                 select new { a.ProjectUser.UserName };

                List<string> devs = new List<string>();
                foreach (var v in developers)
                {
                    devs.Add(v.UserName);
                }

                var workHours = from a in db.Assignment
                                join h in db.Worktime on task.Id equals h.TaskId
                                join user in db.ProjectUser on a.ProjectUserId equals user.Id
                                where h.TaskId == task.Id && a.TaskId == task.Id && a.ProjectUserId == h.ProjectUserId
                                select new { username = user.UserName, hours = h };

                Dictionary<string, int> hours = new Dictionary<string, int>();
                foreach (var time in workHours)
                {
                    int elapsed = (int)(time.hours.EndTime.Subtract(time.hours.StartTime).TotalSeconds / 3600);
                    if (!hours.ContainsKey(time.username))
                    {
                        hours.Add(time.username, elapsed);
                    }
                    else
                    {
                        int currentHours = hours[time.username];
                        hours[time.username] = currentHours + elapsed;
                    }
                }

                List<Comment> comments = (from comment in task.Comment
                                          orderby comment.Timestamp descending
                                          select comment).ToList();
                List<CommentViewModel> commentViewModels = new List<CommentViewModel>();
                foreach (var comment in comments){
                    commentViewModels.Add(
                        new CommentViewModel(comment.Content, comment.Timestamp, comment.ProjectUser.UserName.Trim()));
                }

                model = new TaskDetailsModel(task, state, devs, hours, commentViewModels);
            }

            return PartialView("_Details", model);
        }

        // GET: Create dialog
        public ActionResult CreateDialog(int Id)
        {
            ViewData["projectId"] = Id;
            return PartialView("_CreateDialog");
        }

        // POST: Create task
        [HttpPost]
        public ActionResult Create()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            using (var db = new ProjectManagerDBEntities())
            {
                int projectId = int.Parse(Request.Form["projectId"] as string);
                int leaderId = (from rn in db.RoleName where rn.Name.Trim().Equals("Project Leader") select rn).First().Id;

                var isleader = from r1 in db.Role
                               join p1 in db.Project on r1.ProjectId equals p1.Id
                               where r1.Type == leaderId && p1.Id == projectId && r1.ProjectUserId == userId
                               select r1;

                if (isleader.Count() < 1)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }

                int newStateId = (from ts in db.TaskState where ts.Name.Trim().Equals("New") select ts).First().Id; 

                Task task = new Task();
                task.Name = Request.Form["taskName"];
                task.Description = Request.Form["taskDescription"];
                task.EstimatedWorkHours = int.Parse(Request.Form["workhours"]);
                task.Priority = int.Parse(Request.Form["priority"]);
                task.MaxDevelopers = int.Parse(Request.Form["maxDevs"]); 
                task.ProjectId = projectId;
                task.State = newStateId;

                db.Task.Add(task);
                db.SaveChanges();
            }

            TempData["DetailsPage"] = "1";

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: /Tasks/PostComment
        [HttpPost]
        public ActionResult PostComment(int Id)
        {
            CommentViewModel model;

            int userId = int.Parse(User.Identity.GetProjectUserId());

            using (var db = new ProjectManagerDBEntities())
            {
                //if ((from a in db.Assignment where a.ProjectUserId == userId && a.TaskId == Id select a).Count() < 1)
                //{
                //    return PartialView("_Comment", new CommentViewModel("", new DateTime(), ""));
                //}

                string userName = (from user in db.ProjectUser
                                   where user.Id == userId
                                   select user).First().UserName.Trim();

                DateTime timeStamp = DateTime.Now;

                Comment comment = new Comment();
                comment.ProjectUserId = userId;
                comment.TaskId = Id;
                comment.Timestamp = timeStamp;
                comment.Content = Request.Form["commentContent"];

                db.Comment.Add(comment);
                db.SaveChanges();

                model = new CommentViewModel(Request.Form["commentContent"], timeStamp, userName);
            }
            
            return PartialView("_Comment", model);
        }
    }
}