using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class UserRoleViewModel
    {
        public List<ApplicationUserRole> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }

    //This class represent the application user including a role field for each user
    public class ApplicationUserRole : ApplicationUser
    {
        public string role {
            get
            {
                return role;
            } 
            set
            {
                role = "Admin";
            }
        }
    }
}