using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Linq;
using WebApplication3.Models;



[assembly: OwinStartupAttribute(typeof(WebApplication3.Startup))]
namespace WebApplication3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            createRolesAndUsers();
        }

        private void createRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "mdespotovic333@gmail.com";
                user.Email = "mdespotovic333@gmail.com";
                user.FirstName = "Milica";
                user.LastName = "Despotovic";
                user.Role = 1;
                user.NumberOfTokens = 0;

                string adminPassword = "123123";

                var admin = UserManager.Create(user, adminPassword);

                if (admin.Succeeded)
                {
                    var adminAdded = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }

            dm150321db db = new dm150321db();

            if (!db.InformationsForAdministrator.Any())
            {
                var parameter = new InformationsForAdministrator();
                parameter.Time = 10;
                // ovo izbrisati kasnije 
                parameter.ItemsPerPage = 10;
                parameter.GoldPack = 50;
                parameter.SilverPack = 30;
                parameter.PlatinumPack = 100;
                parameter.ValueToken = 10;
                parameter.Currency = "RSD";
                db.InformationsForAdministrator.Add(parameter);

                db.SaveChanges();
            }

        }



   
}
}
