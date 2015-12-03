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
        public string Name { get; set;}
        [ForeignKey("Playlist")]
        public int? Playlist_Id { get; set; }
        public virtual Playlist Playlist { get; set; }
        public virtual ICollection<ScenarioTime> Times { get; set; }
        public string Genre { get; set;}
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string Lights { get; set; }
        public string Radio { get; set; }
        public string RadioUrl { get; set; }

        public Scenario()
        {
            this.Times = new HashSet<ScenarioTime>();
        }
    }
}
