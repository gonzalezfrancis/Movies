using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class UserRoleViewModel : ApplicationUser
    {
        [Display(Name = "User Role")]
        public IList<string> role { get; set; }
    }
}