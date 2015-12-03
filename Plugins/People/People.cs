using System.Collections.Generic;
using Alfred.Model.Core;
using Alfred.Utils.Plugins;
using Alfred.Utils.Server;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Alfred.Plugins
{
    public class People : BasePlugin
    {
        public override void Initialize()
        {
            ServerData.Peoples.CollectionChanged += PeoplesOnCollectionChanged;
        }

        public void Broadcast()
        {
            WebSocketServer.Broadcast(new AlfredTask
            {
                Command = "People_List",
                Arguments = new Dictionary<string, string>
                {
                    { "people", JsonConvert.SerializeObject(ServerData.Peoples) }
                }
            });
        }

        private void PeoplesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Broadcast();
            foreach (var p1 in args.NewItems)
            {
                var p = p1 as Utils.Server.People;
                p.PropertyChanged += POnPropertyChanged;
            }
        }

        private void POnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Broadcast();
        }
    }
}
