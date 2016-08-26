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
using PagedList.Mvc;
//using System.Web.Script.Serialization;

namespace Movies.Controllers
{
    public class MoviesController : Controller
    {
        private MovieContext db = new MovieContext();

        // GET: Movies
        public ActionResult Index(int? page)
        {
            //If the page paramet is == null then use 1, second parameter is the page size
            return View(db.Movies.ToList().ToPagedList(page ?? 1, 5));
        }

        //GET: Movies/IndexAdmin
        public ActionResult IndexAdmin()
        {
            return View(db.Movies.ToList());
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
        public ActionResult Create([Bind(Include = "Title, Description, Score, ReleaseDate, Trailer")]Movie movie, HttpPostedFileBase upload, string genreString, string workerString)
        {
            List<Genre> myGenre = new List<Genre>();
            List<Worker> myWorker = new List<Worker>();
            if (ModelState.IsValid)
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
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        // GET: Movies/Edit/5
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
        public ActionResult Edit([Bind(Include = "MovieId,Title,Description,ReleaseDate,Score,Cover,Trailer")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult getWorkersGenre()
        {
            var workerGenre = new WorkeGenreViewModels
            {
                WorkersCheck = db.Workers.ToList(),
                GenreCheck = db.Genres.ToList()
            };
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
        public JsonResult search(string query)
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
