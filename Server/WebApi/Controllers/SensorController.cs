using System;
using System.Collections.Generic;
using System.Timers;
using System.Web.Http;
using Alfred.Model.Core;
using Alfred.Plugins;
using Alfred.Utils.Managers;
using Newtonsoft.Json;
using Alfred.Model.Core.Sensor;
using Alfred.Utils;
using Alfred.Server.Attributes;

namespace Alfred.Server.WebApi.Controllers
{
    [TokenAuthorize]
    public class SensorController : ApiController
    {
        private static DateTime _lastMouvement = DateTime.Now;
        private static DateTime _lastDoor = DateTime.Now;
        private static bool _someoneThere;
        private static Timer _mouvementTimer;

        static SensorController()
        {
            _mouvementTimer = new Timer(60000);
            _mouvementTimer.Elapsed += _mouvementTimer_Elapsed;
            _mouvementTimer.Enabled = true;
        }

        static void _mouvementTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _someoneThere = _lastMouvement > _lastDoor;

            Init.WebSocketServer.Broadcast(new AlfredTask
            {
                Command = "Sensor_IsRoomEmpty",
                Arguments = new Dictionary<string, string>
                {
                    { "someoneThere", _someoneThere.ToString() }
                }
            });
        }

        [HttpGet]
        [Route("Sensor/List")]
        public virtual IEnumerable<SensorModel> GetAll()
        {
            return CommonManagers.SensorManager.Devices;
        }

        [HttpGet]
        [Route("Sensor/History/{sensorId}")]
        public virtual IDictionary<DateTime, double> GetHistory(string sensorId)
        {
            var device = CommonManagers.SensorManager.GetDevice(sensorId);
            var idevice = CommonManagers.SensorManager.GetDeviceInterface(device.Manufacturer);
            return idevice.GetDeviceHistory(sensorId);
        }

        [HttpPost]
        [Route("Sensor/NinjaCallback")]
        public virtual void Callback()
        {
            var data = Request.Content.ReadAsStringAsync().Result;
            var dataObj = JsonConvert.DeserializeObject<SubDevice>(data);

            if (dataObj.DA == "010001110101011100110000")
            {
                Launcher.EnqueueTask(new AlfredTask
                {
                    Command = "Scenario_LaunchScenario",
                    Arguments = new Dictionary<string, string>
                    {
                        { "mode", "Romantique" }
                    },
                    Type = TaskType.Server
                });
            }

            if (dataObj.DA == "010101010101010101010101")
            {
                _lastMouvement = DateTime.Now;
                _someoneThere = true;
                Init.WebSocketServer.Broadcast(new AlfredTask
                {
                    Command = "Sensor_IsRoomEmpty",
                    Arguments = new Dictionary<string, string>
                    {
                        { "someoneThere", _someoneThere.ToString() }
                    }
                });
            }

            if (dataObj.DA == "010100010100010101010000")
            {
                _lastDoor = DateTime.Now;
            }

            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            date = date.AddMilliseconds(dataObj.timestamp).ToLocalTime();
            Init.WebSocketServer.Broadcast(new AlfredTask
            {
                Command = "Sensor_Value",
                Arguments = new Dictionary<string, string>
                {
                    { "id", dataObj.GUID },
                    { "value", dataObj.DA },
                    { "date", date.ToString("yyyy-MM-dd'T'HH:mm:ss'.000Z'") }
                }
            });

            if(_lastDoor.Subtract(_lastMouvement).TotalMinutes > 3)
            {
                var alfred = new Alfred.Plugins.Alfred();
                alfred.arguments = new Dictionary<string, string>
                {
                        { "sentence", "Bonjour monsieur" }
                    };
                alfred.PlayTempString();
            }
        }

        class SubDevice
        {
            public int G { get; set; }
            public int V { get; set; }
            public int D { get; set; }
            public string DA { get; set; }
            public long timestamp { get; set; }
            public string node { get; set; }
            public string GUID { get; set; }
            public string id { get; set; }
        }
    }
}
