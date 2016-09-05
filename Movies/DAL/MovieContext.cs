using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Movies.Models;

namespace Movies.DAL
{
    public class MovieContext : IdentityDbContext<ApplicationUser>
    {
        public MovieContext() : base("DefaultConnection")
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Genre> Genres { get; set; }

        //It does not need to create joining tables in many to many, EF does it automatically
        //public DbSet<MovieGenre> MovieGenres { get; set; }
        //public DbSet<MovieWorker> MovieWorkers { get; set; }
    }
}