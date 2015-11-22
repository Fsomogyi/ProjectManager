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
    }
}
