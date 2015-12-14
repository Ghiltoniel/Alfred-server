using mshtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AxWMPLib;
using WMPLib;

namespace AlfredPlayer.Controllers.Players
{
    public class TournedisquePlayer : APlayer
    {
        private AxWindowsMediaPlayer player;
        private System.Timers.Timer nextSongTimer;
        private WebBrowser browser;

        public TournedisquePlayer() : base()
        {
            browser = wrapper.browser;
            player = wrapper.playerWmp;

            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);

            nextSongTimer = new System.Timers.Timer(100);
            nextSongTimer.Elapsed += nextSongTimer_Tick;

            player = wrapper.playerWmp;
            player.PositionChange += player_PositionChange;
            player.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(player_PlayStateChange);
        }

        public void Test()
        {
            Play(null);
        }

        public override void Play(string url)
        {
            browser.Url = new Uri("http://www.letournedisque.com/mix");
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsoluteUri != browser.Url.AbsoluteUri)
                return;

            HtmlElement head = browser.Document.GetElementsByTagName("head")[0];
            HtmlElement sounds = browser.Document.GetElementById("sounds");
            HtmlElementCollection lis = sounds.GetElementsByTagName("li");
            HtmlElementCollection links = lis[0].Children;

            foreach (HtmlElement link in links)
            {
                if (link.GetAttribute("className") == "audio_url")
                {
                    var url = link.InnerText;
                    player.URL = url;
                }
            }
        }

        public override void Pause()
        {
            player.Ctlcontrols.pause();
        }

        public override void PlayPause()
        {
            if (player.playState == WMPPlayState.wmppsPlaying)
            {
                player.Ctlcontrols.pause();
            }
            else
            {
                player.Ctlcontrols.play();
            }
        }

        public override void SetVolume(float volume)
        {
            player.settings.volume = (int)volume;
        }

        public override void Stop()
        {
            if (player.playState == WMPPlayState.wmppsPlaying)
                player.Ctlcontrols.stop();
        }

        public override void SetPosition(double position)
        {
            player.Ctlcontrols.currentPosition = position;
        }

        void player_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            //Init.media.SendUpdatePositionSignal(e.newPosition);
        }

        public void player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPPlayState.wmppsUndefined)
                status = -1;
            else if (e.newState == (int)WMPPlayState.wmppsUndefined)
                status = 0;
            else if (e.newState == (int)WMPPlayState.wmppsTransitioning)
                status = 1;
            else if (e.newState == (int)WMPPlayState.wmppsBuffering)
                status = 2;
            else if (e.newState == (int)WMPPlayState.wmppsPlaying)
            {
                status = 3;
                if (Init.media.syncStatus == MediaManager.SyncStatus.Synchronizing)
                {
                    player.Ctlcontrols.pause();
                    Init.media.SendReadyToPlaySignal();
                }
            }
            else if (e.newState == (int)WMPPlayState.wmppsPaused)
                status = 4;
            else if (e.newState == (int)WMPPlayState.wmppsStopped)
                status = 5;
            else if (e.newState == (int)WMPPlayState.wmppsMediaEnded)
            {
                nextSongTimer.Enabled = true;
                status = 5;
            }
            Init.media.SendUpdateStatusSignal(status, player.currentMedia.duration, player.Ctlcontrols.currentPosition);
        }

        private void nextSongTimer_Tick(object sender, EventArgs e)
        {
            nextSongTimer.Enabled = false;
            Init.media.SendNextSongSignal();
        }
    }
}
