using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Episode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ShowName { get; set; }
        [Required]
        public string Path { get; set; }
        public byte SeasonNumber { get; set; }
    }
}
