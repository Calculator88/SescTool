using System.Collections.Generic;

namespace SescTool.Model
{
    public class ScheduleDay
    {
        public string Day { get; set; }
        public List<Lesson> Lessons { get; set; }
        public bool ExistsChanges { get; set; }
    }
}