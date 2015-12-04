
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
    public class Mail
    {
        public void Count()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Mail_Count"
            });
        }
        public void ReadLast()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Mail_ReadLast"
            });
        }
    }
}
