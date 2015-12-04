using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Alfred.Model.Core;
using Alfred.Model.Core.Music;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Alfred.App.Player
{
    public class ControlView : ContentView
    {
        public ControlStatus ControlStatus;
        public Label titleLabel;
        public Label artistLabel;
        Button prev;
        Button next;
        public Button play;
        public ProgressBar position;

        public ControlView(ControlStatus controlStatus)
        {
            ControlStatus = controlStatus;
            ControlStatus.PropertyChanged += controlStatus_PropertyChanged;

            var grid = new Grid
            {
                ColumnSpacing = 0,
                RowSpacing = 0,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = 
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            prev = new Button
            {
                Image = new FileImageSource
                {
                    File = "ic_action_previous_item.png"
                },
                BorderWidth = 0,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.Transparent
            };
            prev.Clicked += prev_Clicked;

            next = new Button
            {
                Image = new FileImageSource
                {
                    File = "ic_action_next_item.png"
                },
                BorderWidth = 0,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.Transparent
            };
            next.Clicked += next_Clicked;

            play = new Button
            {
                Image = new FileImageSource
                {
                    File = "ic_action_play.png"
                },
                BorderWidth = 0,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.Transparent
            };
            play.Clicked += play_Clicked;

            artistLabel = new Label
            {
                Font = Font.SystemFontOfSize(12),
                HorizontalOptions = LayoutOptions.Center
            };

            titleLabel = new Label
            {
                Font = Font.SystemFontOfSize(13),
                HorizontalOptions = LayoutOptions.Center
            };

            position = new ProgressBar();

            grid.Children.Add(titleLabel, 0, 3, 0, 1);
            grid.Children.Add(artistLabel, 0, 3, 1, 2);
            grid.Children.Add(position, 0, 3, 2, 3);
            grid.Children.Add(prev, 0, 3);
            grid.Children.Add(play, 1, 3);
            grid.Children.Add(next, 2, 3);
            try
            {
                Content = grid;
            }catch(Exception e)
            {

            }
        }

        void controlStatus_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Status")
            {
                Device.BeginInvokeOnMainThread(() =>
                    {
                        titleLabel.Text = ControlStatus.CurrentMusicTitle;
                        artistLabel.Text = ControlStatus.CurrentMusicArtist;
                        play.Image = ControlStatus.PlayPauseImage;
                        position.Progress = ControlStatus.Status.Position / ControlStatus.Status.Length;
                        if (ControlStatus.Status.Status == 3)
                            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
                            {
                                position.Progress += 1 / ControlStatus.Status.Length;
                                return ControlStatus.Status.Status == 3;
                            });
                    }
                );
            }
        }

        private void play_Clicked(object sender, EventArgs e)
        {
            App.client.Websocket.MediaManager.PlayPause();
        }

        private void next_Clicked(object sender, EventArgs e)
        {
            App.client.Websocket.MediaManager.Next();
        }

        private void prev_Clicked(object sender, EventArgs e)
        {
            App.client.Websocket.MediaManager.Previous();
        }
    }

    public class ControlStatus : INotifyPropertyChanged
    {
        private PlayerStatus status;
        private SortedDictionary<int, musique> playlist;
        public event PropertyChangedEventHandler PropertyChanged;

        public ControlStatus()
        {
            status = new PlayerStatus(0);
            playlist = new SortedDictionary<int, musique>();
        }

        public FileImageSource PlayPauseImage
        {
            get
            {
                return new FileImageSource
                {
                    File = status.Status == 3 ? "ic_action_pause.png" : "ic_action_play.png"
                };
            }
        }

        public SortedDictionary<int, musique> Playlist
        {
            get
            {
                return playlist;
            }
            set
            {
                playlist = value;
                OnPropertyChanged();
            }
        }

        public PlayerStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }
        
        public string CurrentMusicArtist
        {
            get
            {
                return playlist.ContainsKey(status.CurrentPlaylistIndex) ? 
                    playlist[status.CurrentPlaylistIndex].artist : string.Empty;
            }
        }
        public string CurrentMusicTitle
        {
            get
            {
                return playlist.ContainsKey(status.CurrentPlaylistIndex) ?
                    playlist[status.CurrentPlaylistIndex].title : string.Empty;
            }
        }

        public void UpdateStatus(AlfredTask task)
        {
            var p = JsonConvert.DeserializeObject<SortedDictionary<int, musique>>(task.Arguments["playlist"]);
            if (!p.SequenceEqual(Playlist))
                Playlist = p;
            Status = JsonConvert.DeserializeObject<PlayerStatus>(task.Arguments["status"]);
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class musique
    {
        public int id { get; set; }
        public string path { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string genre { get; set; }
    }
}
