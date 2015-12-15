using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace AlfredPlayer
{
    public class MediaManager
    {
        private int[] inactiveState = { -1, 0, 5 };
        private DateTime dateStartPing;
        private int ping;
        private Timer starttimer;
        private CountdownEvent pingCountDown;
        public APlayer currentPlayer;
        public int status = 0;
        public SyncStatus syncStatus = SyncStatus.UnSynchronized;
        public Dictionary<string, string> arguments;
        public bool isFirst;

        public enum SyncStatus
        {
            UnSynchronized,
            Synchronizing,
            ReadyToPlay,
            Synchronized
        }

        #region Player remote
        public void Stop()
        {
            currentPlayer.Stop();
        }

        public void PlayPause()
        {
            currentPlayer.PlayPause();
        }

        public void DirectPlay()
        {
            var path = arguments["file"];
            var sync = arguments.ContainsKey("sync") ? arguments["sync"] == "1" : false;
            if (sync)
                syncStatus = SyncStatus.Synchronizing;

            if (currentPlayer != null && currentPlayer.status == 3)
                currentPlayer.Stop();

            var playerExists = Players.GetSpecificPlayer(path);
            if (playerExists != null)
                currentPlayer = playerExists;
            else
                return;

            currentPlayer.Play(path);
        }

        public void StartSynchronize()
        {
            double ping = 0;
            for (int i = 0; i < 3;i++)
            {
                ping = (ping + Init.alfredClient.Ping().Result) / 2;
            }

            if (syncStatus == SyncStatus.ReadyToPlay)
            {
                PreciseSleep(1000 - (int)ping);
                currentPlayer.PlayPause();
                syncStatus = SyncStatus.Synchronized;
            }
        }

        protected void PreciseSleep(int milliseconds)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while(stopwatch.ElapsedMilliseconds < milliseconds)
            {
                var timeout = milliseconds - stopwatch.Elapsed.Milliseconds;
                Thread.Sleep(timeout >= 0 ? timeout : 0);
            }
        }

        public void Synchronize()
        {
            var position = double.Parse(arguments["pos"]);
            var status = arguments["status"];

            if (status == "3" || status == "4")
            {
                if (arguments.ContainsKey("path"))
                    currentPlayer.Play(arguments["path"]);

                currentPlayer.Pause();
                currentPlayer.SetPosition(position);

                SendReadyToPlaySignal();
            }
        }

        public void Registered()
        {
            isFirst = bool.Parse(arguments["isFirst"]);
        }

        public void Volume()
        {
            var volume = float.Parse(arguments["volume"]);
            if (currentPlayer != null)
                currentPlayer.SetVolume(volume);
        }

        public void SetPosition()
        {
            currentPlayer.SetPosition(double.Parse(arguments["position"]));
        }
        #endregion

        #region Synchronize
        public void SendNextSongSignal()
        {
            Init.alfredClient.Websocket.MediaManager.Next();
        }

        public void SendReadyToPlaySignal()
        {
            Init.alfredClient.Websocket.Player.ReadyToPlay();
            syncStatus = SyncStatus.ReadyToPlay;
        }

        public void SendUpdateStatusSignal(int status, double length, double position, float? volume = null)
        {
            if (!isFirst)
                return;

            Init.alfredClient.Websocket.MediaManager.UpdateStatus(status, length, position, volume);
        }
        #endregion
    }
}
