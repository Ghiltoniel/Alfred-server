using System.Collections.Generic;

namespace Alfred.Model.Core.WebSocket
{
    public interface IAlfredWebSocketServer
    {
        void Start(string adresse);
        void Broadcast(AlfredTask task);
        IEnumerable<Client> GetPlayersChannel(int channel);
    }
}
