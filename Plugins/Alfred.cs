using System;
using System.Collections.Generic;
using System.Globalization;
using System.Speech.Synthesis;
using System.Timers;
using Alfred.Model.Core;
using Alfred.Utils.Plugins;
using log4net;

namespace Alfred.Plugins
{
	public class Alfred : BasePlugin
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Alfred));
		public enum State { Sleep, Awake };
		private readonly SpeechSynthesizer synthesizer;
		public State state { get; set; }
		public Dictionary<string, object> data { get; set; }

		public AlfredTask lastCommand { get; set; }
		public string lastResponse { get; set; }

        public Timer sleepTimer { get; set; }

		public Alfred()
		{
			state = State.Sleep;
            sleepTimer = new Timer(300000);
            sleepTimer.Elapsed += sleepTimer_Elapsed;
			try
			{
				synthesizer = new SpeechSynthesizer();
				data = new Dictionary<string, object>();
				var voix = synthesizer.GetInstalledVoices(new CultureInfo("Fr-fr"));
                if (voix.Count == 0)
                    voix = synthesizer.GetInstalledVoices();

				synthesizer.SelectVoice(voix[0].VoiceInfo.Name);
                synthesizer.Rate = 1;
			}
			catch (Exception exc)
			{
                log.Info("Alfred : " + exc.Message);
			}
		}

        void sleepTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            state = State.Sleep;
        }

        public void RestartTimer()
        {
            sleepTimer.Stop();
            sleepTimer.Start();
        }

		public void Start()
		{
            state = State.Awake;
            result.toSpeakString = DateTime.Now.Hour > 17 ? "Bonsoir " : "Bonjour ";
            result.toSpeakString += "Monsieur";
		}

        public void StopListening()
        {
            state = State.Sleep;
            result.toSpeakString = null;
        }

        public void Stop()
        {
            synthesizer.SpeakAsyncCancelAll();
        }

		public void Speak(string reponse)
		{
			if (reponse != null)
				synthesizer.SpeakAsync(reponse);
		}

        public void PlayTempString()
        {
            Speak(Uri.UnescapeDataString(arguments["sentence"]));
        }
	}
}
