using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using BusinessLogicLayer;
using Microsoft.AspNet.Identity;
using ProjectManager.Models;
using App.Extensions;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        // GET: Projects
        public ActionResult Index()
        {
            int userId = int.Parse(User.Identity.GetProjectUserId()); 

            List<Project> projects = new List<Project>();
            using (var db = new ProjectManagerDBEntities())
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