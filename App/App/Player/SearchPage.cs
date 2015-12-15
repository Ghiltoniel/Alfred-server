using System;
using Alfred.Model.Core.Music;
using Xamarin.Forms;

namespace Alfred.App.Player
{
    public class SearchPage : ContentPage
    {
        AbsoluteLayout layout;
        ActivityIndicator loader;

        public SearchPage()
        {
            BindingContextChanged += SearchPage_BindingContextChanged;

            layout = new AbsoluteLayout
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            loader = new ActivityIndicator
            {
                IsEnabled = true,
                Color = Color.Gray,
                IsVisible = true,
                IsRunning = true
            };

            // PositionProportional flag maps the range (0.0, 1.0) to
            // the range "flush [left|top]" to "flush [right|bottom]"

            AbsoluteLayout.SetLayoutFlags(loader,
                AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(loader,
                new Rectangle(0.5,
                    0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            layout.Children.Add(loader);

            Content = layout;
        }

        async void SearchPage_BindingContextChanged(object sender, EventArgs e)
        {
            var text = BindingContext as string;
            var results = await App.client.Http.Music.SearchGrooveshark(text); 
            
            ListView listView = new ListView
            {
                // Source of data items.
                ItemsSource = results,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "SongName");

                    Label artistLabel = new Label
                    {
                        TextColor = Color.FromHex("#AA5566"),
                        Font = Font.SystemFontOfSize(11)
                    };
                    artistLabel.SetBinding(Label.TextProperty, "ArtistName");

                    // Return an assembled ViewCell.
                    var cell = new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(10, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children = 
                            {
                                new StackLayout
                                {
                                    VerticalOptions = LayoutOptions.Center,
                                    Spacing = 0,
                                    Children = 
                                    {
                                        nameLabel,
                                        artistLabel
                                    }
                                }
                            }
                        }
                    };
                    cell.Tapped += cell_Tapped;
                    return cell;
                })
            };

            // Accomodate iPhone status bar.
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 5);

            // Build the page.
            if (layout.Children.Count == 2)
                layout.Children.RemoveAt(1);
            layout.Children.Add(listView);
            loader.IsRunning = false;
            loader.IsVisible = false;
        }

        async void cell_Tapped(object sender, EventArgs e)
        {
            var song = (sender as Cell).BindingContext as GroovesharkSong;

            var choice = await DisplayActionSheet("Choose an option", "Cancel", null, "Direct Play", "Add to end");

            switch (choice)
            {
                case "Direct Play":
                    App.client.Websocket.MediaManager.DirectPlay(song.ArtistName, song.AlbumName, song.SongName, song.Url);
                    break;
                case "Add to end":
                    App.client.Websocket.MediaManager.AddToPlaylist(song.ArtistName, song.AlbumName, song.SongName, song.Url);
                    break;
            }
        }
    }
}

