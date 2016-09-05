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
            //createRolesAndUsers();
        }

        //In this method the default admin role will be create
        private void createRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            //Create the role admin
            var roleAdmin = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (! roleAdmin.RoleExists("Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleAdmin.Create(role);

                //Creating the admin User
                var user = new ApplicationUser();
                //user.FirstName = "Fran";
                //user.LastName = "Gonzalez";
                user.Email = "gonzalezfrancis@hotmail.com";
                user.PasswordHash = "";
                user.EmailConfirmed = true;

                string userPWD = "Fmgg@2284";
                var chkUser = UserManager.Create(user, userPWD);

                //Add default user role admin
                if(chkUser.Succeeded)
                {
                    var result = UserManager.AddToRole(user.Id, "Admin");
                }
            }
        }
    }
}
