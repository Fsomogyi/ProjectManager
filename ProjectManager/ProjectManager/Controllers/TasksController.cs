﻿using System;
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

            var project = manager.GetProjectForTask(Id);
            if (new ProjectUserManager().IsLeader(userId, project.Id))
            {
                ViewData["isLeader"] = "Leader";
            }
            else
            {
                ViewData["isLeader"] = "NoLeader";
            }

            int deletedId = manager.GetDeletedStateId();
            ViewData["deletedId"] = deletedId;

            int doneId = manager.GetDoneStateId();
            ViewData["doneId"] = doneId;

            var task = manager.GetTask(Id);
            var stateName = manager.GetTaskStateName(Id);
            var users = manager.GetUsersForTask(Id);
            var workTimes = manager.GetAllWorkTimeForTask(Id);
            var comments = manager.GetComments(Id);
            var canComment = task.State == manager.GetActiveStateId();

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
            int userId = int.Parse(User.Identity.GetProjectUserId());

            var manager = new TaskManager();
            manager.DeleteTask(taskId);

            var projectId = manager.GetProjectForTask(taskId).Id;

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }

        // POST: /Tasks/RestoreTask
        [HttpPost]
        public ActionResult RestoreTask(int taskId)
        {
            int userId = int.Parse(User.Identity.GetProjectUserId());

            var manager = new TaskManager();
            manager.RestoreTask(taskId);

            var projectId = manager.GetProjectForTask(taskId).Id;

            TempData["DetailsPage"] = "1";
            return Redirect("/Projects/Details/" + projectId);
        }
    }
}