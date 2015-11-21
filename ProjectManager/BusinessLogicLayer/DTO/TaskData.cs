using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    public class TaskData
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public int EstimatedWorkHours { get; set; }

        public int? MaxDevelopers { get; set; }

        public int State { get; set; }
    }
}
