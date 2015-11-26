using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace ProjectManager.Models
{
    public class StatisticsModel
    {
        public int ProjectId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public Chart Chart { get; set; }
    }
}