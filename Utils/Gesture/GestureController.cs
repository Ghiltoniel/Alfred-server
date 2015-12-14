using System;
using System.IO;
using System.Windows;
using AlfredInterface.Controllers.Kinect;
using Kinect.Toolbox;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;

namespace AlfredInterface.Controllers.Gesture
{
    public class GestureController : ControllerAbstract
    {
        public readonly ColorStreamManager ColorManager = new ColorStreamManager();
        public readonly ContextTracker ContextTracker = new ContextTracker();
        public bool IsGestureOn;
        public SkeletonDisplayManager SkeletonDisplayManager;
        public Skeleton[] Skeletons;
        public TrackingGesture TrackingGesture;
        public TemplatedGestureDetector CircleGestureRecognizer;
        public string CircleKbPath;
        public Skeleton CurrentSkeleton;
        public DoubleSwipeGestureDetector DoubleSwipeGestureRecognizer;
        public GestureEventHandlers EventHandler;
        public InteractionStream IntStream;

        public GestureController()
        {
            EventHandler = new GestureEventHandlers(this);
        }

        public override void Initialize()
        {
            IsGestureOn = true;
            CircleKbPath = Path.Combine(Environment.CurrentDirectory, @"data\circleKB.save");
            KinectManager.Initialize();
        }

        public override void StartDevice()
        {
            var kinectSensor = KinectManager.kinectSensor;
            if (IsGestureOn)
            {
                kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                kinectSensor.DepthStream.Enable();
                kinectSensor.SkeletonStream.Enable();
                kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                Init.mainWindow.AnimateWindowSize(575, 768, 345, 1280);
            }
        }

        public override void StopDevice()
        {
            KinectManager.Unload();
            Init.mainWindow.AnimateWindowSize(768, 575, 1280, 345);
        }

        public override void StartListening()
        {
            KinectManager.StartSensor();
            KinectManager.audioSource = KinectManager.kinectSensor.AudioSource.Start();
        }

        public override void StopListening()
        {
            KinectManager.StopKinect();
        }

        public override void RegisterEvents()
        {
            try
            {
                if (KinectManager.kinectSensor == null)
                    MessageBox.Show("Error when loading kinect");

                TrackingGesture = new TrackingGesture();
                DoubleSwipeGestureRecognizer = new DoubleSwipeGestureDetector();
                LoadCircleGestureDetector();
                SkeletonDisplayManager = new SkeletonDisplayManager(KinectManager.kinectSensor,
                    Init.mainWindow.KinectCanvas);

                DoubleSwipeGestureRecognizer.OnDoubleGestureDetected += EventHandler.OnDoubleGestureDetected;
                KinectManager.kinectSensor.AllFramesReady += EventHandler.ks_AllFramesReady;

                Init.mainWindow.KinectDisplay.DataContext = ColorManager;

                var mic = new InteractionClient();
                IntStream = new InteractionStream(KinectManager.kinectSensor, mic);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void UnregisterEvents()
        {
            TrackingGesture = null;
            DoubleSwipeGestureRecognizer = null;
            SkeletonDisplayManager = null;

            KinectManager.kinectSensor.AllFramesReady -= EventHandler.ks_AllFramesReady;
            IntStream.InteractionFrameReady -= EventHandler.intStream_InteractionFrameReady;

            Init.mainWindow.KinectDisplay.DataContext = null;
            IntStream.Dispose();
        }

        public void LoadCircleGestureDetector()
        {
            using (Stream recordStream = File.Open(CircleKbPath, FileMode.OpenOrCreate))
            {
                CircleGestureRecognizer = new TemplatedGestureDetector("Circle", recordStream);
                CircleGestureRecognizer.DisplayCanvas = Init.mainWindow.GestureCanvas;
                CircleGestureRecognizer.OnGestureDetected += EventHandler.OnGestureDetected;

                MouseController.Current.ClickGestureDetector = CircleGestureRecognizer;
            }
        }

        public void CloseGestureDetector()
        {
            if (CircleGestureRecognizer == null)
                return;

            using (Stream recordStream = File.Create(CircleKbPath))
            {
                CircleGestureRecognizer.SaveState(recordStream);
            }
            CircleGestureRecognizer.OnGestureDetected -= EventHandler.OnGestureDetected;
        }
    }
}