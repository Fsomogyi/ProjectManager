using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.TESTING
{
    public class ProjectUserManager
    {
        public List<ProjectUserData> GetAllProjectUserData()
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var allProjectUsers = (from pu in context.ProjectUser
                                       select new ProjectUserData
                                       {
                                           Id = pu.id,
                                           Name = pu.username,
                                           Password = pu.password,
                                           Email = pu.email
                                       }).ToList();

                return allProjectUsers;
            }
        }
    }
}
