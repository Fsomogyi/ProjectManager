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
            int userId = int.Parse(User.Identity.GetProjectUserId());
            return PartialView("_Details", CreateTaskDetailsModel(userId, Id));
        }

        private TaskDetailsModel CreateTaskDetailsModel(int userId, int taskId)
        {
            var manager = new TaskManager();
            var task = manager.GetTask(taskId);
            var stateName = manager.GetTaskStateName(taskId);
            var users = manager.GetUsersForTask(taskId);
            var workTimes = manager.GetAllWorkTimeForTask(taskId);
            var comments = manager.GetComments(taskId);
            var canComment = task.State == manager.GetActiveStateId();
            var addableDevelopers = manager.GetAddableOrRemovableDevelopers(
                taskId, userId, addable: true);
            var removableDevelopers = manager.GetAddableOrRemovableDevelopers(
                taskId, userId, addable: false);
            var unacceptedDevelopers = manager.GetUnacceptedDevelopers(taskId);
            var unacceptedTaskStateChanges = manager.GetUnacceptedTaskStateChanges(taskId);

            var project = manager.GetProjectForTask(taskId);
            ViewData["isLeader"] = new ProjectUserManager().IsLeader(userId, project.Id);

            int deletedId = manager.GetDeletedStateId();
            ViewData["deletedId"] = deletedId;

            int doneId = manager.GetDoneStateId();
            ViewData["doneId"] = doneId;

            int activeId = manager.GetActiveStateId();
            ViewData["activeId"] = activeId;

            int newId = manager.GetNewStateId();
            ViewData["newId"] = newId;

            ViewData["isUserOnTask"] = users.Any(u => u.Id == userId);
            ViewData["isUserApplyUnaccepted"] = unacceptedDevelopers.Any(u => u.Id == userId);
            Boolean maxDevelopers = task.MaxDevelopers == null ? false : task.MaxDevelopers <= users.Count;
            ViewData["userHasWorktime"] = workTimes.Any(w => w.ProjectUserId == userId);

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

            bool projectDone = new ProjectUserManager().IsDone(task.ProjectId);

            return new TaskDetailsModel(task, stateName, devs, userHours, commentViewModels, canComment,
                addableDevelopers, removableDevelopers, unacceptedDevelopers, unacceptedTaskStateChanges,
                projectDone, maxDevelopers);

        }

        // GET: Create dialog
        public ActionResult CreateDialog(int Id)
        {
            ViewData["projectId"] = Id;
            return PartialView("_CreateDialog");
        }

        // GET: Add & remove developer dialog
        public ActionResult AddOrRemoveDeveloperDialog(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());
            return PartialView("_AddRemoveDeveloperDialog", CreateTaskDetailsModel(userId, Id));
        }

        // GET: Delete task dialog
        public ActionResult DeleteTaskDialog(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());
            return PartialView("_DeleteDialog", new TaskManager().GetTask(Id));
        }

        // GET: Delete task dialog
        public ActionResult RestoreTaskDialog(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());
            return PartialView("_RestoreDialog", new TaskManager().GetTask(Id));
        }

        // GET: Delete task dialog
        public ActionResult AddWorkTimeDialog(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());
            return PartialView("_AddWorkTimeDialog", new TaskManager().GetTask(Id));
        }

        // GET: Finish task dialog
        public ActionResult FinishDialog(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());
            return PartialView("_FinishDialog", new TaskManager().GetTask(Id));
        }

        // GET: Reopoen task dialog
        public ActionResult ReopenDialog(int Id)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());
            return PartialView("_ReopenDialog", new TaskManager().GetTask(Id));
        }

        // POST: Create task
        [HttpPost]
        public ActionResult Create(CreateTaskModel model)
        {
            int projectId = int.Parse(Request.Form["projectId"] as string);

            if (!ModelState.IsValid)
            {
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
            var reason = Request.Form["deleteReason"];

            manager.DeleteTask(taskId, userId, reason);

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
            var reason = Request.Form["restoreReason"];

            manager.RestoreTask(taskId, userId, reason);

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
                TempData["errorMessage"] = "Wrong date/time format!";
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
                    TempData["errorMessage"] = "Overlapping work time!";
                }
            }
            else
            {
                TempData["errorMessage"] = "Invalid start or end time!";
            }

            TempData["DetailsPage"] = "1";
            TempData["overlayId"] = "TaskDetails";
            TempData["TaskDetailsId"] = "" + taskId;
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: /Tasks/FinalizeUserApplication
        [HttpPost]
        public ActionResult FinalizeUserApplication(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            if (!manager.FinalizeUserApplication(taskId))
            {
                // TODO: error display
            }

            TempData["DetailsPage"] = "1";
            TempData["overlayId"] = "TaskDetails";
            TempData["TaskDetailsId"] = "" + taskId;
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/AddDeveloper
        [HttpPost]
        public ActionResult AddDeveloper(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            if (Request.Form["addUserId"] != null)
            {
                int developerId = int.Parse(Request.Form["addUserId"]);
                manager.AddDeveloperToTask(developerId, taskId, accepted: true);
            }

            TempData["DetailsPage"] = "1";
            TempData["overlayId"] = "TaskDetails";
            TempData["TaskDetailsId"] = "" + taskId;
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/RemoveDeveloper
        [HttpPost]
        public ActionResult RemoveDeveloper(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            if (Request.Form["removeUserId"] != null)
            {
                int developerId = int.Parse(Request.Form["removeUserId"]);
                manager.RemoveDeveloperFromTask(developerId, taskId);
            }

            TempData["DetailsPage"] = "1";
            TempData["overlayId"] = "TaskDetails";
            TempData["TaskDetailsId"] = "" + taskId;
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/ApplyDeveloper
        [HttpPost]
        public ActionResult ApplyDeveloper(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            manager.AddDeveloperToTask(userId, taskId, accepted: false);

            TempData["DetailsPage"] = "1";
            TempData["overlayId"] = "TaskDetails";
            TempData["TaskDetailsId"] = "" + taskId;
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/AcceptAppliance
        [HttpPost]
        public ActionResult AcceptAppliance(int taskId, int developerId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            manager.AcceptAppliance(taskId, developerId);

            TempData["DetailsPage"] = "1";
            TempData["overlayId"] = "TaskDetails";
            TempData["TaskDetailsId"] = "" + taskId;
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/DeclineAppliance
        [HttpPost]
        public ActionResult DeclineAppliance(int taskId, int developerId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            manager.DeclineAppliance(taskId, developerId);

            TempData["DetailsPage"] = "1";
            TempData["overlayId"] = "TaskDetails";
            TempData["TaskDetailsId"] = "" + taskId;
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/FinishTask
        [HttpPost]
        public ActionResult FinishTask(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;
            var isLeader = new ProjectUserManager().IsLeader(userId, projectId);
            var reason = Request.Form["finishReason"];

            manager.FinishTask(taskId, userId, reason, isLeader);

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/ReopenTask
        [HttpPost]
        public ActionResult ReopenTask(int taskId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;
            var isLeader = new ProjectUserManager().IsLeader(userId, projectId);
            var reason = Request.Form["reopenReason"];

            manager.UnfinishTask(taskId, userId, reason, isLeader);

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/AcceptStateChange
        [HttpPost]
        public ActionResult AcceptStateChange(int taskId, int developerId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            manager.AcceptStateChange(taskId, developerId);

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: Tasks/DeclineStateChange
        [HttpPost]
        public ActionResult DeclineStateChange(int taskId, int developerId)
        {
            var manager = new TaskManager();

            int userId = int.Parse(User.Identity.GetProjectUserId());
            var projectId = manager.GetProjectForTask(taskId).Id;

            manager.DeclineStateChange(taskId, developerId);

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }
    }
}