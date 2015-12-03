using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Path { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }
    }
}
