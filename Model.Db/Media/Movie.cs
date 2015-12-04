using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Path { get; set; }
        [StringLength(256)]
        public string Genre { get; set; }
        [StringLength(256)]
        public string Title { get; set; }
    }
}
