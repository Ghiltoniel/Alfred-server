using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Core.Light
{
    public class LightInterfaceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<ConfigurationModel> Configurations { get; set; }
    }
}
