using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;
using Alfred.Client.Gui.Ressources;
using Alfred.Model.Core;
using Alfred.Model.Core.Interface;
using Alfred.Utils;
using Alfred.Utils.Media;
using Alfred.Utils.Speech;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;

namespace Alfred.Client.Gui.Controllers.Speech
{
    public class SpeechController : ControllerAbstract
    {
        public SpeechRecognitionEngine ASREngine;
        public bool useKinect = true;
        private Grammar grammar;

        public override void Initialize()
        {
            FillGrammar();
            useKinect = !(Init.mainWindow.MicrophoneInputSelect.SelectionBoxItem.ToString() == "Standard Input");
            if(KinectManager.kinectSensor == null || !KinectManager.kinectSensor.IsRunning)
            {
                KinectManager.isGestureOn = false;
                KinectManager.Initialize();
            }
        }

        public override void StartDevice()
        {
            if (useKinect)
            {
                KinectManager.StartSensor();
                KinectManager.audioSource = KinectManager.kinectSensor.AudioSource.Start();
            }
            ConfigEngine();
        }

        public override void StartListening()
        {
            try
            {
                ASREngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception e)
            {
                Init.mainWindow.SetLeapLabelContent(string.Format("Cannot start recognition engine : error {0}", e.Message));
            }
        }

        public override void StopListening()
        {
            try
            {
                ASREngine.RecognizeAsyncStop();
            }
            catch (Exception e)
            {
                Init.mainWindow.SetLeapLabelContent(string.Format("Cannot start recognition engine : error {0}", e.Message));
            }
        }

        public override void RegisterEvents()
        {
            ASREngine.SpeechRecognized += ASREngine_SpeechRecognized;
            ASREngine.SpeechRecognitionRejected += ASREngine_SpeechRecognitionRejected;
            ASREngine.SpeechHypothesized += ASREngine_SpeechHypothesized;
        }

        public override void UnregisterEvents()
        {
            ASREngine.SpeechRecognized -= ASREngine_SpeechRecognized;
            ASREngine.SpeechRecognitionRejected -= ASREngine_SpeechRecognitionRejected;
            ASREngine.SpeechHypothesized -= ASREngine_SpeechHypothesized;
        }

        public void FillGrammar()
        {
            var xmlGrammar = new SrgsDocument();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            xmlGrammar.Culture = CultureInfo.CurrentCulture;
            var builder = new GrammaireBuilder(xmlGrammar);

            var lights = Init.alfredClient.Http.Light.GetAll().Result;

            builder.CreateRule("piece", lights.Select(d => d.Name).ToList());
            
            // TODO : call http services to get configs.
            //builder.CreateRule("artist", new MusicRepository().GetAllArtists().ToList());
            //builder.CreateRule("genre", new MusicRepository().GetAllGenres().ToList());
            //builder.CreateRule("playlist", new MusicRepository().GetAllPlaylists().ToList());
            //builder.CreateRule("mode", new ScenarioRepository().GetAll().Select(s=>s.Name).ToList());
            //builder.CreateRule("number", Enumerable.Range(0, 100));
            //builder.CreateRule("alfred", new CommandRepository().GetCommands());

            var writer = XmlWriter.Create(Paths.path_grammar);
            xmlGrammar.WriteSrgs(writer);
            writer.Close();

            grammar = new Grammar(xmlGrammar);
        }

        public void ConfigEngine()
        {
            if (useKinect)
            {
                var ri = GetKinectRecognizer();

                if (null != ri && KinectManager.audioSource != null)
                {
                    ASREngine = new SpeechRecognitionEngine(ri.Id);

                    ASREngine.SetInputToAudioStream(
                        KinectManager.audioSource, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                }
            }
            else
            {
                ASREngine = new SpeechRecognitionEngine(Thread.CurrentThread.CurrentCulture);
                ASREngine.SetInputToDefaultAudioDevice();
            }
            ASREngine.LoadGrammar(grammar);
            ASREngine.MaxAlternates = 4;
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            var recog = SpeechRecognitionEngine.InstalledRecognizers();
            foreach (var recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("fr-FR".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase) && value == "True")
                {
                    return recognizer;
                }
            }
            return null;
        }

        #region Speech events
        private void ASREngine_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Init.mainWindow.SetControlContent("RecognizedText", "Hypothèse : " + e.Result.Text);
        }

        private void ASREngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Init.mainWindow.SetControlContent("RecognizedText", "Reconnaissance impossible");
        }

        private void ASREngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var fft = new FFT();
            if (!fft.isSignalOk(e.Result.Audio, e.Result.Audio.StartTime, e.Result.Alternates[0].Confidence))
            {
                Init.mainWindow.SetControlContent("RecognizedText", "Bruit");
                return;
            }

            Init.mainWindow.SetControlContent("RecognizedText", e.Result.Text);
            var baseCommand = e.Result.Semantics["alfred"].Value.ToString();
            Init.mainWindow.SetControlContent("RecognizedCommand", baseCommand);
            
            var arguments = e.Result.Semantics.ToDictionary(s => s.Key, s => s.Value.Value.ToString());
            arguments.Add("recognizedText", e.Result.Text);

            var task = new AlfredTask
            { 
                Command = baseCommand, 
                Type = TaskType.Alfred, 
                Arguments = arguments,
                SpeakAfterExecute = true,
                FromName = "Speech"
            };

            Init.alfredClient.SendCommand(task);
            Thread.Sleep(1000);
            StartListening();   
        }
        #endregion
    }
}
