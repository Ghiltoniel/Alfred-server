using Alfred.Model.Core;

namespace Alfred.Client.Plugins
{
    public class People
    {
        public void GetAll()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "People_Broadcast"
            });
        }
    }
}
