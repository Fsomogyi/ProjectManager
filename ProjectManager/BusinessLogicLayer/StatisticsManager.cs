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

        public Statistics GetStatistics(int statisticsId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                return context.Statistics.SingleOrDefault(s => s.Id == statisticsId);
            }
        }

        public void TogglePublic(int statisticsId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var statistics = context.Statistics.SingleOrDefault(s => s.Id == statisticsId);

                if (statistics != null)
                {
                    statistics.Public = !statistics.Public;
                    context.SaveChanges();
                }
            }
        }

        public void Delete(int statisticsId)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                var statistics = context.Statistics.SingleOrDefault(s => s.Id == statisticsId);

                if (statistics != null)
                {
                    context.Statistics.Remove(statistics);
                    context.SaveChanges();
                }
            }
        }

        public void AddNewStatistics(StatisticsData data)
        {
            using (var context = new ProjectManagerDBEntities())
            {
                Statistics stat = new Statistics()
                {
                    ProjectId = data.ProjectId,
                    Name = data.Name,
                    CreatedDate = data.CreatedDate,
                    Public = data.Public,
                    Chart = data.ImageContent
                };

                context.Statistics.Add(stat);
                context.SaveChanges();
            }
        }
    }
}
