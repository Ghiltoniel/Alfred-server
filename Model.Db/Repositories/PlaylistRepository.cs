using Alfred.Model.Core.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Db.Repositories
{
    public class PlaylistRepository
    {
        public IEnumerable<PlaylistModel> GetAll()
        {
            using(var db = new AlfredContext())
            {
                return db.Playlists.ToList().Select(p => new PlaylistModel()
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
            }
        }
    }
}
