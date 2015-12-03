using System.Web.Http;
using Alfred.Utils.Synology;

namespace Alfred.Server.WebApi.Controllers
{
    public class TorrentController : ApiController
    {
        [HttpGet]
        [Route("torrent/download")]
        public bool AddTorrent(string magnet)
        {
            var valid = TorrentModel.AddUrl(magnet);
            return valid;
        }
    }
}
