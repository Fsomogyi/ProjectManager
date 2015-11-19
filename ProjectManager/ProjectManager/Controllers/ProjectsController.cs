using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using BusinessLogicLayer;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        // GET: Projects
        public ActionResult Index()
        {
            int userId = 1;

            List<Project> projects = new List<Project>();
            using (var db = new BusinessLogicLayerEntities())
            {
                var res = from project in db.Project
                          join role in db.Role on project.Id equals role.ProjectId
                          where role.ProjectUserId == userId
                          select project;
                projects = res.ToList();
            }
                
            return View(projects);
        }
    }
}