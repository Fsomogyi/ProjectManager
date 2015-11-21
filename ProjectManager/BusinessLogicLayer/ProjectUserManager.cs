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
        public int GetLeaderId()
        {
            return 2;
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
    }
}
