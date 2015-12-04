using Alfred.Client.Core.Plugins;

namespace Alfred.Client.Core
{
    public class AlfredPluginsWebsocket
    {
        public static WebSocketClient Client;
        public Velib Velib;
        public Lights Lights;
        public Sensor Sensor;
        public Player Player;
        public Mail Mail;
        public Common Common;
        public MediaManager MediaManager;
        public Multiroom Multiroom;
        public Scenario Scenario;
        public Interface Interface;
        public Playlist Playlist;
        public Plugins.Alfred Alfred;

        public AlfredPluginsWebsocket(WebSocketClient wsClient)
        {
            Client = wsClient;
            Velib = new Velib();
            Lights = new Lights();
            Sensor = new Sensor();
            Player = new Player();
            Mail = new Mail();
            Common = new Common();
            MediaManager = new MediaManager();
            Multiroom = new Multiroom();
            Interface = new Interface();
            Scenario = new Scenario();
            Playlist = new Playlist();
            Alfred = new Plugins.Alfred();
        }
    }
}
