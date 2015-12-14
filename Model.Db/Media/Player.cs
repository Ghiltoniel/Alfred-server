using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        [StringLength(256)]
        public string Name { get; set; }
        public int Channel { get; set; }
    }
}
