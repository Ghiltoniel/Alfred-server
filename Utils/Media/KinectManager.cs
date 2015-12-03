using System;
using System.IO;
using Microsoft.Kinect;

namespace Alfred.Utils.Media
{
    public static class KinectManager
    {
        public static KinectSensor kinectSensor;
        public static Stream audioSource;
        public static bool isGestureOn = false;
        
        public static void Initialize()
        {
            //listen to any status change for Kinects
            KinectSensor.KinectSensors.StatusChanged += Kinects_StatusChanged;
            try
            {

                //loop through all the Kinects attached to this PC, and start the first that is connected without an error.
                foreach (var kinect in KinectSensor.KinectSensors)
                {
                    if (kinect.Status == KinectStatus.Connected)
                    {
                        kinectSensor = kinect;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static void Unload()
        {
            //listen to any status change for Kinects
            KinectSensor.KinectSensors.StatusChanged -= Kinects_StatusChanged;
            try
            {

                //loop through all the Kinects attached to this PC, and start the first that is connected without an error.
                foreach (var kinect in KinectSensor.KinectSensors)
                {
                    if (kinect.Status == KinectStatus.Connected)
                    {
                        kinect.AudioSource.Stop();
                        if (kinect.ColorStream.IsEnabled)
                            kinect.ColorStream.Disable();
                        if (kinect.DepthStream.IsEnabled)
                            kinect.DepthStream.Disable();
                        if (kinect.SkeletonStream.IsEnabled)
                            kinect.SkeletonStream.Disable();
                        if (kinect.IsRunning)
                            kinect.Stop();
                    }
                }
            }
            catch (Exception)
            {
                //Log.WriteErrorLine("Error when unloading kinect : " + ex.Message);
            }
        }

        public static void StartSensor()
        {
            if (null != kinectSensor)
            {
                try
                {
                    if (!kinectSensor.IsRunning)
                        kinectSensor.Start();
                }
                catch (IOException)
                {
                    kinectSensor = null;
                }
            }
        }

        public static void StopKinect()
        {
            if (null != kinectSensor)
            {
                try
                {
                    if (kinectSensor.IsRunning)
                        kinectSensor.Stop();
                }
                catch (IOException)
                {
                    kinectSensor = null;
                }
            }
        }
        
        static void Kinects_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (kinectSensor == null)
                    {
                        kinectSensor = e.Sensor;
                    }
                    break;
                default:
                    break;
            }
        }


    }
}
