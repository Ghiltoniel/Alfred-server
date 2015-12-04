using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Alfred.App.Player;
using Alfred.Model.Core;
using Newtonsoft.Json;
using WebSocket4Net;
using Xamarin.Forms;

namespace Alfred.App
{
    class PlayerPage : MasterDetailPage
    {
        ControlView controlView;
        ControlStatus controlStatus;
        PlaylistPage playlistPage;
        ListView listView;
        Dictionary<string, Page> pages;

        public PlayerPage()
        {
            controlStatus = new ControlStatus();
            controlView = new ControlView(controlStatus);

            controlStatus.PropertyChanged += controlStatus_PropertyChanged;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += tapGestureRecognizer_Tapped;

            controlView.artistLabel.GestureRecognizers.Add(tapGestureRecognizer);
            controlView.titleLabel.GestureRecognizers.Add(tapGestureRecognizer);

            playlistPage = new PlaylistPage();

            Title = "NAM Player"; 
            controlView = controlView;

            listView = new ListView
            {
                ItemsSource = new Dictionary<string,string> { { "Tournedisque", "Tournedisque" }, {"Radios", "Radios"}},
                ItemTemplate = new DataTemplate (() => {
         
                    // Create views with bindings for displaying each property.
                    Label titleLabel = new Label ();
                    titleLabel.SetBinding (Label.TextProperty, "Value");
 
                    return new ViewCell {
                        View = new StackLayout {
                            Padding = new Thickness (5, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children = {
                                new StackLayout {
                                    VerticalOptions = LayoutOptions.Center,
                                    Spacing = 0,
                                    Children = {
                                        titleLabel
                                    }
                                }
                            }
                        }
                    };
                })
            };

            var search = new SearchBar
            {
                Placeholder = "Search ..."
            };
            search.SearchButtonPressed += search_SearchButtonPressed;

            Master = new ContentPage
            {
                BackgroundColor = Color.FromHex("#eeeeee"),
                Title = "NAM Player",
                Content = new StackLayout
                {
                    Children = 
                    {
                        search,
                        listView,
                        controlView
                    }
                }
            };

            TapGestureRecognizer tap = new TapGestureRecognizer();
            if (Device.OS != TargetPlatform.iOS)
            {
                tap.Tapped += (sender, args) =>
                    {
                        IsPresented = true;
                    };
            }

            var tournedisque = new TournedisquePage();
            var radio = new RadioPage();
            var searchPage = new SearchPage();

            pages = new Dictionary<string, Page>
            {
                { "Tournedisque", tournedisque },
                { "Radios", radio },
                { "Search", searchPage },
                { "Playlist", playlistPage }
            };

            listView.ItemSelected += (sender, args) =>
            {
                if (args.SelectedItem == null)
                    return;

                // Set the BindingContext of the detail page.
                Detail = pages[((KeyValuePair<string, string>)args.SelectedItem).Value];
                
                // Show the detail page.
                IsPresented = false;
            };

            listView.SelectedItem = new KeyValuePair<string,string>("Tournedisque", "Tournedisque");

        }

        void controlStatus_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Playlist")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    playlistPage.BindingContext = controlStatus;
                });
            }
        }

        async void search_SearchButtonPressed(object sender, EventArgs e)
        {
            var text = (sender as SearchBar).Text;
            pages["Search"].BindingContext = text;
            Detail = pages["Search"];
            IsPresented = false;
        }

        private void tapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            pages["Playlist"].BindingContext = controlStatus;
            Detail = pages["Playlist"];
            listView.SelectedItem = null;
            IsPresented = false;
        }

        public void OnConnect()
        {
            var item = Detail.ToolbarItems.SingleOrDefault(t => t.Icon == "Icon.png");
            if (item == null)
            {
                Detail.ToolbarItems.Add(new ToolbarItem
                {
                    Icon = new FileImageSource { File = "connected.png" }
                });
            }
        }

        public void OnDisconnect()
        {
            var item = Detail.ToolbarItems.SingleOrDefault(t => t.Icon == "Icon.png");
            if (item != null)
                ToolbarItems.Remove(item);
        }

        public void OnReceive(AlfredTask task)
        {
            try
            {
                if (task.Type == TaskType.WebsiteInfo)
                {
                    switch (task.Command)
                    {
                        case "UpdatePlaylist":
                            controlView.ControlStatus.UpdateStatus(task);
                            playlistPage.BindingContext = controlStatus;
                            break;
                        case "UpdateStatus":
                            controlView.ControlStatus.UpdateStatus(task);
                            playlistPage.BindingContext = controlStatus;
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
