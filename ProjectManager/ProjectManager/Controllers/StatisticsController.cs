using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: /Statistics/CreateDialog
        public ActionResult CreateDialog(int Id)
        {
            ViewData["projectId"] = Id;
            return PartialView("_CreateDialog");
        }

        // POST: /Statistics/Create
        [HttpPost]
        public ActionResult Create(int projectId)
        {
            if (!ModelState.IsValid)
            {
                TempData["DetailsPage"] = "2";
                return Redirect("/Projects/Details/" + projectId);
            }
            TempData["DetailsPage"] = "2";

            return Redirect("/Projects/Details/" + projectId);
        }

        // GET: /Statistics/BuildChart
        public ActionResult BuildChart()
        {
            var chart = new Chart(500, 500);

            StatisticsListElement model = (StatisticsListElement)TempData["model"];

            var statistics = model.Statistics;
            var dataList = model.StatisticsDataList;

            chart.AddTitle(string.Format("{0} {1} {2}", 
                statistics.Name,
                statistics.StartTime != null ? ((DateTime)statistics.StartTime).ToString("yyyy.MM.dd") + " - " : string.Empty,
                statistics.EndTime != null ? ((DateTime)statistics.StartTime).ToString("yyyy.MM.dd") : string.Empty
                ));
            chart.AddLegend(statistics.LegendX);
            chart.AddSeries(yValues: dataList.Select(d => d.ValueY).ToList());

            var guid = Guid.NewGuid().ToString();

            chart.Save("~/Content/images/charts/chart-" + guid, "jpeg");
            return base.File("~/Content/images/charts/chart-" + guid, "jpeg");
        }
	}
}