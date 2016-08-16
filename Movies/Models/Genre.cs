using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class Genre
    {
        public Genre()
        {

        }
        public int GenreId { get; set; }
        public string GenreName { get; set; }

        //Relationship
        public virtual ICollection<Movie> Movies { get; set; }

    }
}