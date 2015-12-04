using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mshtml;

namespace AlfredPlayer.Controllers.Players
{
    public class GroovesharkPlayer : APlayer
    {
        private delegate void delegateGrooveshark();
        private readonly WebBrowser browser;
        private string groovesharkCommand;
        public object[] currentArgs;
        private readonly delegateGrooveshark commandGroovesharkDelegate;

        public GroovesharkPlayer()
        {
            browser = Init.browser;
            browser.ObjectForScripting = new ScriptManager(this);

            commandGroovesharkDelegate = CommandGrooveshark;
            browser.DocumentCompleted += browser_DocumentCompleted;
        }

        public override void Play(string url)
        {
            browser.Url = new Uri(url.Replace("\\", ""));
        }

        public override void Pause()
        {
            groovesharkCommand = "Pause";
            browser.Invoke(commandGroovesharkDelegate);
        }

        public override void PlayPause()
        {
            groovesharkCommand = "PlayPause";
            browser.Invoke(commandGroovesharkDelegate);
        }

        public override void Stop()
        {
            groovesharkCommand = "Stop";
            browser.Invoke(commandGroovesharkDelegate);
        }

        public override void SetVolume(float volume)
        {
            groovesharkCommand = "SetVolume";
            currentArgs = new object[] { Convert.ToInt32(volume) };
            browser.Invoke(commandGroovesharkDelegate);
        }

        public override void SetPosition(double pos)
        {
            groovesharkCommand = "SetPosition";
            currentArgs = new object[] { Convert.ToInt32(pos * 1000) };
            browser.Invoke(commandGroovesharkDelegate);
        }

        public void CommandGrooveshark()
        {
            var head = browser.Document.GetElementsByTagName("head")[0];
            if (currentArgs != null)
                browser.Document.InvokeScript(groovesharkCommand + "Grooveshark", currentArgs);
            else
                browser.Document.InvokeScript(groovesharkCommand + "Grooveshark");
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
                
				function LoadGrooveshark() {
					window.onbeforeunload = null;
					window.Grooveshark.setSongStatusCallback(StatusCallback);
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

				function PlayPauseGrooveshark() {
					window.Grooveshark.togglePlayPause();
				}

				function PauseGrooveshark() {
					window.Grooveshark.pause();
				}

                function SetPositionGrooveshark(pos) {
					window.Grooveshark.seekToPosition(pos);
				}

                function SetVolumeGrooveshark(volume) {
					window.Grooveshark.setVolume(volume);
				}

                function StopGrooveshark() {
					var song = window.Grooveshark.getCurrentSongStatus();
						window.Grooveshark.pause();
				}
			";
            head.AppendChild(scriptEl);
            browser.Document.InvokeScript("LoadGrooveshark");
        }
    }

    [ComVisible(true)]
    public class ScriptManager
    {
        // Variable to store the form of type Form1.
        public APlayer player;

        // Constructor.
        public ScriptManager(APlayer player)
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
