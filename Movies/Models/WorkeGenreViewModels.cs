using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class WorkeGenreViewModels
    {
        public List<Worker> WorkersCheck { get; set; } 
        public List<Genre> GenreCheck { get; set; }
        
    }
}