namespace Alfred.Plugins.Managers
{
    public class CommonPlugins
    {
        public static Alfred alfred;
        public static MediaManager media;

		static CommonPlugins()
        {
            alfred = new Alfred();
            media = new MediaManager();
		}
    }
}
