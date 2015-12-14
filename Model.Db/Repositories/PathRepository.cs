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
    public class PathRepository
    {
        public IDictionary<string, string> GetPaths()
        {
            using (var db = new AlfredContext())
            {
                return db.Paths.ToDictionary(p => p.Name, p => p.Value);
            }
        }

        public Path Get(string name)
        {
            using (var db = new AlfredContext())
            {
                return db.Paths.Single(p => p.Name == name);
            }
        }

        public void SavePaths(Dictionary<string, string> paths)
        {
            using (var db = new AlfredContext())
            {
                var pathsDb = db.Paths;
                foreach (var item in paths)
                {
                    var existingPath = pathsDb.SingleOrDefault(p => p.Name == item.Key);
                    if (existingPath == null)
                        pathsDb.Add(new Path { Name = item.Key, Value = item.Value });
                    else
                        existingPath.Value = item.Value;
                }
                db.SaveChanges();
            }
        }
    }
}
