using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<MusicPlaylist> Playlists { get; set; }
        public virtual ICollection<Scenario> Scenarios { get; set; }

        public Playlist()
        {
            this.Playlists = new HashSet<MusicPlaylist>();
            this.Scenarios = new HashSet<Scenario>();
        }
    }
}
