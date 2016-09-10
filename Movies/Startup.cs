using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Movies.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(Movies.Startup))]
namespace Movies
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //TODO: set up roles to admin users
            createRolesAndUsers();
        }

        //In this method the default admin role will be create
        private async void createRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            //Create the role admin
            var roleAdmin = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleAdmin.RoleExists("Admin"))
            {
                //Create Role Admin
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                var chkRole = await roleAdmin.CreateAsync(role);

                //Creating the admin User
                var user = new ApplicationUser();
                user.FirstName = "Fran";
                user.LastName = "Gonzalez";
                user.UserName = "gonzalezfrancis@hotmail.com";
                user.Email = "gonzalezfrancis@hotmail.com";
                
                user.EmailConfirmed = true;

                string userPWD = "Fmgg@2284";
                var chkUser = await UserManager.CreateAsync(user, userPWD);

                //Create Role SuperAdmin
                var role2 = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role2.Name = "SuperAdmin";
                var chkRole2 = await roleAdmin.CreateAsync(role2);

                //Add default user role admin
                if (chkUser.Succeeded && chkRole.Succeeded && chkRole2.Succeeded)
                {
                    var result1 = await UserManager.AddToRoleAsync(user.Id, "Admin");
                    var result2 = await UserManager.AddToRoleAsync(user.Id, "SuperAdmin");
                }
            }
        }
    }
}
