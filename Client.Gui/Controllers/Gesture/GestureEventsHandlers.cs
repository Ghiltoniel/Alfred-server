using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using Alfred.Model.Core.Light;
using Alfred.Utils.Media;
using Kinect.Toolbox;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using Newtonsoft.Json;

namespace Alfred.Client.Gui.Controllers.Gesture
{
    public class GestureEventHandlers
    {
        private readonly GestureController _controller;
        private static double _humanInactivitySeconds;
        private DateTime _lastSkeletonDate = DateTime.Now;
        private LightModel _salon;

        public GestureEventHandlers(GestureController controller)
        {
            GetDevices();
            _controller = controller;
        }

        public async void GetDevices()
        {
            var lights = await Init.alfredClient.Http.Light.GetAll();
            _salon = lights.SingleOrDefault(s => s.Name == "Salon");
        }

        public void OnGestureDetected(string gesture)
        {
            if (_controller.CurrentSkeleton != null)
            {
                var hand = _controller.CurrentSkeleton.Joints[JointType.HandRight].Position;
                var head = _controller.CurrentSkeleton.Joints[JointType.Head].Position;
                Init.mainWindow.GestureLabel.Content = string.Format("{0}", gesture);

                _controller.TrackingGesture.Action(head, hand, gesture);
            }
        }

        public void OnActivityDetected(bool active)
        {
            if (_salon == null)
                return;

            if (active)
            {
                _humanInactivitySeconds = 0;
                _lastSkeletonDate = DateTime.Now;
                if (_salon.Bri == 0 && (_lastSkeletonDate.Hour > 19 || _lastSkeletonDate.Hour < 9))
                    Init.alfredClient.Websocket.Lights.LightCommand(_salon.Key, true);
            }
            else if (!active && _salon.On)
                Init.alfredClient.Websocket.Lights.LightCommand(_salon.Key, false);
        }

        public void OnDoubleGestureDetected(string gesture)
        {
            Init.mainWindow.GestureLabel.Content = string.Format("{0}", gesture);
            _controller.TrackingGesture.DoubleAction(gesture);
        }

        public void ks_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (var dif = e.OpenColorImageFrame())
            {
                if (dif == null)
                    return;
                _controller.ColorManager.Update(dif);
            }

            using (var dif = e.OpenDepthImageFrame())
            {
                if (dif == null)
                    return;

                _controller.IntStream.ProcessDepth(dif.GetRawPixelData(), dif.Timestamp);
            }

            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                _humanInactivitySeconds = DateTime.Now.Subtract(_lastSkeletonDate).TotalSeconds;
                if (_humanInactivitySeconds > 15)
                    OnActivityDetected(false);

                if (skeletonFrame != null)
                {
                    _controller.Skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(_controller.Skeletons);

                    if (_controller.Skeletons.All(s => s.TrackingState == SkeletonTrackingState.NotTracked))
                        return;

                    ProcessFrame(_controller.Skeletons);

                    _controller.IntStream.ProcessSkeleton(_controller.Skeletons, KinectManager.kinectSensor.AccelerometerGetCurrentReading(), skeletonFrame.Timestamp);
                }
            }
        }

        void ProcessFrame(Skeleton[] skeletons)
        {
            foreach (var skeleton in skeletons)
            {
                OnActivityDetected(true);

                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    continue;

                _controller.ContextTracker.Add(skeleton.Position.ToVector3(), skeleton.TrackingId);
                if (!_controller.ContextTracker.IsStableRelativeToCurrentSpeed(skeleton.TrackingId))
                    continue;

                _controller.CurrentSkeleton = skeleton;

                var leftHandPos = new SkeletonPoint();
                var rightHandPos = new SkeletonPoint();

                var rightJoint = skeleton.Joints[JointType.HandRight];
                var leftJoint = skeleton.Joints[JointType.HandLeft];

                Init.mainWindow.xLabel.Content = rightJoint.Position.X;
                Init.mainWindow.yLabel.Content = rightJoint.Position.Y;
                Init.mainWindow.zLabel.Content = rightJoint.Position.Z;

                if (rightJoint.TrackingState == JointTrackingState.Tracked && leftJoint.TrackingState == JointTrackingState.Tracked)
                {
                    rightHandPos = rightJoint.Position;
                    leftHandPos = leftJoint.Position;
                    if (rightHandPos.X != 0 && leftHandPos.X != 0)
                        _controller.DoubleSwipeGestureRecognizer.Add(leftHandPos, rightHandPos, KinectManager.kinectSensor);
                }
            }

            _controller.SkeletonDisplayManager.Draw(skeletons, false);
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (KinectManager.kinectSensor != null)
                KinectManager.kinectSensor.Stop();

            KinectManager.kinectSensor = null;
        }

        public void intStream_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            var iFrame = e.OpenInteractionFrame();
            if (iFrame == null) return;

            var usrInfo = new UserInfo[6];

            iFrame.CopyInteractionDataTo(usrInfo);

            var curUsers = usrInfo.Where(x => x.SkeletonTrackingId > 0).ToList();

            if (curUsers.Count > 0)
            {
                var curUser = curUsers[0];

                var rightHand = curUser.HandPointers.SingleOrDefault(p => p.HandType == InteractionHandType.Right);
                var leftHand = curUser.HandPointers.SingleOrDefault(p => p.HandType == InteractionHandType.Left);

                if (rightHand == null || leftHand == null || Math.Abs(leftHand.Y - rightHand.Y) < 0.50)
                    return;

                if (rightHand.HandEventType == InteractionHandEventType.Grip)
                    OnGestureDetected("SwipeToBottom");
                else if (rightHand.HandEventType == InteractionHandEventType.GripRelease)
                    OnGestureDetected("SwipeToUp");
            }
        }
    }
}