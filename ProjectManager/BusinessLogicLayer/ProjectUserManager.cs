using BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ProjectUserManager
    {
        public int GetDeveloperId()
        {
            return 1;
        }

        public int GetLeaderId()
        {
            return 2;
        }

        public void FinishProject(int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var project = context.Project.First(p => p.Id == projectId);

                project.Done = true;

                context.SaveChanges();
            }
        }

        public int AddUserAndReturnId(string userName)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var user = new ProjectUser()
                {
                    UserName = userName
                };

                context.ProjectUser.Add(user);
                context.SaveChanges();

                return user.Id;
            }
        }

        public Project GetProject(int id)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Project.First(p => p.Id == id);
            }
        }

        public ProjectUser GetUser(int id)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.ProjectUser.First(u => u.Id == id);
            }
        }

        public List<Project> GetProjectsForUser(int userId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Project.Where(p => p.Role.Any(r => r.ProjectUserId == userId)).ToList();
            }
        }

        public List<ProjectUser> GetUsersForProject(int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var roles = context.Role.Where(
                    r => r.ProjectId == projectId);

                return context.ProjectUser.Where(u => roles.Any(r => r.ProjectUserId == u.Id)).ToList();
            }
        }

        public void CreateNewProject(int userId, ProjectData data)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var project = new Project()
                {
                    Name = data.Name,
                    Description = data.Description,
                    Deadline = data.Deadline,
                    Done = data.Done
                };

                context.Project.Add(project);

                context.Role.Add(new Role()
                {
                    ProjectUserId = userId,
                    Type = new ProjectUserManager().GetLeaderId(),
                    Project = project
                });

                context.SaveChanges();
            }
        }

        public List<ProjectUser> GetAddableOrRemovableDevelopers(int projectId, int leaderId, bool addable)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                List<ProjectUser> result = new List<ProjectUser>();

                var project = context.Project.First(p => p.Id == projectId);

                foreach (var user in context.ProjectUser.Where(u => u.Id != leaderId))
                {
                    var roles = project.Role.Where(r => r.ProjectUserId == user.Id);

                    if ((roles.Count() == 0) == addable)
                    {
                        result.Add(user);
                    }
                }

                return result;
            }
        }

        public bool IsLeader(int userId, int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int leaderId = GetLeaderId();

                var leaderRoles = context.Role.Where(r => r.ProjectId == projectId &&
                        r.ProjectUserId == userId && r.Type == leaderId);

                return leaderRoles.Count() == 1;
            }
        }

        public ProjectUser GetProjectLeader(int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int leaderId = GetLeaderId();

                var leaderRole = context.Role.First(r => r.ProjectId == projectId && r.Type == leaderId);

                return context.ProjectUser.First(u => u.Id == leaderRole.ProjectUserId);
            }
        }

        public void AddDeveloperToProject(int userId, int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                int developerId = GetDeveloperId();
                var roles = context.Role.Where(
                    r => r.ProjectId == projectId && r.ProjectUserId == userId && r.Type == developerId);

                if (roles.Count() == 0)
                {
                    context.Role.Add(new Role()
                    {
                        ProjectUserId = userId,
                        ProjectId = projectId,
                        Type = developerId
                    });

                    context.SaveChanges();
                }
            }
        }

        public void RemoveDeveloperFromProject(int userId, int projectId)
        {
            // TODO: mi van az assigned task-okkal?
            using (var context = new ProjectManagerDBEntities())
            {
                int developerId = GetDeveloperId();
                var role = context.Role.FirstOrDefault(
                    r => r.ProjectId == projectId && r.ProjectUserId == userId && r.Type == developerId);

                if (role != null)
                    context.Role.Remove(role);

                context.SaveChanges();
            }
        }
    }
}
