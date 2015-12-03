using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class MusicPlaylist
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }

        [ForeignKey("Playlist")]
        public int Playlist_Id { get; set; }
        public virtual Playlist Playlist { get; set; }

        [ForeignKey("Music")]
        public int Music_Id { get; set; }
        public virtual Music Music { get; set; }
    }
}
