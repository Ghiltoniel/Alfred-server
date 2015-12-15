using System;
using System.IO;
using System.Timers;
using AxWMPLib;
using WMPLib;

namespace AlfredPlayer.Controllers.Players
{
    class WMPPlayer : APlayer
    {
        private readonly AxWindowsMediaPlayer player;
        private readonly Timer nextSongTimer;

        public WMPPlayer()
        {
            nextSongTimer = new Timer(100);
            nextSongTimer.Elapsed += nextSongTimer_Tick;

            player = wrapper.playerWmp;
            player.PositionChange += player_PositionChange;
            player.PlayStateChange += player_PlayStateChange;
        }

        public override void Play(string file)
        {
            if (File.Exists(file))
            {
                var media = wrapper.playerWmp.newMedia(file);
                player.currentMedia = media;
                player.Ctlcontrols.playItem(media);
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

        void player_PositionChange(object sender, _WMPOCXEvents_PositionChangeEvent e)
        {
            Init.media.SendUpdateStatusSignal(status, player.currentMedia.duration, e.newPosition, player.settings.volume);
        }

        public void player_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
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
                if(Init.media.syncStatus == MediaManager.SyncStatus.Synchronizing)
                {
                    player.Ctlcontrols.pause();
                    SetPosition(0);
                    Init.media.SendReadyToPlaySignal();
                }
                else
                    status = 3;
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
            if (player.currentMedia != null)
                Init.media.SendUpdateStatusSignal(status, player.currentMedia.duration, player.Ctlcontrols.currentPosition, player.settings.volume);
        }

        private void nextSongTimer_Tick(object sender, EventArgs e)
        {
            nextSongTimer.Enabled = false;
            Init.media.SendNextSongSignal();
        }
    }
}
