using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Alfred.Model.Core;
using Alfred.Utils.Extensions;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using Alfred.Utils.Utils;
using CoreAudioApi;
using log4net;

namespace Alfred.Plugins
{
    public class Common : BasePlugin
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Common));
        public void Time()
        {
            result.toSpeakString = String.Format("Il est {0:00} heure {1:00}", DateTime.Now.Hour, DateTime.Now.Minute);
        }

        public void VolumeDown()
        {
            var devEnum = new MMDeviceEnumerator();
            var device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(Math.Max((device.AudioEndpointVolume.MasterVolumeLevelScalar - 0.1), 0));
        }

        public void VolumeUp()
        {
            var DevEnum = new MMDeviceEnumerator();
            var device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(Math.Min((device.AudioEndpointVolume.MasterVolumeLevelScalar + 0.1), 1));
        }

        public void Volume()
        {
            var DevEnum = new MMDeviceEnumerator();
            var device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            var task = new AlfredTask
            {
                Command = "Volume",
                Type = TaskType.ActionPlayer,
                Arguments = new Dictionary<string, string> { { "volume", arguments["volume"] } }
            };
            var channel = int.Parse(arguments["channel"]);
            PlayerManager.BroadcastPlayersChannel(task, channel);
        }

        public static float GetVolume()
        {
            var devEnum = new MMDeviceEnumerator();
            var device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            return device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
        }

        public void Dialog()
        {
            if (arguments.ContainsKey("recognizedText"))
            {
                var direct = Path.GetDirectoryName(Application.ExecutablePath);
                var alfredDialogs = FileUtils.ReadFileToArray(direct + "/Data/Common/responses.txt");
                if (alfredDialogs.ContainsKey(arguments["recognizedText"]))
                {
                    var responses = alfredDialogs[arguments["recognizedText"]];
                    responses.Shuffle();
                    result.toSpeakString = responses.First();
                }
            }
        }
    }
}
