using BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class StatisticsManager
    {
        public List<Statistics> GetAllStatistics(int projectId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Statistics.Where(s => s.ProjectId == projectId).ToList();
            }
        }

        public List<StatisticsData> GetDataForStatistics(int statisticsId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.StatisticsData.Where(s => s.StatisticsId == statisticsId).ToList();
            }
        }
    }
}
