using Tellstick;

namespace Alfred.Utils.Lights.Telldus
{
    public class TelldusLight
    {
        public static ConnectionDetails connection;

        static TelldusLight()
        {
            connection = new ConnectionDetails(
                "FEHUVEW84RAFR5SP22RABURUPHAFRUNU",
                "ZUXEVEGA9USTAZEWRETHAQUBUR69U6EF",
                "e1da4e4f8f2d705c23f24032bc984c9d050423c7a",
                "c8cd435512952def2c5d5b321a2f01f0");
        }
    }
}
