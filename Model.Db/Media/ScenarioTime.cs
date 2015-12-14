using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Alfred.Model.Db
{
    public class ScenarioTime
    {
        [Key]
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool Active { get; set; }
        public int Scenario_Id { get; set; }
        public virtual Scenario Scenario { get; set; }
    }
}
