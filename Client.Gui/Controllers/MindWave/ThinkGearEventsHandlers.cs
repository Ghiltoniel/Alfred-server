using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Timers;
using Alfred.Model.Core.Light;
using Newtonsoft.Json;

namespace Alfred.Client.Gui.Controllers.MindWave
{
    class ThinkGearEventsHandlers
    {
        private static DateTime LastDetectedBlink;
        private static byte NumberDetectedBlinks;
        private static readonly Timer DetectedBlinkTimer;
        public static bool RaiseEvents;
        private static LightModel device;

        static ThinkGearEventsHandlers()
        {
            RaiseEvents = true;
            LastDetectedBlink = DateTime.Now;
            NumberDetectedBlinks = 0;
            DetectedBlinkTimer = new Timer(1200);
            DetectedBlinkTimer.Elapsed += DetectedBlinkTimer_Elapsed;
            var lights = Init.alfredClient.Http.Light.GetAll().Result;
            device = lights.SingleOrDefault(d => d.Name == "Salon");
        }

        static void DetectedBlinkTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(NumberDetectedBlinks > 0)
            {
                switch(NumberDetectedBlinks)
                {
                    case 2:
                        Init.alfredClient.Websocket.Lights.EteinsTout();
                        break;
                    case 3:
                        Init.alfredClient.Websocket.Lights.AllumeTout();
                        break;
                }
                NumberDetectedBlinks = 0;
            }
        }

        public void MeditationEventHandler(int level)
        {
            if (device != null)
                Init.alfredClient.Websocket.Lights.LightCommand(device.Key, true, device.Bri, level * 65535 / 100, 255);
        }

        public void AttentionEventHandler(int level)
        {
            if (device != null)
                Init.alfredClient.Websocket.Lights.LightCommand(device.Key, true, (byte)(Math.Max(level - 60, 0) * 255 / 40));
        }

        public void BlinkEventHandler(int level)
        {
            if (level > 100)
            {
                if (DetectedBlinkTimer.Enabled == false)
                    DetectedBlinkTimer.Enabled = true;

                if (LastDetectedBlink.AddSeconds(1) > DateTime.Now)
                    NumberDetectedBlinks++;
                else
                    NumberDetectedBlinks = 1;

                LastDetectedBlink = DateTime.Now;
            }
        }

        public void PoorSignalEventHandler(int level)
        {

        }

        public void EegEventHandler(int delta, int theta, int low_alpha, int high_alpha, int low_beta, int high_beta, int low_gamma, int mid_gamma)
        {

        }
    }
}
