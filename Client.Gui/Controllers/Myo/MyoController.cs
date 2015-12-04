//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using Alfred.Model.Core.Interface;
//using Alfred.Model.Core.Light;
//using MyoSharp;
//using Newtonsoft.Json;
//using MyoSharp.Communication;
//using MyoSharp.Device;
//using MyoSharp.Poses;

//namespace Alfred.Client.Gui.Controllers.Myo
//{
//    public class MyoController : ControllerAbstract
//    {
//        IMyo _myo;
//        private int _lastRoll = 0;

//        public override void Initialize()
//        {
//            using (var channel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create())))
//            using (var hub = Hub.Create(channel))
//            {
//                // listen for when the Myo connects
//                hub.MyoConnected += (sender, e) =>
//                {
//                    e.Myo.Vibrate(VibrationType.Short);
//                    e.Myo.Unlock(UnlockType.Hold);
//                    _myo = e.Myo;

//                    _myo.PoseChanged += Myo_PoseChanged;
//                    _myo.Locked += Myo_Locked;
//                    _myo.Unlocked += Myo_Unlocked;
//                    _myo.OrientationDataAcquired += Myo_OrientationDataAcquired;
//                };

//                // listen for when the Myo disconnects
//                hub.MyoDisconnected += (sender, e) =>
//                {
//                    e.Myo.PoseChanged -= Myo_PoseChanged;
//                    e.Myo.Locked -= Myo_Locked;
//                    e.Myo.Unlocked -= Myo_Unlocked;
//                };

//                // start listening for Myo data
//                channel.StartListening();
//            }
//        }

//        public override void StartDevice()
//        {
//        }

//        public override void StopDevice()
//        {
//        }

//        public override void StartListening()
//        {
//        }

//        public override void StopListening()
//        {
//            if (_myo != null)
//            {
//                _myo.PoseChanged -= Myo_PoseChanged;
//                _myo.Locked -= Myo_Locked;
//                _myo.Unlocked -= Myo_Unlocked;
//                _myo.OrientationDataAcquired -= Myo_OrientationDataAcquired;
//            }
//        }

//        #region Event Handlers
//        private void Myo_PoseChanged(object sender, PoseEventArgs e)
//        {
//            Console.WriteLine("{0} arm Myo detected {1} pose!", e.Myo.Arm, e.Myo.Pose);

//            if (e.Pose == Pose.FingersSpread)
//            {
//                Init.alfredClient.Websocket.Lights.Allume("Salon");
//            }
//            else if (e.Pose == Pose.Fist)
//                Init.alfredClient.Websocket.Lights.Eteins("Salon");
//        }

//        private void Myo_Unlocked(object sender, MyoEventArgs e)
//        {
//            Console.WriteLine("{0} arm Myo has unlocked!", e.Myo.Arm);
//        }

//        private void Myo_Locked(object sender, MyoEventArgs e)
//        {
//            Console.WriteLine("{0} arm Myo has locked!", e.Myo.Arm);
//        }

//        private void Myo_OrientationDataAcquired(object sender, OrientationDataEventArgs e)
//        {
//            const float PI = (float)System.Math.PI;

//            // convert the values to a 0-9 scale (for easier digestion/understanding)
//            var roll = (int)((e.Roll + PI) / (PI * 2.0f) * 10);
//            var pitch = (int)((e.Pitch + PI) / (PI * 2.0f) * 10);
//            var yaw = (int)((e.Yaw + PI) / (PI * 2.0f) * 10);

//            if (roll > _lastRoll)
//            {
//                Init.alfredClient.Websocket.Lights.TurnUp("Salon");
//            }
//            else if (roll < _lastRoll)
//            {
//                Init.alfredClient.Websocket.Lights.TurnDown("Salon");
//            }
//            _lastRoll = roll;

//            Console.Clear();
//            Console.WriteLine(@"Roll: {0}", roll);
//            Console.WriteLine(@"Pitch: {0}", pitch);
//            Console.WriteLine(@"Yaw: {0}", yaw);
//        }

//        #endregion
//    }
//}
