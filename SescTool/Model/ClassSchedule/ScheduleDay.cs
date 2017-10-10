using System.Collections.Generic;

namespace SescTool.Model.ClassSchedule
{
    public class ScheduleDay
    {
        public string Day { get; set; }
        public List<Lesson> Lessons { get; set; }
        public bool ExistsChanges { get; set; }
    }
}