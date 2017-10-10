using System.Collections.Generic;

namespace SescTool.Model.ClassroomSchedule
{
    public class ScheduleLesson
    {
        public int LessonNumber { get; set; }
        public List<Lesson> Classrooms { get; set; }

    }
}