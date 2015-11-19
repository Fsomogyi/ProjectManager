using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ProjectUserManager
    {
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
    }
}
