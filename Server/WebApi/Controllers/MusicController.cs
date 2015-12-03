using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Alfred.Model.Core.Music;
using Alfred.Server.Properties;
using Alfred.Utils.Media;
using Alfred.Utils.Radios;
using Alfred.Model.Db;
using System;
using Alfred.Model.Db.Repositories;

namespace Alfred.Server.WebApi.Controllers
{
    public class MusicController : ApiController
    {
        private PlaylistRepository _playlistRepo;
        private MusicRepository _musicRepo;
        
        public MusicController()
        {
            _playlistRepo = new PlaylistRepository();
            _musicRepo = new MusicRepository();
        }

        [HttpGet]
        [Route("tournedisque")]
        public virtual IEnumerable<Song> TournedisqueList()
        {
            return Tournedisque.GetSongsFromWeb("");
        }

        [HttpGet]
        [Route("tournedisque/genres")]
        public virtual IDictionary<string, string> TournedisqueGenres()
        {
            return Tournedisque.GetGenres();
        }

        [HttpGet]
        [Route("tournedisque/genres/{genre}")]
        public virtual IEnumerable<Song> TournedisqueGenre(string genre)
        {
            return Tournedisque.GetSongsFromWeb(genre);
        }

        [HttpGet]
        [Route("radios")]
        public virtual IEnumerable<Radio> RadioList()
        {
            var radios = new List<ARadio>();
            foreach (SettingsProperty currentProperty in Settings.Default.Properties)
            {
                var radio = ARadio.GetARadio(currentProperty.Name);
                if (radio != null)
                    radios.Add(radio);
            }
            return radios;
        }

        [HttpGet]
        [Route("radios/{basename}")]
        public virtual Radio Radio(string basename)
        {
            var radio = ARadio.GetARadio(basename);
            return radio;
        }

        [HttpGet]
        [Route("radios/subradios/{name}")]
        public virtual IEnumerable<SubRadio> SubRadioList(string name)
        {
            var radio = ARadio.GetARadio(name);
            return radio.HasSubsetRadios ? radio.GetAllSubRadios() : new SubRadio[]{};
        }

        [HttpGet]
        [Route("playlists")]
        public virtual IEnumerable<PlaylistModel> Playlists()
        {
            return _playlistRepo.GetAll();
        }

        [HttpGet]
        [Route("music/genres")]
        public virtual IEnumerable<string> Genres()
        {
            return _musicRepo.GetAllGenres();
        }

        [HttpGet]
        [Route("music/artists")]
        public virtual IEnumerable<string> Artists(string genre = null)
        {
            return _musicRepo.GetAllArtists(genre);
        }

        [HttpGet]
        [Route("music/albums")]
        public virtual IEnumerable<string> Albums(string artist = null)
        {
            return _musicRepo.GetAllAlbum();
        }
    }
}
