using System.Collections.Generic;

namespace SescTool.Model.ClassSchedule
{
    public class Lesson
    {
        public int Number { get; set; }
        public List<LessonByGroups> LessonsByGroups { get; set; }
    }
}