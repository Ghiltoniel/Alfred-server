using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Configuration
    {
        [Key]
        public string Name { get; set; }

        [StringLength(256)]
        public string Value { get; set; }
    }
}
