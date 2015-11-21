using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    public class CommentData
    {
        public int ProjectUserId { get; set; }

        public int TaskId { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
