using Alfred.Model.Core;

namespace Alfred.Client.Core
{
    public class Interface
    {
        public void ReloadPlugins()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Interface_ReloadPlugins"
            });
        }

        public void RestartPlayer()
        {
            var task = new AlfredTask
            {
                Command = "Interface_RestartPlayer",
                Type = TaskType.Interface
            };

            AlfredPluginsWebsocket.Client.SendCommand(task);
        }

        public void RestartWebsite()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Interface_RestartWebsite",
                Type = TaskType.Interface
            });
        }

        public void ReloadSpeech()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_ReloadSpeech",
                Type = TaskType.Server
            });
        }

        public void ToggleRecognition()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Interface_StartStopRecognition",
                Type = TaskType.Interface
            });
        }

        public void ToggleKinect()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Interface_StartStopKinect",
                Type = TaskType.Interface
            });
        }
    }
}
