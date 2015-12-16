using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mshtml;

namespace AlfredPlayer.Controllers.Players
{
    public class YoutubePlayer : APlayer
    {
        private delegate void delegateYoutube();
        private readonly WebBrowser browser;
        private string youtubeCommand;
        public object[] currentArgs;
        private readonly delegateYoutube commandYoutubeDelegate;

        public YoutubePlayer()
        {
            browser = Init.browser;
            browser.ObjectForScripting = new YoutubeScriptManager(this);

            commandYoutubeDelegate = CommandYoutube;
            browser.DocumentCompleted += browser_DocumentCompleted;
        }

        public override void Play(string url)
        {
            browser.Url = new Uri(url.Replace("\\", ""));
        }

        public override void Pause()
        {
            youtubeCommand = "Pause";
            browser.Invoke(commandYoutubeDelegate);
        }

        public override void PlayPause()
        {
            youtubeCommand = "PlayPause";
            browser.Invoke(commandYoutubeDelegate);
        }

        public override void Stop()
        {
            youtubeCommand = "Stop";
            browser.Invoke(commandYoutubeDelegate);
        }

        public override void SetVolume(float volume)
        {
            youtubeCommand = "SetVolume";
            currentArgs = new object[] { Convert.ToInt32(volume) };
            browser.Invoke(commandYoutubeDelegate);
        }

        public override void SetPosition(double pos)
        {
            youtubeCommand = "SetPosition";
            currentArgs = new object[] { Convert.ToInt32(pos * 1000) };
            browser.Invoke(commandYoutubeDelegate);
        }

        public void CommandYoutube()
        {
            var head = browser.Document.GetElementsByTagName("head")[0];
            if (currentArgs != null)
                browser.Document.InvokeScript(youtubeCommand + "Youtube", currentArgs);
            else
                browser.Document.InvokeScript(youtubeCommand + "Youtube");
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsoluteUri != browser.Url.AbsoluteUri)
                return;

            var head = browser.Document.GetElementsByTagName("head")[0];
            var scriptEl = browser.Document.CreateElement("script");
            var element = (IHTMLScriptElement)scriptEl.DomElement;
            element.text = @"
                var lastDate = new Date();
                var lastMs = currentDate.getTime();
                
				function LoadYoutube() {
					window.onbeforeunload = null;
					window.Youtube.setSongStatusCallback(StatusCallback);
				}

				function StatusCallback(song) {
					if(song.status == 'completed')
					{
                        var newDate = new Date();
                        var newMs = newDate.getTime();

                        if ((newMs - lastMs) < 500)
                            return;
                        lastMs = newMs;

                        window.external.NextSong();
						return;
					}
                    window.external.UpdateStatus(song.status, song.song.calculatedDuration, song.song.position);
				}

				function PlayPauseYoutube() {
					window.Youtube.togglePlayPause();
				}

				function PauseYoutube() {
					window.Youtube.pause();
				}

                function SetPositionYoutube(pos) {
					window.Youtube.seekToPosition(pos);
				}

                function SetVolumeYoutube(volume) {
					window.Youtube.setVolume(volume);
				}

                function StopYoutube() {
					var song = window.Youtube.getCurrentSongStatus();
						window.Youtube.pause();
				}

                var tag = document.createElement('script');

                tag.src = ""https://www.youtube.com/iframe_api\"";
                var firstScriptTag = document.getElementsByTagName('script')[0];
                firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

                // 3. This function creates an <iframe> (and YouTube player)
                //    after the API code downloads.
                var player;
                function onYouTubeIframeAPIReady() {
                    player = new YT.Player('player', {
                        height: '390',
                        width: '640',
                        videoId: 'M7lc1UVf-VE',
                        events: {
                            'onReady': onPlayerReady,
                            'onStateChange': onPlayerStateChange
                        }
                    });
                }

                // 4. The API will call this function when the video player is ready.
                function onPlayerReady(event) {
                    event.target.playVideo();
                }

                // 5. The API calls this function when the player's state changes.
                //    The function indicates that when playing a video (state=1),
                //    the player should play for six seconds and then stop.
                var done = false;
                function onPlayerStateChange(event) {
                    if (event.data == YT.PlayerState.PLAYING && !done) {
                        setTimeout(stopVideo, 6000);
                        done = true;
                    }
                }
                
                function StopYoutube()
                {
                    player.stopVideo();
                }
			";
            head.AppendChild(scriptEl);
            browser.Document.InvokeScript("LoadYoutube");
        }
    }

    [ComVisible(true)]
    public class YoutubeScriptManager
    {
        // Variable to store the form of type Form1.
        public APlayer player;

        // Constructor.
        public YoutubeScriptManager(APlayer player)
        {
            // Save the form so it can be referenced later.
            this.player = player;
        }

        // This method can be called from JavaScript.
        public void UpdateStatus(string status, double length, double position)
        {
            if (status == "failed")
                player.status = -1;
            else if (status == "none")
                player.status = 0;
            else if (status == "loading")
                player.status = 1;
            else if (status == "buffering")
                player.status = 2;
            else if (status == "playing")
                player.status = 3;
            else if (status == "paused")
                player.status = 4;
            else if (status == "completed")
                player.status = 5;

            Init.media.SendUpdateStatusSignal(player.status, length / 1000, position / 1000);
        }

        // This method can be called from JavaScript.
        public void NextSong()
        {
            player.Stop();
            Init.media.SendNextSongSignal();
        }

        // This method can be called from JavaScript.
        public void SetStatus(string status)
        {
           
        }
    }
}
