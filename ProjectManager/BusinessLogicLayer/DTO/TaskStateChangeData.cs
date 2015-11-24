using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    public class TaskStateChangeData
    {
        //public int ProjectUserId { get; set; }

        //public int TaskId { get; set; }

        //public DateTime Timestamp { get; set; }

        //public string Reason { get; set; }

        //public bool Accepted { get; set; }

        public int ProjectUserId { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
        public string StateName { get; set; }
    }
}
