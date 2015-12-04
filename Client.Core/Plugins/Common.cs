
using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
    public class Common
    {
		public void Time()
		{
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Common_Time"
            });
		}

		public void VolumeDown()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Common_VolumeDown"
            });
        }

		public void VolumeUp()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Common_VolumeUp"
            });
        }

		public void Volume(float level, int channel = 0)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Common_Volume",
                Arguments = new Dictionary<string, string>
                {
                    { "volume", level.ToString() },                    
                    { "channel", channel.ToString() }
                }
            });
		}

        public static void GetVolume()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Common_GetVolume"
            });
        }

        public void Dialog(string recognizedText)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Common_Dialog",
                Arguments = new Dictionary<string, string>
                {
                    { "recognizedText", recognizedText }
                }
            });
        }
	}
}
