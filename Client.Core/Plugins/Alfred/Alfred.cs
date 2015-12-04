
using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
	public class Alfred
    {
		public void Start()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Alfred_Start"
            });
		}

        public void StopListening()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Alfred_StopListening"
            });
        }

        public void Stop()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Alfred_Stop"
            });
        }

        public void PlayTempString(string sentence)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Alfred_PlayTempString",
                Arguments = new Dictionary<string, string>
                {
                    { "sentence", sentence }
                }
            });
        }

		public void Again()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Alfred_Again"
            });
		}
	}
}
