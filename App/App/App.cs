using Alfred.Client.Core;
using Xamarin.Forms;

namespace Alfred.App
{
    public class App
    {
        public static AlfredClient client;

        public static Page GetMainPage()
        {
            var page = new PlayerPage();

            client = new AlfredClient(
                "test",
                "nam.kicks-ass.org",
                13100,
                "guigui",
                "Ghiltoniel1",
                page.OnReceive,
                page.OnDisconnect,
                page.OnConnect);

            return page;
        }

        public static void Volume(bool isUp)
        {
            if (isUp)
                client.Websocket.Common.VolumeUp();
            else
                client.Websocket.Common.VolumeDown();
        }
    }
}
