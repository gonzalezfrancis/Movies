using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class Movie
    {
        public Movie()
        {
            this.Workers = new HashSet<Worker>();
            this.Genres = new HashSet<Genre>();
            
        }
        [Key]
        public int MovieId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime  ReleaseDate { get; set; }

        public decimal Score { get; set; }

        public byte[] Cover { get; set; }

        public string Trailer { get; set; }

        //RelationShips
        public virtual ICollection<Worker> Workers { get; set; }
        public virtual ICollection<Genre> Genres { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}