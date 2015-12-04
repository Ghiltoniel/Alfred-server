using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Alfred.Model.Core.Interface;
using Alfred.Model.Core.Light;
using Leap;
using Newtonsoft.Json;

namespace Alfred.Client.Gui.Controllers.Leap
{
    public class LeapController : ControllerAbstract
    {
        private Controller controller;
        private Listener listener;

        public override void Initialize()
        {
            // Create a sample listener and controller
            listener = new LeapListener();
            controller = new Controller();
        }

        public override void StartDevice()
        {
        }

        public override void StopDevice()
        {
            controller.Dispose();
        }

        public override void StartListening()
        {
            controller.AddListener(listener);
        }

        public override void StopListening()
        {
            controller.RemoveListener(listener);
        }
    }

    class LeapListener : Listener
    {
        private readonly Object _thisLock = new Object();
        private int _lastBri = 0;
        private List<LightModel> _lights;
        private DateTime _lastChanged;

        private void SafeWriteLine(String line)
        {
            lock (_thisLock)
            {
                try
                {
                    Init.mainWindow.SetLeapLabelContent(line);
                }
                catch(Exception)
                {

                }
            }
        }

        public override void OnInit(Controller controller)
        {
            SafeWriteLine("Initialized");
            _lastChanged = DateTime.Now;
        }

        public override void OnConnect(Controller controller)
        {
            _lights = Init.alfredClient.Http.Light.GetAll().Result.ToList();
        }

        public override void OnDisconnect(Controller controller)
        {
            SafeWriteLine("Disconnected");
        }

        public override void OnExit(Controller controller)
        {
            SafeWriteLine("Exited");
        }

        public override void OnFrame(Controller controller)
        {
            if (DateTime.Now.Subtract(_lastChanged).TotalSeconds < 1)
                return;

            _lastChanged = DateTime.Now;
            var frame = controller.Frame();

            if (frame.Hands.Count == 2)
            {
                var maxFingers = frame.Hands.Max(h => h.Fingers.Count);
                var rightHand = frame.Hands.First(h => h.Fingers.Count == maxFingers);
                var leftHand = frame.Hands.First(h => h.Id != rightHand.Id);

                if (rightHand != leftHand && leftHand != null && rightHand != null)
                {
                    var numberFingersLeft = leftHand.Fingers.Count;

                    if (numberFingersLeft > 0 && _lights.Count >= numberFingersLeft)
                    {
                        var light = _lights[numberFingersLeft - 1];

                        var ypos = rightHand.PalmPosition.y;
                        var ybright = Math.Round((ypos - 120) * 255 / 450);
                        var intbright = Math.Max(0, Math.Min(Convert.ToInt32(ybright), 255));

                        var xpos = rightHand.PalmPosition.x;
                        var xbright = Math.Round((xpos) * 65535 / 280);
                        var inthue = Math.Max(0, Math.Min(Convert.ToInt32(xbright), 65535));

                        var zpos = rightHand.PalmPosition.z;
                        var zbright = Math.Round((-1 * Math.Abs(zpos) + 280) * 255 / 280);
                        var intsat = Math.Max(0, Math.Min(Convert.ToInt32(zbright), 255));

                        SafeWriteLine(string.Format("Lampe : {0}, bri : {1}, hue : {2}, sat : {3}", light.Name, intbright, inthue, intsat));

                        Init.alfredClient.Websocket.Lights.LightCommand(light.Key, true, (byte)intbright, inthue, intsat);
                    }
                }
            }

            // Get gestures
            var gestures = frame.Gestures();
            for (var i = 0; i < gestures.Count; i++)
            {
                var gesture = gestures[i];

                switch (gesture.Type)
                {
                    case global::Leap.Gesture.GestureType.TYPECIRCLE:
                        var circle = new CircleGesture(gesture);

                        // Calculate clock direction using the angle between circle normal and pointable
                        String clockwiseness;
                        if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                        {
                            //Clockwise if angle is less than 90 degrees
                            clockwiseness = "clockwise";
                        }
                        else
                        {
                            clockwiseness = "counterclockwise";
                        }

                        float sweptAngle = 0;

                        // Calculate angle swept since last frame
                        if (circle.State != global::Leap.Gesture.GestureState.STATESTART)
                        {
                            var previousUpdate = new CircleGesture(controller.Frame(1).Gesture(circle.Id));
                            sweptAngle = (circle.Progress - previousUpdate.Progress) * 360;

                            //SafeWriteLine(sweptAngle.ToString());
                            //DeviceManager.Light(light, 255, Convert.ToInt32(Math.Round(sweptAngle * 65536 / 10)), 255);
                        }

                        SafeWriteLine("Circle id: " + circle.Id
                                       + ", " + circle.State
                                       + ", progress: " + circle.Progress
                                       + ", radius: " + circle.Radius
                                       + ", angle: " + sweptAngle
                                       + ", " + clockwiseness);
                        break;
                    case global::Leap.Gesture.GestureType.TYPESWIPE:
                        var swipe = new SwipeGesture(gesture);
                        break;
                    case global::Leap.Gesture.GestureType.TYPEKEYTAP:
                        var keytap = new KeyTapGesture(gesture);
                        break;
                    case global::Leap.Gesture.GestureType.TYPESCREENTAP:
                        var screentap = new ScreenTapGesture(gesture);
                        var numberFingers = screentap.Pointables.Count;
                        if (_lights.Count >= numberFingers)
                        {
                            var light = _lights[numberFingers - 1];
                            if (light.Bri > 0)
                                Init.alfredClient.Websocket.Lights.LightCommand(light.Key, false);
                            else
                                Init.alfredClient.Websocket.Lights.LightCommand(light.Key, true);

                            SafeWriteLine(numberFingers.ToString());
                        }
                        break;
                    default:
                        SafeWriteLine("Unknown gesture type.");
                        break;
                }
            }
        }
    }
}
