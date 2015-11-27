using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class StatisticsController : Controller
    {
        private string tempChartPath = "~/Content/images/tempchart";

        // GET: /Statistics/CreateDialog
        public ActionResult CreateDialog(int Id)
        {
            ViewData["projectId"] = Id;
            return PartialView("_CreateDialog");
        }

        // GET: /Statistics/SaveDialog
        public ActionResult SaveDialog()
        {
            var model = ((StatisticsModel)TempData["UnsavedStatistics"]);

            return PartialView("_SaveDialog", model);
        }

        // GET: /Statistics/GetTempImage
        public ActionResult GetTempImage()
        {
            return base.File(tempChartPath, "jpeg");
        }

        // GET: /Statistics/GetImage
        public ActionResult GetImage(int Id)
        {
            var manager = new StatisticsManager();
            var stat = manager.GetStatistics(Id);

            var stream = new MemoryStream(stat.Chart.ToArray());

            return new FileStreamResult(stream, "image/jpeg");
        }

        // GET: /Statistics/SaveStatistics
        public ActionResult Save()
        {
            var manager = new StatisticsManager();
            var model = (StatisticsModel)TempData["model"];

            Image img = new Bitmap(Server.MapPath(tempChartPath));
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            var bytes = ms.ToArray();

            StatisticsData data = new StatisticsData()
            {
                ProjectId = model.ProjectId,
                Name = model.Name,
                CreatedDate = model.CreatedDate,
                Public = false,
                ImageContent = bytes
            };
            manager.AddNewStatistics(data);

            ms.Close();
            img.Dispose();
            var file = new FileInfo(Server.MapPath(tempChartPath));
            file.Delete();

            TempData["DetailsPage"] = "3";
            return Redirect("/Projects/Details/" + model.ProjectId);
        }

        // POST: /Statistics/Create
        [HttpPost]
        public ActionResult Create(int projectId)
        {
            string errors = string.Empty;

            var name = Request.Form["statisticsName"];
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;
            try
            {
                startTime = DateTime.Parse(Request.Form["startTime"]);
            }
            catch (Exception e)
            {

            }
            try
            {
                endTime = DateTime.Parse(Request.Form["endTime"]);
            }
            catch (Exception e)
            {

            }
            var measuredValue = Request.Form["measuredValue"];
            var aggregationType = Request.Form["aggregationType"];
            var createdDate = DateTime.Now;

            if (name == string.Empty)
            {
                errors = errors + "Statistics name can not be empty!";
            }

            if (errors != string.Empty)
            {
                TempData["errorMessage"] = errors;
                TempData["DetailsPage"] = "3";
                return Redirect("/Projects/Details/" + projectId);
            }

            string axisXTitle = string.Empty;
            string axisYTitle = string.Empty;
            List<object> xValues = new List<object>();
            List<object> yValues = new List<object>();

            // Get the X and Y axis data for the chart
            switch (measuredValue)
            {
                case "userWorktime":
                    {
                        axisXTitle = "Users";
                        axisYTitle = "Worktime in Hours";
                        BuildUserWorkTimeValues(xValues, yValues, startTime, endTime, projectId);
                        break;
                    }
                case "taskDoneWorktime":
                    {
                        axisXTitle = "Finished Tasks";
                        axisYTitle = "Worktime in Hours";
                        BuildTaskDoneWorktimeValues(xValues, yValues, startTime, endTime, projectId);
                        break;
                    }
                case "tasksDoneNumber":

                    {
                        axisYTitle = "Number of Finished Tasks";
                        BuildNumberOfFinishedTasksValues(xValues, yValues, startTime, endTime, projectId);
                        break;
                    }
                default: break;
            }

            if (aggregationType == "average")
            {
                axisXTitle = string.Format("{0} {1}", "Average ", axisXTitle);
                axisYTitle = string.Format("{0} {1}", "Average ", axisYTitle);

                var average = (int)Math.Round(yValues.Average(v => (int)v));
                yValues.Clear();
                yValues.Add(average);

                xValues.Clear();
                xValues.Add("Average");
            }

            var chart = BuildChart(name, startTime, endTime, xValues, yValues, axisXTitle, axisYTitle);
            chart.Save(tempChartPath, "jpeg");

            var model = new StatisticsModel()
            {
                ProjectId = projectId,
                Name = name,
                CreatedDate = createdDate,
                Chart = chart
            };

            TempData["DetailsPage"] = "3";
            TempData["UnsavedStatistics"] = model;
            return Redirect("/Projects/Details/" + projectId);
        }

        private void BuildUserWorkTimeValues(List<object> xValues, List<object> yValues, DateTime startTime, DateTime endTime,
            int projectId)
        {
            var managerProject = new ProjectUserManager();
            var managerTask = new TaskManager();

            var users = managerProject.GetUsersForProject(projectId);
            foreach (var u in users)
            {
                double seconds = 0.0;

                if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    seconds = managerTask.GetAllWorkTimeForUser(u.Id, projectId)
                        .Where(wt => wt.StartTime >= startTime && wt.EndTime <= endTime)
                        .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);
                }
                else if (startTime != DateTime.MinValue && endTime == DateTime.MinValue)
                {
                    seconds = managerTask.GetAllWorkTimeForUser(u.Id, projectId)
                        .Where(wt => wt.StartTime >= startTime)
                        .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);
                }
                else if (startTime == DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    seconds = managerTask.GetAllWorkTimeForUser(u.Id, projectId)
                        .Where(wt => wt.EndTime <= endTime)
                        .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);
                }
                else
                {
                    seconds = managerTask.GetAllWorkTimeForUser(u.Id, projectId)
                        .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);
                }

                xValues.Add(u.UserName);
                yValues.Add(((int)seconds / 3600));
            }
        }

        private void BuildTaskDoneWorktimeValues(List<object> xValues, List<object> yValues, DateTime startTime, DateTime endTime,
            int projectId)
        {
            var managerProject = new ProjectUserManager();
            var managerTask = new TaskManager();

            int doneId = managerTask.GetDoneStateId();
            var doneTasks = managerTask.GetTasksForProject(projectId).Where(t => t.State == doneId);
            foreach (var t in doneTasks)
            {
                double seconds = 0.0;

                if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    seconds = managerTask.GetAllWorkTimeForTask(t.Id)
                        .Where(wt => wt.StartTime >= startTime && wt.EndTime <= endTime)
                        .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);
                }
                else if (startTime != DateTime.MinValue && endTime == DateTime.MinValue)
                {
                    seconds = managerTask.GetAllWorkTimeForTask(t.Id)
                        .Where(wt => wt.StartTime >= startTime)
                        .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);
                }
                else if (startTime == DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    seconds = managerTask.GetAllWorkTimeForTask(t.Id)
                        .Where(wt => wt.EndTime <= endTime)
                        .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);
                }
                else
                {
                    seconds = managerTask.GetAllWorkTimeForTask(t.Id)
                        .Sum(w => w.EndTime.Subtract(w.StartTime).TotalSeconds);
                }

                xValues.Add(t.Name);
                yValues.Add(((int)seconds / 3600));
            }
        }

        private void BuildNumberOfFinishedTasksValues(List<object> xValues, List<object> yValues, DateTime startTime, DateTime endTime,
            int projectId)
        {
            var managerProject = new ProjectUserManager();
            var managerTask = new TaskManager();

            int count = 0;
            int doneId = managerTask.GetDoneStateId();
            var doneTasks = managerTask.GetTasksForProject(projectId).Where(t => t.State == doneId);
            foreach (var t in doneTasks)
            {
                var dtst = managerTask.GetDoneTaskStateChanges(t.Id);
                if (dtst.Count == 0)    // invalid test data
                    continue;

                var max = dtst.Max(st => st.Timestamp);
                var finalStateChange = dtst.SingleOrDefault(st => st.Timestamp == max);

                if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    if (finalStateChange.Timestamp >= startTime && finalStateChange.Timestamp <= endTime)
                        count++;
                }
                else if (startTime != DateTime.MinValue && endTime == DateTime.MinValue)
                {
                    if (finalStateChange.Timestamp >= startTime)
                        count++;
                }
                else if (startTime == DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    if (finalStateChange.Timestamp <= endTime)
                        count++;
                }
                else
                {
                    count++;
                }
            }

            xValues.Add("Finished Tasks");
            yValues.Add(count);
        }

        private Chart BuildChart(string name, DateTime startTime, DateTime endTime, 
            List<object> x, List<object> y, string axisX, string axisY)
        {
            var chart = new Chart(300, 300);

            chart.AddTitle(string.Format("{0} {1} {2}",
                name,
                startTime != DateTime.MinValue ? "\nFrom: " + (startTime).ToString("yyyy.MM.dd H:mm"): string.Empty,
                endTime != DateTime.MinValue ? "\nUntil: " + (endTime).ToString("yyyy.MM.dd H:mm") : string.Empty
                ));
            chart.AddSeries(
                xValue: x,
                yValues: y);
            chart.SetXAxis(axisX);
            chart.SetYAxis(axisY);

            return chart;
        }

        public ActionResult TogglePublic(int statisticsId, int projectId)
        {
            new StatisticsManager().TogglePublic(statisticsId);

            TempData["DetailsPage"] = "3";
            return Redirect("/Projects/Details/" + projectId);
        }

        public ActionResult Delete(int statisticsId, int projectId)
        {
            new StatisticsManager().Delete(statisticsId);

            TempData["DetailsPage"] = "3";
            return Redirect("/Projects/Details/" + projectId);
        }
	}
}