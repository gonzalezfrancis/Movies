using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Web.Mvc;
using Movies.DAL;
using Movies.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using PagedList.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
//using System.Web.Script.Serialization;

namespace Movies.Controllers
{
    public class MoviesController : Controller
    {
        private MovieContext db = new MovieContext();

        // GET: Movies
        public ActionResult Index(int? selector)
        {
            DateTime now = DateTime.Now;
            switch(selector)
            {
                case 2: ViewBag.Title = "Latest Releases";
                    return View(db.Movies.Where(m => m.ReleaseDate <= now).Take(20).OrderByDescending(s => s.ReleaseDate).ToList());

                case 3:
                    ViewBag.Title = "Coming Soon";
                    return View(db.Movies.Where(m => m.ReleaseDate > now).Take(20).OrderByDescending(r => r.ReleaseDate).ToList());

                //If the page paramet is == null then use 1, second parameter is the page size
                default: ViewBag.Title = "Most Popular";
                    return View(db.Movies.OrderByDescending(i => i.Score).Take(20).ToList());
            }
            
            
        }

        //GET: Movies/Search
        //public ActionResult Search(int? page)
        //{

        //    return View();
        //}

       
        public ActionResult Search(int? page, string query)
        {
            List<Movie> search = new List<Movie>();
            //Search for Title name start with the query string
            if (!string.IsNullOrEmpty(query))
            {
                search = db.Movies.Where(s => s.Title.StartsWith(query)).ToList();
            }
            ViewBag.query = query;
            return View("Search", search.ToPagedList(page ?? 1, 5));
        }
        //GET: Movies/IndexAdmin
        [Authorize(Roles = "Admin")]
        public ActionResult IndexAdmin()
        {
            return View(db.Movies.ToList());
        }

        //GET: Movies/UserIndex
        [Authorize]
        public ActionResult UserIndex()
        { 
            //Check if the user is loged in
            if (User.Identity.IsAuthenticated)
            {
                //Get all favorites movies from the user
                string userId = User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(u => u.Id == userId);
                //Return the favorites movies for the user
                return View(user.Movies.ToList());
            }
            return View();
        }
        //GET: Partial view
        public ActionResult FavoritePartial()
        {
            return PartialView("_FavoritePartial");
        }
        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            //Send the complete List of Actors and Genren into the view 
            var workerGenre = new WorkeGenreViewModels
            {
                WorkersCheck = db.Workers.ToList(), GenreCheck = db.Genres.ToList()
            };
            ViewBag.WorkerGenre = workerGenre;
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Title, Description, Score, ReleaseDate, Trailer")]Movie movie, HttpPostedFileBase upload, string genreString, string workerString)
        {
            List<Genre> myGenre = new List<Genre>();
            List<Worker> myWorker = new List<Worker>();
            if (ModelState.IsValid && upload != null)
            {
                //This is the cover image
                using (var reader = new System.IO.BinaryReader(upload.InputStream))
                {
                    movie.Cover = reader.ReadBytes(upload.ContentLength);
                }
                //This are the keys for the Genres converted to int array
                int[] genreArray = Array.ConvertAll(genreString.Split(','), g => int.Parse(g));
                //This are the keys for the Genres converted to int array
                int[] workerArray = Array.ConvertAll(workerString.Split(','), w => int.Parse(w));
                foreach (int item in genreArray)
                {
                    Genre tempGenre = db.Genres.SingleOrDefault(g => g.GenreId == item);
                    myGenre.Add(tempGenre);
                }
                foreach(int item in workerArray)
                {
                    Worker tempWorker = db.Workers.SingleOrDefault(w => w.WorkerId == item);
                    myWorker.Add(tempWorker);
                }
                //copy the List of genres and workers into the Movie
                movie.Genres = myGenre;
                movie.Workers = myWorker; 
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("IndexAdmin");
            }

            return View(movie);
        }
        //GET: Movies/UserList
        [Authorize(Users = "gonzalezfrancis@hotmail.com")]
        public ActionResult UserList()
        {
            //Get the List of users
            List<ApplicationUser> users = db.Users.ToList();
            //Create a List of User with the role for the view Model
            List<UserRoleViewModel> userRoles = new List<UserRoleViewModel>();
            //Get the role of the user using id
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            
            foreach (var item in users)
            {
                //Create a User with role and add it to the List
                var userRole = new UserRoleViewModel
                {
                    Id = item.Id,
                    Email = item.Email,
                    EmailConfirmed = item.EmailConfirmed,
                    AccessFailedCount = item.AccessFailedCount,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    //Get the role for for the userId provided
                    role = UserManager.GetRoles(item.Id),
                };
                userRoles.Add(userRole);
            }
            //Return the Users with the roles
            return View(userRoles.OrderBy(u => u.FirstName).ToList());
        }
        // POST: Change Users Role
        public async Task<ActionResult> AddRole(string id)
        {
            var context = new ApplicationDbContext();
            //Create a user manager
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //Get the user
            //var user = db.Users.SingleOrDefault(u => u.Id == id);
            await UserManager.AddToRoleAsync(id, "Admin");
            //Get the List of users
            List<ApplicationUser> users = db.Users.ToList();
            //Create a List of User with the role for the view Model
            List<UserRoleViewModel> userRoles = new List<UserRoleViewModel>();

            foreach (var item in users)
            {
                //Create a User with role and add it to the List
                var userRole = new UserRoleViewModel
                {
                    Id = item.Id,
                    Email = item.Email,
                    EmailConfirmed = item.EmailConfirmed,
                    AccessFailedCount = item.AccessFailedCount,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    //Get the role for for the userId provided
                    role = await UserManager.GetRolesAsync(item.Id),
                };
                userRoles.Add(userRole);
            }
            return PartialView("_UsersPartial", userRoles.OrderBy(u => u.FirstName).ToList());
        }
        //POST: Remove role
        public async Task<ActionResult> RemoveRole(string id)
        {
            //Create a user manager
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //Get the user
            var user = db.Users.SingleOrDefault(u => u.Id == id);
            if(user.Email != "gonzalezfrancis@hotmail.com")
            {
                await UserManager.RemoveFromRoleAsync(id, "Admin");
            }
            //Get the List of users
            List<ApplicationUser> users = db.Users.ToList();
            //Create a List of User with the role for the view Model
            List<UserRoleViewModel> userRoles = new List<UserRoleViewModel>();

            foreach (var item in users)
            {
                //Create a User with role and add it to the List
                var userRole = new UserRoleViewModel
                {
                    Id = item.Id,
                    Email = item.Email,
                    EmailConfirmed = item.EmailConfirmed,
                    AccessFailedCount = item.AccessFailedCount,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    //Get the role for for the userId provided
                    role = await UserManager.GetRolesAsync(item.Id),
                };
                userRoles.Add(userRole);
            }
            return PartialView("_UsersPartial", userRoles.OrderBy(u => u.FirstName).ToList());
        }
        // GET: Movies/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "MovieId,Title,Description,ReleaseDate,Score,Trailer")] Movie movie, HttpPostedFileBase upload, string genreString, string workerString)
        {
            //Load the Genres for the movie very important in many to many relationship with the birdge table
            db.Movies.Attach(movie);
            await db.Entry(movie).Collection(g => g.Genres).LoadAsync();
            await db.Entry(movie).Collection(w => w.Workers).LoadAsync();
            
            //This are the keys for the Genres converted to int array
            if(genreString != null)
            {
                int[] genreArray = Array.ConvertAll(genreString.Split(','), g => int.Parse(g));
                movie.Genres.Clear();
                foreach (var item in genreArray)
                {
                    Genre myGenre = await db.Genres.SingleAsync(g => g.GenreId == item);
                    if (myGenre != null)
                    {
                        movie.Genres.Add(myGenre);
                    }
                }
            }
            if(workerString != null)
            {
                //This are the keys for the Genres converted to int array
                int[] workerArray = Array.ConvertAll(workerString.Split(','), w => int.Parse(w));

                movie.Workers.Clear();

                foreach (var item in workerArray)
                {
                    Worker myWorker = await db.Workers.SingleAsync(w => w.WorkerId == item);
                    if (myWorker != null)
                    {
                        movie.Workers.Add(myWorker);
                    }
                }
            }
            //This is the cover image
            if (upload != null)
            {
                using (var reader = new System.IO.BinaryReader(upload.InputStream))
                {
                    movie.Cover = reader.ReadBytes(upload.ContentLength);
                }
            }
            else if (upload == null)
            {
                movie.Cover = await db.Movies.Where(m => m.MovieId == movie.MovieId).Select(e => e.Cover).SingleAsync();
            }
            if (ModelState.IsValid)
            {    
                db.Entry(movie).State = EntityState.Modified;
                await db.SaveChangesAsync();
                
                return RedirectToAction("IndexAdmin");
            }
            return View(movie);
        }
        /////////Genres
        //GET:
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditGenre()
        {
            return View(await db.Genres.OrderBy(g => g.GenreName).ToListAsync());
        }
        //Delete Genre
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteGenre(int? id)
        {
            var genre = await db.Genres.SingleOrDefaultAsync(g => g.GenreId == id);
            db.Genres.Remove(genre);
            await db.SaveChangesAsync();
            return PartialView("_genrePartial", await db.Genres.OrderBy(g => g.GenreName).ToListAsync());
        }
        //Add genre and return partial view
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddGenre(string genre)
        {
            Genre myGenre = new Genre { GenreName = genre };
            db.Genres.Add(myGenre);
            await db.SaveChangesAsync();
            return PartialView("_genrePartial", await db.Genres.OrderBy(g => g.GenreName).ToListAsync());
        }
        //Save an edited genre
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SaveEdit(string id, string value)
        {
            int myId = Int32.Parse(id);
            Genre myGenre = await db.Genres.SingleOrDefaultAsync(g => g.GenreId == myId);
            myGenre.GenreName = value;
            await db.SaveChangesAsync();
            return PartialView("_genrePartial", await db.Genres.OrderBy(g => g.GenreName).ToListAsync());
        }
        ///Edit Actors(Workers)
        public async Task<ActionResult> EditWorker()
        {
            return View(await db.Workers.OrderBy(w => w.Name).ToListAsync());
        }
        //Delete worker
        public async Task<ActionResult> DeleteWorker(int? id)
        {
            Worker myWorker = await db.Workers.SingleOrDefaultAsync(w => w.WorkerId == id);
            db.Workers.Remove(myWorker);
            await db.SaveChangesAsync();
            return PartialView("_WorkerPartial", await db.Workers.OrderBy(w => w.Name).ToListAsync());
        }
        //Add worker
        public async Task<ActionResult> AddWorker (string firstName, string lastName)
        {
            Worker myWorker = new Worker { Name = firstName, LastName = lastName };
            db.Workers.Add(myWorker);
            await db.SaveChangesAsync();
            return PartialView("_WorkerPartial",await db.Workers.OrderBy(w => w.Name).ToListAsync());
        }
        public async Task<ActionResult> EditWorkerOrderBy (string selector)
        {
            switch(selector)
            {
                case "s2": return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("D") || w.Name.StartsWith("E") || w.Name.StartsWith("F")).ToListAsync());

                case "s3": return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("G") || w.Name.StartsWith("H") || w.Name.StartsWith("I")).ToListAsync());

                case "s4": return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("J") || w.Name.StartsWith("K") || w.Name.StartsWith("L")).ToListAsync());

                case "s5": return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("M") || w.Name.StartsWith("N") || w.Name.StartsWith("O")).ToListAsync());

                case "s6": return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("P") || w.Name.StartsWith("Q") || w.Name.StartsWith("R")).ToListAsync());

                case "s7": return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("S") || w.Name.StartsWith("T") || w.Name.StartsWith("U")).ToListAsync());

                case "s8": return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("V") || w.Name.StartsWith("W") || w.Name.StartsWith("X")).ToListAsync());

                case "s9": return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("Y") || w.Name.StartsWith("Z")).ToListAsync());

                case "s10": return PartialView("_WorkerPartial", await db.Workers.OrderBy(w => w.Name).ToListAsync());

                case "s11": return PartialView("_WorkerPartial", await db.Workers.OrderBy(w => w.LastName).ToListAsync());

                default: return PartialView("_WorkerPartial", await db.Workers.Where(w => w.Name.StartsWith("A") || w.Name.StartsWith("B") || w.Name.StartsWith("C")).ToListAsync());
            }
        }
        //GET: 
        public async Task<ActionResult> SaveWorker(string id, string firstName, string lastName)
        {
            int intId = Int32.Parse(id);
            Worker myWorker = await db.Workers.SingleOrDefaultAsync(w => w.WorkerId == intId);
            myWorker.Name = firstName;
            myWorker.LastName = lastName;
            await db.SaveChangesAsync();
            return PartialView("_WorkerPartial", await db.Workers.OrderBy(w => w.Name).ToListAsync());
        }
        // GET: Movies/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("IndexAdmin");
        }

        public async Task<JsonResult> getWorkersGenre(int? id)
        {
            //Check if requesting specific movie or the complete List
            var workerGenre = new WorkeGenreViewModels();
            if(id != null)
            {
                Movie myMovie = await db.Movies.SingleOrDefaultAsync(w => w.MovieId == id);
                workerGenre.WorkersCheck = myMovie.Workers.OrderBy(g => g.Name).ToList();
                workerGenre.GenreCheck = myMovie.Genres.OrderBy(g => g.GenreName).ToList();
            }
            else
            {
                workerGenre.WorkersCheck = await db.Workers.ToListAsync();
                workerGenre.GenreCheck = await db.Genres.OrderBy(g => g.GenreName).ToListAsync();
            }
            //Choose to ignore the Looping reference Many to many while serialing the object into Json format
            //Also specified with attribute [JsonIgnore] in the many to many tables
            var json = JsonConvert.SerializeObject(workerGenre, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            //Add allowget for security 
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        
        //GET: Movies/GetImage Image from the databse
        public ActionResult GetImage(int id)
        {
            byte[] image = db.Movies.SingleOrDefault(i => i.MovieId == id).Cover;
            return File(image, "image/jpg");
        }

        //Create a new Actor
        [HttpPost]
        public void postWorker(Worker jsonObj)
        {

            //Store the new Actor in the db
            db.Workers.Add(jsonObj);
            db.SaveChanges();
        }

        //Create a new Genre
        [HttpPost]
        public void postGenre(Genre jsonObj)
        {
            db.Genres.Add(jsonObj);
            db.SaveChanges();
        }

        //Autocomplete Search
        [HttpPost]
        public JsonResult SearchAutoComplete(string query)
        {
            List<Movie> search = new List<Movie>();
            //Search for Title name start with the query string
            if(!string.IsNullOrEmpty(query))
            {
                search = db.Movies.Where(s => s.Title.StartsWith(query)).ToList();
            }
            //Convert into JSON
            var json = JsonConvert.SerializeObject(search, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        ////Favorites for the user
        [Authorize]
        public JsonResult GetFavorites()
        {
            //List of favorites
            List<int> favorites = new List<int>();
            //Check if the user is loged in
            if (User.Identity.IsAuthenticated)
            {
                //Get all favorites movies from the user
                string userId = User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(u => u.Id == userId);
                //Select all movieId from the user favorites
                favorites = user.Movies.Select(m => m.MovieId).ToList();
            }
            var json = JsonConvert.SerializeObject(favorites, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }); 
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        //Add favorites in the user List
        [Authorize]
        public async Task AddFavorites(string movie)
        {
            int movieId = Convert.ToInt32(movie);
            //Get the current user
            string userId = User.Identity.GetUserId();
            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            //Get the movie
            var myMovie = await db.Movies.SingleOrDefaultAsync(m => m.MovieId == movieId);
            user.Movies.Add(myMovie);
            await db.SaveChangesAsync();
            
        }
        [Authorize]
        public async Task<ActionResult> DeleteFavorites(string movie)
        {
            int movieId = Int32.Parse(movie);
            //Get the current user
            string userId = User.Identity.GetUserId();
            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            //Get the movie
            var myMovie = await db.Movies.SingleOrDefaultAsync(m => m.MovieId == movieId);
            user.Movies.Remove(myMovie);
            await db.SaveChangesAsync();
            //Return the partial view to update favorites
            return PartialView("_FavoritePartial", user.Movies.OrderBy(m => m.Title).ToList());
        }
        [Authorize]
        public async Task<ActionResult> FavoriteOrderBy(int? selector)
        {
            string userId = User.Identity.GetUserId();
            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            switch(selector)
            {
                case 2: return PartialView("_FavoritePartial", user.Movies.OrderByDescending(m => m.ReleaseDate).ToList());

                case 3: return PartialView("_FavoritePartial", user.Movies.OrderByDescending(m => m.Score).ToList());

                default: return PartialView("_FavoritePartial", user.Movies.OrderBy(m => m.Title).ToList());
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
