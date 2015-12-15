using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Db
{
    public class Command
    {
        [Key]
        public int Id { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Ruleref { get; set; }
        public virtual ICollection<CommandItem> Items { get; set; }

        public Command()
        {
            this.Items = new HashSet<CommandItem>();
        }
    }
}
