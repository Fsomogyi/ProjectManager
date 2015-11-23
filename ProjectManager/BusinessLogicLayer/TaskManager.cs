using BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class TaskManager
    {
        public int GetNewStateId()
        {
            return 1;
        }

        public int GetActiveStateId()
        {
            return 2;
        }

        public int GetDoneStateId()
        {
            return 3;
        }

        public int GetDeletedStateId()
        {
            return 4;
        }

        public void AddNewTask(int projectId, TaskData data)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int newId = GetNewStateId();

                context.Task.Add(new Task()
                    {
                        Name = data.Name,
                        Description = data.Description,
                        Priority = data.Priority,
                        MaxDevelopers = data.MaxDevelopers,
                        EstimatedWorkHours = data.EstimatedWorkHours,
                        ProjectId = projectId,
                        State = newId
                    });

                context.SaveChanges();
            }
        }

        public Project GetProjectForTask(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var task = context.Task.First(t => t.Id == taskId);

                return context.Project.First(p => p.Id == task.ProjectId);
            }
        }

        public string GetTaskStateName(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var task = GetTask(taskId);

                return context.TaskState.First(ts => ts.Id == task.State).Name;
            }
        }

        public List<Comment> GetComments(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Comment.Where(c => c.TaskId == taskId).ToList();
            }
        }

        public bool IsCommented(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var task = context.Task.First(t => t.Id == taskId);

                return task.Comment.Count > 0;
            }
        }

        public Task GetTask(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Task.First(t => t.Id == taskId);
            }
        }

        public List<Task> GetTasksForProject(int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Task.Where(t => t.ProjectId == projectId).ToList();
            }
        }

        public List<Task> GetAssignedTasks(int userId, int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Task.Where(t => t.ProjectId == projectId &&
                    t.Assignment.Any(a => a.ProjectUserId == userId)).ToList();
            }
        }

        public List<Task> GetUnassignedTasks(int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Task.Where(t => t.ProjectId == projectId &&
                    t.Assignment.Count == 0).ToList();
            }
        }

        public List<Worktime> GetAllWorkTimeForProject(int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                List<Worktime> result = new List<Worktime>();

                var tasks = context.Task.Where(t => t.ProjectId == projectId).ToList();
                var worktimes = context.Worktime.ToList();

                foreach (var w in worktimes)
                {
                    if (tasks.Any(t => t.Id == w.TaskId))
                        result.Add(w);
                }

                return result;
            }
        }

        public List<Worktime> GetAllWorkTimeForTask(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Worktime.Where(w => w.TaskId == taskId).ToList();
            }
        }

        public List<Worktime> GetAllWorkTimeForUser(int userId, int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                List<Worktime> result = new List<Worktime>();

                var tasks = context.Task.Where(t => t.ProjectId == projectId).ToList();
                var worktimes = context.Worktime.Where(w => w.ProjectUserId == userId).ToList();

                foreach (var w in worktimes)
                {
                    if (tasks.Any(t => t.Id == w.TaskId))
                        result.Add(w);
                }

                return result;
            }
        }

        public List<ProjectUser> GetUsersForTask(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                List<ProjectUser> result = new List<ProjectUser>();

                var userIds = context.Assignment.Where(a => a.TaskId == taskId).Select(a => a.ProjectUserId);
                var users = context.ProjectUser;

                foreach (var u in users)
                {
                    if (userIds.Contains(u.Id))
                        result.Add(u);
                }

                return result;
            }
        }

        public void DeleteTask(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int deletedId = GetDeletedStateId();

                var task = context.Task.First(t => t.Id == taskId);
                task.State = deletedId;

                context.SaveChanges();
            }
        }

        public void RestoreTask(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int newId = GetNewStateId();
                int activeId = GetActiveStateId();

                var task = context.Task.First(t => t.Id == taskId);

                var workTimes = context.Worktime.Where(w => w.TaskId == taskId);

                if (workTimes.Count() > 0)
                    task.State = activeId;
                else
                    task.State = newId;

                context.SaveChanges();
            }
        }

        public bool AddWorkTime(WorktimeData data)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var relatedWorkTimes = context.Worktime.Where(w => w.TaskId == data.TaskId &&
                    w.ProjectUserId == data.ProjectUserId);
                var overLappingWorkTimes = relatedWorkTimes.Where(w => 
                    (data.StartTime >= w.StartTime && data.StartTime <= w.EndTime) ||
                    (data.EndTime >= w.StartTime && data.EndTime <= w.EndTime));

                if (overLappingWorkTimes.Count() == 0)
                {

                    context.Worktime.Add(new Worktime()
                    {
                        ProjectUserId = data.ProjectUserId,
                        TaskId = data.TaskId,
                        StartTime = data.StartTime,
                        EndTime = data.EndTime
                    });

                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public bool FinalizeUserApplication(int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int activeId = GetActiveStateId();

                var task = context.Task.First(t => t.Id == taskId);

                var unAcceptedAssignments = context.Assignment.Where(a => a.TaskId == taskId &&
                    a.Accepted == false);

                if (unAcceptedAssignments.Count() == 0)
                {
                    task.State = activeId;
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public List<ProjectUser> GetAddableOrRemovableDevelopers(int taskId, int leaderId, bool addable)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                List<ProjectUser> result = new List<ProjectUser>();

                var task = context.Task.First(t => t.Id == taskId);

                foreach (var user in context.ProjectUser.Where(u => u.Id != leaderId))
                {
                    var assignments = task.Assignment.Where(a => a.ProjectUserId == user.Id);

                    if ((assignments.Count() == 0) == addable)
                    {
                        result.Add(user);
                    }
                }

                return result;
            }
        }

        public void AddDeveloperToTask(int userId, int taskId, bool accepted)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int developerId = new ProjectUserManager().GetDeveloperId();
                var assignments = context.Assignment.Where(
                    a => a.TaskId == taskId && a.ProjectUserId == userId);

                if (assignments.Count() == 0)
                {
                    context.Assignment.Add(new Assignment()
                    {
                        ProjectUserId = userId,
                        TaskId = taskId,
                        Accepted = accepted
                    });

                    context.SaveChanges();
                }
            }
        }

        public void RemoveDeveloperFromTask(int userId, int taskId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int developerId = new ProjectUserManager().GetDeveloperId();
                var assignment = context.Assignment.FirstOrDefault(
                    a => a.TaskId == taskId && a.ProjectUserId == userId);

                if (assignment != null)
                    context.Assignment.Remove(assignment);

                context.SaveChanges();
            }
        }
    }
}
