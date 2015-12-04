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
    class RadioPage : TabbedPage
    {
        Grid grid;
        Label titre;

        public RadioPage()
        {
            var test = ToolbarItems;
            ToolbarItems.Add(new ToolbarItem
            {
                Name = "Radios"
            });
            Appearing += RadioPage_Appearing;
        }

        async void RadioPage_Appearing(object sender, EventArgs e)
        {
            var radios = (await App.client.Http.Music.GetRadios()).ToArray();
            ItemsSource = radios;

            ItemTemplate = new DataTemplate(typeof(RadioGenrePage));
            SelectedItem = radios.FirstOrDefault();
        }
    }
}
