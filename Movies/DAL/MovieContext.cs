using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Movies.Models;

namespace Movies.DAL
{
    public class MovieContext : DbContext
    {
        public MovieContext() : base("MovieContext")
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Genre> Genres { get; set; }

        //public DbSet<MovieGenre> MovieGenres { get; set; }
        //public DbSet<MovieWorker> MovieWorkers { get; set; }
    }
}