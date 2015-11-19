using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using System.Diagnostics;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        // GET: Projects
        public ActionResult Index()
        {
            int userId = 1;

            List<Project> projects = new List<Project>();
            using (var db = new ProjectManagerDBEntities())
            {
                var res = from r in db.Role
                          select r;

                foreach (var v in res)
                {
                    Debug.WriteLine("res: " + v.id);
                }
                
               // projects = res.ToList();
            }
                
            return View(projects);
        }
    }
}