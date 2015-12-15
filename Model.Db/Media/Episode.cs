using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Episode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string ShowName { get; set; }

        [Required]
        [StringLength(256)]
        public string Path { get; set; }
        public byte SeasonNumber { get; set; }
    }
}
