using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Alfred.Client.Gui.Controllers;
using Alfred.Client.Gui.Controllers.Gesture;
using Alfred.Client.Gui.Controllers.Speech;
using Alfred.Utils;
using Alfred.Utils.Media;
using Microsoft.Speech.Recognition;

namespace Alfred.Client.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ControllerManager ControllerManager;

        public delegateIHM ihmRecognitionDelegate;
        public delegateIHM ihmKinectDelegate;
        public delegateIHM ihmReloadSpeechDelegate;
        private Storyboard myStoryBoard1;
        private Storyboard myStoryBoard2;

        public delegate void delegateIHM(bool? b);
        delegate void SetButtonCallback();

        public MainWindow()
        {
            Loaded += MainWindow_Loaded;
            InitializeComponent();
            SetEvents();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init.mainWindow = this;
            ControllerManager = new ControllerManager();
        }

        #region Init
        private void SetEvents()
        {
            ihmRecognitionDelegate = ToggleSpeech;
            ihmReloadSpeechDelegate = ReloadSpeech;
            SynchronizationButton.Click += synchroniseMusicDB_Click;

            GestureOn.Checked += Controller_Checked;
            SpeechOn.Checked += Controller_Checked;
            LeapOn.Checked += Controller_Checked;
            InkOn.Checked += Controller_Checked;
            MindWaveOn.Checked += Controller_Checked;
            MyoOn.Checked += Controller_Checked;

            GestureOn.Unchecked += Controller_UnChecked;
            SpeechOn.Unchecked += Controller_UnChecked;
            LeapOn.Unchecked += Controller_UnChecked;
            InkOn.Unchecked += Controller_UnChecked;
            MindWaveOn.Unchecked += Controller_UnChecked;
            MyoOn.Unchecked += Controller_UnChecked;
        }

        void Controller_Checked(object sender, RoutedEventArgs e)
        {
            var name = ((CheckBox)(e.OriginalSource)).Name;
            var controllerName = name.Replace("On", "");
            ControllerManager.InitializeController(controllerName);
            ControllerLabel.Content = string.Format("{0} controller has been started !", controllerName);
        }

        void Controller_UnChecked(object sender, RoutedEventArgs e)
        {
            var name = ((CheckBox)(e.OriginalSource)).Name;
            var controllerName = name.Replace("On", "");
            ControllerManager.StopController(controllerName);
            ControllerLabel.Content = string.Format("{0} controller has been stopped !", controllerName);
        }

        private void ReloadProperties()
        {

        }
        #endregion

        #region Events
        private void synchroniseMusicDB_Click(object sender, EventArgs e)
        {
            SynchronizationButton.IsEnabled = false;
            var thread = new Thread(synchronizeThread);
            thread.Start();
        }

        private void synchronizeThread()
        {
            var mm = new MediaSynchronizer();
            mm.FillDBFromFiles();
            EnableSynchronizeButton();
        }

        private void EnableSynchronizeButton()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate {
                SynchronizationButton.IsEnabled = true;
            }));
        }

        public void SetLeapLabelContent(string content)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate {
                LeapLabel.Content = content;
            }));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadProperties();
        }
        #endregion

        #region Speech
        public void ToggleSpeech(bool? toggle = null)
        {
            if (ControllerManager == null)
                return;

            var ASR = ControllerManager.GetController<SpeechController>(typeof(SpeechController));
            var asrOn = ASR != null && ASR.ASREngine != null;
            if ((!toggle.HasValue || toggle.Value) && asrOn && ASR.ASREngine.AudioState == AudioState.Stopped)
            {
                ASR.StartListening();
            }
            else if ((!toggle.HasValue || !toggle.Value) && asrOn && ASR.ASREngine.AudioState != AudioState.Stopped)
            {
                ASR.StopListening();
            }
        }
        public void ReloadSpeech(bool? toggle = null)
        {
            var ASR = ControllerManager.GetController<SpeechController>(typeof(SpeechController));
            ASR = new SpeechController();
            ToggleSpeech(true);
        }
        #endregion

        #region Kinect
        public void ToggleKinect(bool? toggle = null)
        {
            var kinectController = ControllerManager.GetController<GestureController>(typeof(GestureController));
            if ((!toggle.HasValue || toggle.Value) && KinectManager.kinectSensor.IsRunning)
            {
                kinectController.StopDevice();
            }
            else if ((!toggle.HasValue || !toggle.Value) && !KinectManager.kinectSensor.IsRunning)
            {
                kinectController.StartDevice();
                kinectController.StartListening();
            }
        }
        #endregion

        #region UI
        public void SetControlContent(string labelName, string content)
        {
            switch (labelName)
            {
                case "RecognizedCommand":
                    RecognizedCommand.Content = content;
                    break;
                case "RecognizedText":
                    RecognizedText.Content = content;
                    break;
            }
        }

        public void AnimateWindowSize(double fromHeight, double toHeight, double fromWidth, double toWidth)
        {
            var myAnimationHeight = new DoubleAnimation(fromHeight, toHeight, new Duration(TimeSpan.FromMilliseconds(500)));
            var myAnimationWidth = new DoubleAnimation(fromWidth, toWidth, new Duration(TimeSpan.FromMilliseconds(500)));
            myStoryBoard1 = new Storyboard();
            myStoryBoard2 = new Storyboard();
            myStoryBoard1.Children.Add(myAnimationHeight);
            myStoryBoard2.Children.Add(myAnimationWidth);
            Storyboard.SetTargetName(myAnimationHeight, Name);
            Storyboard.SetTargetName(myAnimationWidth, Name);
            Storyboard.SetTargetProperty(myAnimationHeight, new PropertyPath(HeightProperty));
            Storyboard.SetTargetProperty(myAnimationWidth, new PropertyPath(WidthProperty));
            myStoryBoard1.Completed += myStoryBoard1_Completed;
            myStoryBoard1.Begin(this);
        }

        void myStoryBoard1_Completed(object sender, EventArgs e)
        {
            myStoryBoard2.Begin(this);
        }

        #endregion
    }
}
