using BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class CommentManager
    {
        public void AddComment(CommentData data)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                context.Comment.Add(new Comment()
                {
                    ProjectUserId = data.ProjectUserId,
                    TaskId = data.TaskId,
                    Content = data.Content,
                    Timestamp = data.Timestamp
                });

                context.SaveChanges();
            }
        }
    }
}
