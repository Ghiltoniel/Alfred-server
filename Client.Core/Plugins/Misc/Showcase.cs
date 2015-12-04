
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
    public class Showcase
	{
		public void ShowcaseToday()
		{
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Showcase_ShowcaseToday"
            });
		}

		public void ShowcaseAll()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Showcase_ShowcaseAll"
            });
		}
	}
}
