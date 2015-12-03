using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class ShopItem
    {
        [Key]
        public int Id { get; set; }
        public string Product { get; set; }
        public virtual User User { get; set; }
    }
}
