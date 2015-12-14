using Alfred.Model.Core.Light;
using Alfred.Model.Core.Scenario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Db.Repositories
{
    public class PlayerRepository
    {
        public IEnumerable<Player> Getall()
        {
            using (var db = new AlfredContext())
            {
                return db.Players.ToList();
            }
        }

        public void DeleteAll()
        {
            using (var db = new AlfredContext())
            {
                foreach (var player in db.Players)
                    db.Players.Remove(player);
                db.SaveChanges();
            }
        }
    }
}
