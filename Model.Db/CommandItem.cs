using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Db
{
    public class CommandItem
    {
        [Key]
        public int Id { get; set; }

        [StringLength(256)]
        public string Term { get; set; }

        [ForeignKey("Command")]
        public int CommandId { get; set; }
        public virtual Command Command { get; set; }
    }
}
