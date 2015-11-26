using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    public class StatisticsData
    {
        public int ProjectId { get; set; }

        public string Name { get; set; }

        public bool Public { get; set; }

        public DateTime CreatedDate { get; set; }

        public byte[] ImageContent { get; set; }
    }
}
