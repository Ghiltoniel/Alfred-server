using Alfred.Model.Core.Light;
using System;
using System.Collections.Generic;

namespace Alfred.Model.Core.Scenario
{
    public class ScenarioModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PlaylistId { get; set; }
        public string PlaylistName { get; set; }
        public string Genre { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public IEnumerable<LightModel> Lights { get; set; }
        public string Radio { get; set; }
        public string RadioUrl { get; set; }
        public virtual ICollection<ScenarioTime> ScenarioTimes { get; set; }
    }

    public class ScenarioTime
    {
        public int id { get; set; }
        public int IdScenario { get; set; }
        public sbyte Active { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string ExecutionDays { get; set; }
    }
}
