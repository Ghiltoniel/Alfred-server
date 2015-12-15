using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Alfred.Model.Core;
using Alfred.Model.Core.Light;
using Alfred.Plugins.Managers;
using Alfred.Utils;
using Alfred.Model.Db;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using Newtonsoft.Json;
using System.Timers;
using System;
using System.Threading;
using Alfred.Model.Core.Scenario;
using Alfred.Model.Db.Repositories;

namespace Alfred.Plugins
{
    public class Scenario : BasePlugin
    {
        private LightManager LightManager;
        private System.Timers.Timer scenarioTimer;
        private ScenarioRepository _repo;

        public Scenario() : base()
        {
            _repo = new ScenarioRepository();
        }

        public override void Initialize()
        {
            LightManager = CommonManagers.LightManager;

            // Start timer for scenario launcher
            scenarioTimer = new System.Timers.Timer(60000);
            scenarioTimer.Elapsed += scenarioTimer_Elapsed;
            scenarioTimer.Start();
        }

        private void scenarioTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var hour = DateTime.Now.TimeOfDay.Hours;
            var minute = DateTime.Now.TimeOfDay.Minutes;
            var day = (int)(DateTime.Now.DayOfWeek);
            var scenarios = _repo.GetTimeScenarios(hour, minute);

            if (scenarios.Any())
            {
                var scenarioPlugin = new Scenario();
                foreach (var s in scenarios)
                {
                    scenarioPlugin.LaunchScenario(new ScenarioModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        PlaylistId = s.Playlist.Id,
                        PlaylistName = s.Playlist.Name,
                        Genre = s.Genre,
                        Artist = s.Artist,
                        Radio = s.Radio,
                        Lights = JsonConvert.DeserializeObject<List<LightModel>>(s.Lights)
                    });
                }
            }

            if (e.SignalTime.Hour == 3 && e.SignalTime.Minute == 0)
            {
                var thread = new Thread(synchronizeThread);
                thread.Start();
            }
        }

        public static void synchronizeThread()
        {
            var mm = new MediaSynchronizer();
            mm.FillDBFromFiles();
        }

        private ScenarioModel GetScenarioByName(string name)
        {
            return GetScenarios().First(s => s.Name.ToLower() == name.ToLower());
        }

        public IEnumerable<ScenarioModel> GetScenarios()
        {
            return _repo.GetAll();
        }

        public void BroadcastScenarios()
        {
            WebSocketServer.Broadcast(new AlfredTask
            {
                Command = "scenarios",
                Arguments = new Dictionary<string, string>
                {
                    { "scenarios", JsonConvert.SerializeObject(GetScenarios()) }
                }
            });
        }

        public void LaunchScenario()
        {
            var scenario = GetScenarioByName(arguments["mode"]);
            LaunchScenario(scenario);
        }

        public void LaunchScenario(ScenarioModel scenario)
        {
            var devices = CommonManagers.LightManager.Devices;
            foreach (var light in scenario.Lights)
            {
                var device = devices.SingleOrDefault(d => d.Name == light.Name);
                if (device != null)
                {
                    CommonManagers.LightManager.Light(device.Key, light.On, light.Bri, light.Hue, light.Sat);
                    Thread.Sleep(200);
                }
            }

            if (scenario.PlaylistId != null)
            {
                var playlist = new Playlist();
                playlist.arguments = new Dictionary<string, string>();
                playlist.arguments["playlist"] = scenario.PlaylistName;
                playlist.Launch();
            }
            else if (!string.IsNullOrEmpty(scenario.Artist) || !string.IsNullOrEmpty(scenario.Genre))
            {
                var media = CommonPlugins.media;
                media.arguments = new Dictionary<string, string>
                {
                    {"artist", scenario.Artist},
                    {"genre", scenario.Genre},
                    {"count", "15"}
                };
                media.Stop();
                media.DeleteAllFromPlaylist();
                media.AddAllToPlaylist();
            }
            else if (!string.IsNullOrEmpty(scenario.Radio))
            {
                var media = CommonPlugins.media;
                media.arguments = new Dictionary<string, string>
                {
                    {"artist", scenario.Radio},
                    {"title", scenario.Radio},
                    {"album", ""},
                    {"file", scenario.RadioUrl}
                };
                media.Stop();
                media.DirectPlay();
            }
            CommonPlugins.media.Stop();
        }
    }
}
