using System;
using System.Collections.Generic;
using System.Linq;
using Alfred.Model.Core.Music;
using Xamarin.Forms;

namespace Alfred.App.Player
{
    class TournedisqueGenrePage : ContentPage
    {
        Grid grid;
        Label titre;
        AbsoluteLayout layout;
        ActivityIndicator loader;
        private double allocatedHeight;

        public TournedisqueGenrePage()
        {
            Appearing += TournedisqueGenrePage_Appearing;
            this.SetBinding(TitleProperty, "Name");

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

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            allocatedHeight = height;
        }

        async void TournedisqueGenrePage_Appearing(object sender, EventArgs e)
        {
            if (grid != null)
                return;

            grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Padding = new Thickness(10, 10, 10, 10)
            };

            var songs = await App.client.Http.Music.GetTournedisqueGenre((BindingContext as TournedisqueGenre).Url);
            grid.RowDefinitions = new RowDefinitionCollection();
            foreach (var song in Enumerable.Range(0, songs.Count() / 3))
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(130, GridUnitType.Absolute) });

            int row = 0, column = 0;
            foreach (var song in songs.Where(s => !string.IsNullOrEmpty(s.Link)
                && !string.IsNullOrEmpty(s.Artist)
                && !string.IsNullOrEmpty(s.Title)
                && !string.IsNullOrEmpty(s.Album)))
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += tapGestureRecognizer_Tapped;
                tapGestureRecognizer.CommandParameter = songs.SkipWhile(s=>s != song);
                var stack =
                    new StackLayout
                    {
                        Padding = new Thickness(3, 3),
                        Orientation = StackOrientation.Vertical,
                        Children = 
                            {
                                new StackLayout
                                {
                                    VerticalOptions = LayoutOptions.Center,
                                    Spacing = 0,
                                    Children = 
                                    {
                                        new Image
                                        {
                                            Source = ImageSource.FromUri(new Uri(song.Album)),
                                            HorizontalOptions = LayoutOptions.Center,
                                            WidthRequest = 80
                                        },
                                        new Label
                                        {
                                            Text = song.Title,
                                            HorizontalOptions = LayoutOptions.Center,
                                            Font = Font.SystemFontOfSize(10),
                                            TextColor = Color.Black
                                        },
                                        new Label
                                        {
                                            Text = song.Artist,
                                            HorizontalOptions = LayoutOptions.Center,
                                            Font = Font.SystemFontOfSize(10)
                                        }
                                    }
                                }
                            }
                    };

                stack.GestureRecognizers.Add(tapGestureRecognizer);
                grid.Children.Add(stack, column, row);

                row = row + (column + 1) / 3;
                column++;
                column %= 3;
            }

            var items = new ScrollView
            {
                Content = grid,
                WidthRequest = 360,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = allocatedHeight
            };

            AbsoluteLayout.SetLayoutFlags(items,
                AbsoluteLayoutFlags.None);

            AbsoluteLayout.SetLayoutBounds(items,
                new Rectangle(0, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            
            layout.Children.Add(items);
            loader.IsRunning = false;
            loader.IsVisible = false;
        }

        async void tapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var songs = (e as TappedEventArgs).Parameter as IEnumerable<Song>; 
            var choice = await DisplayActionSheet("Choose an option", "Cancel", null, "Direct Play", "Add to end");

            switch (choice)
            {
                case "Direct Play":
                    App.client.Websocket.MediaManager.SetSongsPlaylist(songs);
                    break;
                case "Add to end":
                    App.client.Websocket.MediaManager.AddSongsToPlaylist(songs);
                    break;
            }
        }
    }
}
