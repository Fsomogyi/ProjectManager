using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    public class WorktimeData
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int ProjectUserId { get; set; }

        public int TaskId { get; set; }
    }
}
