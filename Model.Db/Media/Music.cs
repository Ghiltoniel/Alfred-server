using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class Music
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Path { get; set; }
        public string Genre { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }

        public ICollection<MusicPlaylist> Playlists { get; set; }

        public Music()
        {
            this.Playlists = new HashSet<MusicPlaylist>();
        }
    }
}
