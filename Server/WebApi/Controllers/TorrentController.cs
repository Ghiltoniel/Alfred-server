using System.Web.Http;
using Alfred.Utils.Synology;
using Alfred.Server.Attributes;

namespace Alfred.Server.WebApi.Controllers
{
    [TokenAuthorize]
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
