using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class Role
    {
        public Role()
        {

        }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        //Relationships
        public virtual ICollection<Movie> Movies { get; set; }
    }
}