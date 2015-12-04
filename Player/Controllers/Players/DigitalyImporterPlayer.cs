using System;
using System.Windows.Forms;
using mshtml;

namespace AlfredPlayer.Controllers.Players
{
    public class DigitalyImportedPlayer : APlayer
    {
        private delegate void delegateDigitalyImported();
        private readonly WebBrowser browser;
        private string DigitalyImportedCommand;
        public object[] currentArgs;
        private readonly delegateDigitalyImported commandDigitalyImportedDelegate;

        public DigitalyImportedPlayer()
        {
            browser = Init.browser;
            browser.ObjectForScripting = new ScriptManager(this);

            commandDigitalyImportedDelegate = CommandDigitalyImported;
            //browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        }

        public override void Play(string url)
        {
            browser.Url = new Uri(url.Replace("\\", ""));
        }

        public override void Pause()
        {
            DigitalyImportedCommand = "Pause";
            browser.Invoke(commandDigitalyImportedDelegate);
        }

        public override void PlayPause()
        {
            DigitalyImportedCommand = "PlayPause";
            browser.Invoke(commandDigitalyImportedDelegate);
        }

        public override void Stop()
        {
            DigitalyImportedCommand = "Stop";
            browser.Invoke(commandDigitalyImportedDelegate);
        }

        public override void SetVolume(float volume)
        {
            DigitalyImportedCommand = "SetVolume";
            currentArgs = new object[] { Convert.ToInt32(volume) };
            browser.Invoke(commandDigitalyImportedDelegate);
        }

        public override void SetPosition(double pos)
        {
            DigitalyImportedCommand = "SetPosition";
            currentArgs = new object[] { Convert.ToInt32(pos * 1000) };
            browser.Invoke(commandDigitalyImportedDelegate);
        }

        public void CommandDigitalyImported()
        {
            var head = browser.Document.GetElementsByTagName("head")[0];
            if (currentArgs != null)
                browser.Document.InvokeScript(DigitalyImportedCommand + "DigitalyImported", currentArgs);
            else
                browser.Document.InvokeScript(DigitalyImportedCommand + "DigitalyImported");
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
                
				function LoadDigitalyImported() {
					window.onbeforeunload = null;
					window.DigitalyImported.setSongStatusCallback(StatusCallback);
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

				function PlayPauseDigitalyImported() {
					window.DigitalyImported.togglePlayPause();
				}

				function PauseDigitalyImported() {
					window.DigitalyImported.pause();
				}

                function SetPositionDigitalyImported(pos) {
					window.DigitalyImported.seekToPosition(pos);
				}

                function SetVolumeDigitalyImported(volume) {
					window.DigitalyImported.setVolume(volume);
				}

                function StopDigitalyImported() {
					var song = window.DigitalyImported.getCurrentSongStatus();
						window.DigitalyImported.pause();
				}
			";
            head.AppendChild(scriptEl);
            browser.Document.InvokeScript("LoadDigitalyImported");
        }
    }
}
