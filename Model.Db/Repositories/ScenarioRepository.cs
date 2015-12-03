using Alfred.Model.Core.Light;
using Alfred.Model.Core.Scenario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Db.Repositories
{
    public class ScenarioRepository
    {
        public IEnumerable<Scenario> GetTimeScenarios(int hours, int minutes)
        {
            using(var db = new AlfredContext())
            {
                var day = DateTime.Now.DayOfWeek;
                return db.ScenarioTimes
                   .Where(st => st.Hours == hours
                       && st.Minutes == minutes
                       && st.Day == day
                       && st.Active)
                   .Select(st => st.Scenario)
                   .ToList();
            }
        }

        public IEnumerable<ScenarioModel> GetAll()
        {
            using (var db = new AlfredContext())
            {
                return db.Scenarios.Include(p => p.Playlist).ToList().Select(s =>
                    new ScenarioModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        PlaylistId = s.Playlist != null ? (int?)s.Playlist.Id : null,
                        PlaylistName = s.Playlist != null ? s.Playlist.Name : null,
                        Genre = s.Genre,
                        Artist = s.Artist,
                        Radio = s.Radio,
                        Lights = JsonConvert.DeserializeObject<List<LightModel>>(s.Lights)
                    });
            }
        }

        public ScenarioModel Get(int id)
        {
            using (var db = new AlfredContext())
            {
                var scenarioDb = db.Scenarios.Include(p => p.Playlist).Single(s => s.Id == id);
                if (scenarioDb == null)
                    return null;

                return new ScenarioModel
                {
                    Id = scenarioDb.Id,
                    Name = scenarioDb.Name,
                    PlaylistId = scenarioDb.Playlist != null ? (int?)scenarioDb.Playlist.Id : null,
                    PlaylistName = scenarioDb.Playlist != null ? scenarioDb.Playlist.Name : null,
                    Genre = scenarioDb.Genre,
                    Artist = scenarioDb.Artist,
                    Radio = scenarioDb.Radio,
                    Lights = JsonConvert.DeserializeObject<List<LightModel>>(scenarioDb.Lights)
                };
            }
        }

        public ScenarioModel GetByName(string name)
        {
            using (var db = new AlfredContext())
            {
                var scenarioDb = db.Scenarios.Include(p => p.Playlist).SingleOrDefault(s => s.Name.ToLower() == name.ToLower());
                if (scenarioDb == null)
                    return null;

                return new ScenarioModel
                {
                    Id = scenarioDb.Id,
                    Name = scenarioDb.Name,
                    PlaylistId = scenarioDb.Playlist != null ? (int?)scenarioDb.Playlist.Id : null,
                    PlaylistName = scenarioDb.Playlist != null ? scenarioDb.Playlist.Name : null,
                    Genre = scenarioDb.Genre,
                    Artist = scenarioDb.Artist,
                    Radio = scenarioDb.Radio,
                    Lights = JsonConvert.DeserializeObject<List<LightModel>>(scenarioDb.Lights)
                };
            }
        }

        public void Save(ScenarioModel scenario)
        {
            using (var db = new AlfredContext())
            {
                var existingScenario = db.Scenarios.SingleOrDefault(s => s.Id == scenario.Id);
                if (existingScenario == null)
                {
                    existingScenario = new Scenario();
                    db.Scenarios.Add(existingScenario);
                }

                existingScenario.Name = scenario.Name;
                existingScenario.Playlist_Id = scenario.PlaylistId;
                existingScenario.Genre = scenario.Genre;
                existingScenario.Artist = scenario.Artist;
                existingScenario.RadioUrl = scenario.RadioUrl;
                existingScenario.Radio = scenario.Radio;
                existingScenario.Lights = JsonConvert.SerializeObject(scenario.Lights);
                db.SaveChanges();
            }
        }
    }
}
