using System;
using System.Linq;
using Xamarin.Forms;

namespace Alfred.App.Player
{
    // Used in:
    //      MasterDetailPageDemoPage (as a page)
    //      TabbedPageDemoPage (as a page template)
    //      CarouselPageDemoPage (as a page template)
    //
    //  Expects BindingContext to be of type NamedColor!
    class TournedisquePage : TabbedPage
    {
        Grid grid;
        Label titre;

        public TournedisquePage()
        {
            var test = ToolbarItems;
            ToolbarItems.Add(new ToolbarItem
            {
                Name = "Tournedisque"
            });
            Appearing += TournedisquePage_Appearing;
        }

        async void TournedisquePage_Appearing(object sender, EventArgs e)
        {
            var genres = (await App.client.Http.Music.GetTournedisqueGenres()).Select(g => new TournedisqueGenre(g.Value, g.Key)).ToArray();
            ItemsSource = genres;

            ItemTemplate = new DataTemplate(typeof(TournedisqueGenrePage));
            if (genres.Any())
                SelectedItem = genres.FirstOrDefault();
        }
    }

    class TournedisqueGenre
    {
        public TournedisqueGenre(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { private set; get; }

        public string Url { private set; get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
