namespace ProjectManager.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using ProjectManager.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ProjectManager.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ProjectManager.Models.ApplicationDbContext";
        }

        protected override void Seed(ProjectManager.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);

            if (!context.Users.Any(u => u.UserName == "kbela"))
            {
                var user = new ApplicationUser()
                {
                    UserName = "kbela",
                    Email = "kbela@testmail.com",
                    EmailConfirmed = true,
                    ProjectUserId = 1
                };

                manager.Create(user, "password");
            }

            if (!context.Users.Any(u => u.UserName == "tuser"))
            {
                var user = new ApplicationUser()
                {
                    UserName = "tuser",
                    Email = "tuser@testmail.com",
                    EmailConfirmed = true,
                    ProjectUserId = 2
                };

                manager.Create(user, "password");
            }

            if (!context.Users.Any(u => u.UserName == "gjakab"))
            {
                var user = new ApplicationUser()
                {
                    UserName = "gjakab",
                    Email = "gjakab@testmail.com",
                    EmailConfirmed = true,
                    ProjectUserId = 3
                };

                manager.Create(user, "password");
            }

            if (!context.Users.Any(u => u.UserName == "njozsef"))
            {
                var user = new ApplicationUser()
                {
                    UserName = "njozsef",
                    Email = "njozsef@testmail.com",
                    EmailConfirmed = true,
                    ProjectUserId = 4
                };

                manager.Create(user, "password");
            }
        }
    }
}
