using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Movies.Models
{
    public class Worker
    {
        
        public Worker()
        {
            
            this.Movies = new HashSet<Movie>();
            
        }
        public int WorkerId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        //Relationship
        [JsonIgnore]
        public virtual ICollection<Movie> Movies { get; set; }
        
    }
}