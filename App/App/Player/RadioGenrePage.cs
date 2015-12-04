using System;
using System.Linq;
using Alfred.Model.Core.Music;
using Xamarin.Forms;

namespace Alfred.App.Player
{
    class RadioGenrePage : ContentPage
    {
        Grid grid;
        Label titre;
        AbsoluteLayout layout;
        ActivityIndicator loader;

        public RadioGenrePage()
        {
            Appearing += RadioGenrePage_Appearing;
            this.SetBinding(TitleProperty, "DisplayName");

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

        async void RadioGenrePage_Appearing(object sender, EventArgs e)
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

            var radio = (BindingContext as Radio);
            if(!radio.HasSubsetRadios)
            {
                var button = new Button
                {
                        Image = new FileImageSource
                        {
                            File = "djamradio.jpg"
                        },
                        BackgroundColor = Color.Transparent,
                        BorderWidth = 0,
                        HorizontalOptions = LayoutOptions.Center
                    };
                button.CommandParameter = radio;
                button.Clicked += button_Clicked;
                AbsoluteLayout.SetLayoutFlags(button,
                AbsoluteLayoutFlags.PositionProportional);

                AbsoluteLayout.SetLayoutBounds(button,
                    new Rectangle(0.5,
                        0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                layout.Children.Add(button);
                loader.IsRunning = false;
                loader.IsVisible = false;
                return;
            }

            var subradios = await App.client.Http.Music.GetSubRadios(radio.BaseName);
            if (subradios == null)
                return;

            grid.RowDefinitions = new RowDefinitionCollection();
            foreach (var song in Enumerable.Range(0, subradios.Count() / 3))
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(130, GridUnitType.Absolute) });

            int row = 0, column = 0;
            foreach (var subradio in subradios.Where(s => !string.IsNullOrEmpty(s.Name)
                && !string.IsNullOrEmpty(s.Url)))
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += tapGestureRecognizer_Tapped;
                tapGestureRecognizer.CommandParameter = subradio;
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
                                            Source = ImageSource.FromUri(new Uri(subradio.ImgSrc)),
                                            HorizontalOptions = LayoutOptions.Center,
                                            WidthRequest = 80
                                        },
                                        new Label
                                        {
                                            Text = subradio.Name,
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
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            AbsoluteLayout.SetLayoutFlags(items,
                AbsoluteLayoutFlags.None);

            AbsoluteLayout.SetLayoutBounds(items,
                new Rectangle(0, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            
            layout.Children.Add(items);
            loader.IsRunning = false;
            loader.IsVisible = false;
        }

        async void button_Clicked(object sender, EventArgs e)
        {
            var radio = (sender as Button).CommandParameter as Radio;
            var choice = await DisplayActionSheet("Choose an option", "Cancel", null, "Direct Play", "Add to end");

            switch (choice)
            {
                case "Direct Play":
                    App.client.Websocket.MediaManager.DirectPlay(radio.DisplayName, null, null, radio.BaseUrl);
                    break;
                case "Add to end":
                    App.client.Websocket.MediaManager.AddToPlaylist(radio.DisplayName, null, null, radio.BaseUrl);
                    break;
            }
        }

        async void tapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var radio = (e as TappedEventArgs).Parameter as SubRadio;
            var choice = await DisplayActionSheet("Choose an option", "Cancel", "Cancel", "Direct Play", "Add to end");

            switch (choice)
            {
                case "Direct Play":
                    App.client.Websocket.MediaManager.DirectPlay(radio.RadioName, null, radio.Name, radio.Url);
                    break;
                case "Add to end":
                    App.client.Websocket.MediaManager.AddToPlaylist(radio.RadioName, null, radio.Name, radio.Url);
                    break;
            }
        }
    }
}
