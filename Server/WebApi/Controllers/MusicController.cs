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
using Alfred.Server.Attributes;

namespace Alfred.Server.WebApi.Controllers
{
    [TokenAuthorize]
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
        public virtual IEnumerable<Model.Core.Music.Radio> RadioList()
        {
            return _musicRepo.GetAllRadios();
        }

        [HttpGet]
        [Route("radios/{basename}")]
        public virtual Radio Radio(string basename)
        {
            return _musicRepo.GetRadio(basename);
        }

        [HttpGet]
        [Route("radios/subradios/{name}")]
        public virtual IEnumerable<SubRadio> SubRadioList(string name)
        {
            var rDb = _musicRepo.GetRadio(name);
            var radio = ARadio.GetARadio(rDb);
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
