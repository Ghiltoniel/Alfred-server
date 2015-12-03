using Alfred.Model.Db.Repositories;
using Alfred.Model.Core.WebSocket;
using Alfred.Utils.Server;

namespace Alfred.Utils.Managers
{
	public class Init
	{
        public static ServerStatus serverStatus;
        public static IAlfredWebSocketServer WebSocketServer;

		static Init()
        {
            serverStatus = new ServerStatus();
            new PlayerRepository().DeleteAll();
		}
	}
}
