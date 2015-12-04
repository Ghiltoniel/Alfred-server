
using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
    public class Scenario
    {
        public void BroadcastScenarios()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Scenario_BroadcastScenarios"
            });
        }

        public void LaunchScenario(string name)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Scenario_LaunchScenario",
                Arguments = new Dictionary<string, string>
                {
                    { "mode", name }
                },
                SpeakBeforeExecute = true,
                SpeakAfterExecute = true
            });
        }
    }
}
