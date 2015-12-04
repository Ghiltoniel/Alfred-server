using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Kinect;

namespace Alfred.Client.Gui
{
    public class TrackingGesture
    {
        public Dictionary<string, SkeletonPoint> allLights;
        public DateTime lastActionTime;

        public TrackingGesture()
        {
            allLights = new Dictionary<string, SkeletonPoint>();
            FillAllLigthPoints();
        }

        private void FillAllLigthPoints()
        {
            var pointsOrig = new Dictionary<string, SkeletonPoint>();
            pointsOrig.Add("Led", new SkeletonPoint { X = (float)1.5, Y = 1, Z = (float)3.75 });
            pointsOrig.Add("Salon", new SkeletonPoint { X = (float)0.42, Y = (float)0.75, Z = (float)1.84 });
            pointsOrig.Add("Feu rouge", new SkeletonPoint { X = (float)-0.5, Y = 0, Z = 0 });
            pointsOrig.Add("Entrée", new SkeletonPoint { X = -2, Y = 2, Z = (float)0.1 });
            pointsOrig.Add("Cuisine", new SkeletonPoint { X = (float)-1.90, Y = 2, Z = (float)3.2 });
            pointsOrig.Add("Tele", new SkeletonPoint { X = 0, Y = 0, Z = 0 });

            //float angle = 27;
            //double angleRadian = angle * Math.PI / 180;

            //foreach (var item in pointsOrig)
            //{
            //    var skeletonPointNew = new SkeletonPoint();
            //    skeletonPointNew.X = item.Value.X;
            //    skeletonPointNew.Z = (float)(item.Value.Z * Math.Cos(angleRadian) + item.Value.Y * Math.Sin(angleRadian));
            //    skeletonPointNew.Y = (float)(-item.Value.Z * Math.Sin(angleRadian) + item.Value.Y * Math.Cos(angleRadian));
            //    allLights.Add(item.Key, skeletonPointNew);
            //}

            allLights = pointsOrig;
        }

        public double GetHorAngleFromPoints(SkeletonPoint point1, SkeletonPoint point2)
        {
            return Math.Atan2(point2.X - point1.X, point1.Z - point2.Z);
        }

        public double GetVerAngleFromPoints(SkeletonPoint point1, SkeletonPoint point2)
        {
            return Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        }

        public string GetClosestDeviceFromAngle(SkeletonPoint pointStart, SkeletonPoint pointTarget, Dictionary<string, SkeletonPoint> points)
        {
            // The angle between the shoulder and the hand
            var targetHorAngle = GetHorAngleFromPoints(pointStart, pointTarget);
            var targetVerAngle = GetVerAngleFromPoints(pointStart, pointTarget);
            var deltaHorAngle = double.MaxValue;
            var deltaVerAngle = double.MaxValue;
            double tempHorAngle = 0;
            double tempVerAngle = 0;
            double tempHorDelta = 0;
            double tempVerDelta = 0;

            string result = null;

            foreach (var light in points.Keys)
            {
                tempHorAngle = GetHorAngleFromPoints(pointStart, points[light]);
                tempVerAngle = GetVerAngleFromPoints(pointStart, points[light]);

                if ((tempHorDelta = Math.Abs(targetHorAngle - tempHorAngle)) < deltaHorAngle
                    && (tempVerDelta = Math.Abs(targetVerAngle - tempVerAngle)) < deltaVerAngle)
                {
                    deltaHorAngle = tempHorDelta;
                    deltaVerAngle = tempVerDelta;
                    result = light;
                }
            }
            if (deltaVerAngle < 0.6 && deltaHorAngle < 0.6)
                return result;
            return null;
        }

        public void Action(SkeletonPoint head, SkeletonPoint hand, string gesture)
        {
            if (lastActionTime.AddSeconds(1) > DateTime.Now)
                return;

            if (gesture == "SwipeToLeft")
            {
            }

            if (gesture == "SwipeToRight")
            {
            }

            var deviceName = GetClosestDeviceFromAngle(head, hand, allLights);

            switch (deviceName)
            {
                case "Tele":
                    if (gesture == "SwipeToUp")
                    {
                    }
                    else if (gesture == "SwipeToBottom")
                    {
                    }
                    break;

                default:
                    if (deviceName != null)
                    {
                        if (gesture == "SwipeToUp")
                            Init.alfredClient.Websocket.Lights.Allume(deviceName);
                        else if (gesture == "SwipeToBottom")
                            Init.alfredClient.Websocket.Lights.Eteins(deviceName);
                        else if (gesture == "Circle")
                            Init.alfredClient.Websocket.Lights.TurnDown(deviceName);
                    }
                    break;
            }
            lastActionTime = DateTime.Now;
        }

        public void DoubleAction(string gesture)
        {
            if (lastActionTime.AddSeconds(1) > DateTime.Now)
                return;
            
            if (gesture == "SwipeToUp")
                Init.alfredClient.Websocket.Lights.AllumeTout();
            else if (gesture == "SwipeToBottom")
                Init.alfredClient.Websocket.Lights.EteinsTout();
            else if (gesture == "SwipeToExt")
                SendKeys.SendWait("{+}");
            else if (gesture == "SwipeToCenter")
                SendKeys.SendWait("{-}");
        }
    }
}