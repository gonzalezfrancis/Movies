using System;
using System.Collections.Generic;
using System.Data.Entity;
using Movies.Models;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Drawing;
using System.IO;

namespace Movies.DAL
{
    public class MovieInitializer : DropCreateDatabaseIfModelChanges<MovieContext>
    {
        protected override void Seed(MovieContext context)
        {
            List<Movie> movies = new List<Movie>();
            List<Worker> workers = new List<Worker>();
            List<Role> roles = new List<Role>();
            List<Genre> genres = new List<Genre>();
            
            //Add worker
            var worker1 = new Worker
            {
                Name = "Brad",
                LastName = "Pitt",
                Movies = movies
            };
            var worker2 = new Worker
            {
                Name = "Eric",
                LastName = "Bana",
                Movies = movies
            };
            workers.Add(worker1);
            workers.Add(worker2);
            //Add Role
            var role1 = new Role
            {
                RoleName = "Actor",
                Movies = movies
            };
            var role2 = new Role
            {
                RoleName = "Director",
                Movies = movies
            };
            roles.Add(role1);
            roles.Add(role2);
            //Add Genre
            var genre1 = new Genre
            {
                GenreName = "Action",
                Movies = movies
            };
            var genre2 = new Genre
            {
                GenreName = "Romance",
                Movies = movies
            };
            genres.Add(genre1);
            genres.Add(genre2);
            //Add movie
            string path = HttpContext.Current.Server.MapPath("~/Images/Covers/Troy.jpg");
            var movie = new Movie
            {
                Title = "Troy",
                Description = "Troy is a 2004 American epic adventure war film written by David Benioff",
                ReleaseDate = DateTime.Now,
                Score = 5,
                Cover = File.ReadAllBytes(path),
                Trailer = "",
                Roles = roles,
                Genres = genres,
                Workers = workers
            };
            movies.Add(movie);
            //Add to the db
            context.Movies.Add(movie);
            context.Roles.Add(role1);
            context.Roles.Add(role2);
            context.Genres.Add(genre1);
            context.Genres.Add(genre2);
            context.Workers.Add(worker1);
            context.Workers.Add(worker2);
            context.SaveChanges();
            ////////////////////
            
        }
    }
}