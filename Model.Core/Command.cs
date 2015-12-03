using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Core
{
    public class CommandModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ruleref { get; set; }
        public virtual ICollection<CommandItemModel> Items { get; set; }

        public CommandModel()
        {
            this.Items = new HashSet<CommandItemModel>();
        }
    }

    public class CommandItemModel
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public int CommandId { get; set; }
    }
}
