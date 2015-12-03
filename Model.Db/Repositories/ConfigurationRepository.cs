using System.Linq;
using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Model.Db.Repositories
{
    public class ConfigurationRepository
    {
        public IEnumerable<ConfigurationModel> GetAll()
        {
            using(var db = new AlfredContext())
            {
                return db.Configurations.Select(c => new ConfigurationModel()
                    {
                        Name = c.Name,
                        Value = c.Value
                    }).ToList();
            }
        }

        public void Save(ConfigurationModel config)
        {
            using (var db = new AlfredContext())
            {
                db.Configurations.Add(new Configuration()
                    {
                        Name = config.Name,
                        Value = config.Value
                    });
                db.SaveChanges();
            }
        }
    }
}
