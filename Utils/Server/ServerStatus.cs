using System.Collections.Generic;
using System.Linq;
using Alfred.Model.Db;
using Alfred.Model.Db.Repositories;

namespace Alfred.Utils
{
    public class ServerStatus
    {
        public bool IsRecognitionUp;
        public bool IsServerUp;
        public bool IsKinectUp;
        public List<Player> Players;

        public ServerStatus()
        {
            IsRecognitionUp = false;
            IsServerUp = true;
            IsKinectUp = false;
            Players = new PlayerRepository().Getall().ToList();
        }
    }
}
