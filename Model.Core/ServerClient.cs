using System;
using Newtonsoft.Json;
using Alfred.Model.Core.WebSocket;
using Fleck2.Interfaces;

namespace Alfred.Model.Core
{
    /// <summary>
    /// Holds the name and context instance for an online user
    /// </summary>
    public class Client
    {
        public string Name = String.Empty;
        public bool IsPlayer = false;
        public int Ping = 0;
        public IWebSocketConnection Context { get; set; }
        public int Channel = 0;
        public bool ReadyToPlay = false;

        public void Send(AlfredTask task)
        {
            Context.Send(JsonConvert.SerializeObject(task));
        }
    }
}
