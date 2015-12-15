using System;
using System.Collections;
using Xamarin.Forms;

namespace Alfred.App.Player
{
    public class PlaylistPage : ContentPage
    {
        AbsoluteLayout layout;
        ActivityIndicator loader;
        ControlStatus status;

        public PlaylistPage()
        {
            BindingContextChanged += PlaylistPage_BindingContextChanged;
            Appearing += PlaylistPage_BindingContextChanged;

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

        public async void PlaylistPage_BindingContextChanged(object sender, EventArgs e)
        {
            loader.IsRunning = true;
            loader.IsVisible = true;

            status = BindingContext as ControlStatus;
            
            ListView listView = new ListView
            {
                // Source of data items.
                ItemsSource = status.Playlist,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Value.title");
                    nameLabel.BindingContextChanged += label_BindingContextChanged;

                    Label artistLabel = new Label
                    {
                        TextColor = Color.FromHex("#AA5566"),
                        Font = Font.SystemFontOfSize(11)
                    };
                    artistLabel.SetBinding(Label.TextProperty, "Value.artist");
                    artistLabel.BindingContextChanged += label_BindingContextChanged;

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

                    cell.BindingContextChanged += cell_BindingContextChanged;

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

        void label_BindingContextChanged(object sender, EventArgs e)
        {
            var label = sender as Label;
            var col = ((DictionaryEntry)(label.BindingContext)).Key;
            if ((int)col == status.Status.CurrentPlaylistIndex)
                label.TextColor = Color.White;
        }

        void cell_BindingContextChanged(object sender, EventArgs e)
        {
            var cell = sender as ViewCell;
            var col = ((DictionaryEntry)(cell.BindingContext)).Key;
            
            cell.View.BackgroundColor = (int)col == status.Status.CurrentPlaylistIndex ? Color.Green : Color.Transparent;
        }

        async void cell_Tapped(object sender, EventArgs e)
        {
            var song = (DictionaryEntry)((sender as Cell).BindingContext);
            App.client.Websocket.MediaManager.GoPlaylist((int)song.Key);
        }
    }
}

