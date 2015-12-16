using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class RadioDb
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string BaseUrl { get; set; }
        [Required]
        [StringLength(256)]
        public string BaseName { get; set; }
        [StringLength(256)]
        public string DisplayName { get; set; }
        [StringLength(256)]
        public string ThumbnailUrl { get; set; }
        public bool HasSubsetRadios { get; set; }

        public RadioDb()
        {
        }
    }
}
