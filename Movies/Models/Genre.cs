using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

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
        [JsonIgnore]
        public virtual ICollection<Movie> Movies { get; set; }

    }
}