using Alfred.Model.Core;

namespace Alfred.Client.Plugins
{
    public class Velib
    {
        public void GetNearestAvailabilities()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Velib_GetNearestAvailabilities"
            });
        }
    }
}
