using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Path
    {
        [Key]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
