
using System.Collections.Generic;

namespace Alfred.Model.Core
{
    public enum TaskType
    {
        Server,
        Interface,
        Alfred,
        ActionPlayer,
        WebsiteInfo
    };

    public class AlfredTask
    {
        public string Command;
        public Dictionary<string, string> Arguments;
        public Client Client;
        public TaskType Type;
        public string FromName;
        public string Token;
        public bool SpeakBeforeExecute;
        public bool SpeakAfterExecute;

        public AlfredTask()
        {
            SpeakBeforeExecute = false;
            SpeakAfterExecute = false;
        }
    }
}
