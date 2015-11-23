using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Extensions;
using BusinessLogicLayer;
using ProjectManager.Models;
using System.Diagnostics;
using System.Web.UI;
using BusinessLogicLayer.DTO;

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

            var manager = new TaskManager();
            var task = manager.GetTask(Id);
            var stateName = manager.GetTaskStateName(Id);
            var users = manager.GetUsersForTask(Id);
            var workTimes = manager.GetAllWorkTimeForTask(Id);
            var comments = manager.GetComments(Id);
            var canComment = task.State == manager.GetActiveStateId();

            var project = manager.GetProjectForTask(Id);
            ViewData["isLeader"] = new ProjectUserManager().IsLeader(userId, project.Id);

            int deletedId = manager.GetDeletedStateId();
            ViewData["deletedId"] = deletedId;

            int doneId = manager.GetDoneStateId();
            ViewData["doneId"] = doneId;

            ViewData["isUserOnTask"] = users.Any(u => u.Id == userId);

            List<string> devs = new List<string>();
            foreach (var u in users)
            {
                devs.Add(u.UserName);
            }

            List<CommentViewModel> commentViewModels = new List<CommentViewModel>();
            foreach (var comment in comments)
            {
                var commentingUser = new ProjectUserManager().GetUser(comment.ProjectUserId);
                commentViewModels.Add(
                    new CommentViewModel(comment.Content, comment.Timestamp, commentingUser.UserName.Trim()));
            }

            Dictionary<string, int> userHours = new Dictionary<string, int>();
            foreach (var d in devs)
            {
                userHours.Add(d, 0);
            }

            foreach (var workTime in workTimes)
            {
                var userName = users.First(u => u.Id == workTime.ProjectUserId).UserName;
                int elapsed = (int)(workTime.EndTime.Subtract(workTime.StartTime).TotalSeconds / 3600);

                int currentHours = userHours[userName];
                userHours[userName] = currentHours + elapsed;
            }

            model = new TaskDetailsModel(task, stateName, devs, userHours, commentViewModels, canComment);
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
        public ActionResult Create(CreateTaskModel model)
        {
            int projectId = int.Parse(Request.Form["projectId"] as string);

            if (!ModelState.IsValid)
            {
                // TODO: display errors
                //return PartialView("_CreateDialog");

                TempData["DetailsPage"] = "1";
                return Redirect("/Projects/Details/" + projectId);
            }

            int userId = int.Parse(User.Identity.GetProjectUserId());

            TaskData data = new TaskData()
            {
                Name = model.Name,
                Description = model.Description,
                EstimatedWorkHours = model.WorkHours,
                Priority = model.Priority,
                MaxDevelopers = model.MaxDevelopers,
            };

            new TaskManager().AddNewTask(projectId, data);

            TempData["DetailsPage"] = "1";

            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: /Tasks/PostComment
        [HttpPost]
        public ActionResult PostComment(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            var content = Request.Form["commentContent"];
            if (content == string.Empty)
            {
                return PartialView("_Comment", null);
            }

            DateTime timeStamp = DateTime.Now;
            new CommentManager().AddComment(new CommentData()
            {
                ProjectUserId = userId,
                TaskId = Id,
                Timestamp = timeStamp,
                Content = content
            });

            var userName = new ProjectUserManager().GetUser(userId).UserName.Trim();

            return PartialView("_Comment", new CommentViewModel(content, timeStamp, userName));
        }

        // POST: /Tasks/DeleteTask
        [HttpPost]
        public ActionResult DeleteTask(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            manager.DeleteTask(taskId);

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: /Tasks/RestoreTask
        [HttpPost]
        public ActionResult RestoreTask(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            manager.RestoreTask(taskId);

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: /Tasks/AddWorkTime
        [HttpPost]
        public ActionResult AddWorkTime(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;
            DateTime startTime = DateTime.Now.AddHours(1);
            DateTime endTime = DateTime.Now;

            try
            {
                startTime = DateTime.Parse(Request.Form["startTime"]);
                endTime = DateTime.Parse(Request.Form["endTime"]);
            }
            catch (Exception ex)
            {
                // TODO: exception handling
            }

            if (startTime < endTime && startTime.AddMinutes(-5) < DateTime.Now)
            {
                WorktimeData data = new WorktimeData()
                {
                    ProjectUserId = userId,
                    TaskId = taskId,
                    StartTime = startTime,
                    EndTime = endTime
                };

                if (!manager.AddWorkTime(data))
                {
                    // TODO: error display and stay on view (overlapping work time)
                }
            }
            else
            {
                // TODO: error display and stay on view (invalid start or end time)
            }

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }
    }
}