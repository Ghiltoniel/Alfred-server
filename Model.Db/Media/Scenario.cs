using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Scenario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set;}
        [ForeignKey("Playlist")]
        public int? Playlist_Id { get; set; }
        public virtual Playlist Playlist { get; set; }
        public virtual ICollection<ScenarioTime> Times { get; set; }
        [StringLength(256)]
        public string Genre { get; set; }
        [StringLength(256)]
        public string Artist { get; set; }
        [StringLength(256)]
        public string Album { get; set; }
        [StringLength(256)]
        public string Title { get; set; }
        [StringLength(3000)]
        public string Lights { get; set; }
        [StringLength(256)]
        public string Radio { get; set; }
        [StringLength(256)]
        public string RadioUrl { get; set; }

        public Scenario()
        {
            this.Times = new HashSet<ScenarioTime>();
        }
    }
}
