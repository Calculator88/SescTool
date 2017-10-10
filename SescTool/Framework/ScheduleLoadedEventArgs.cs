using System;
using System.Collections.Generic;
using System.ComponentModel;
using SescTool.Model.ClassSchedule;
using ScheduleDay = SescTool.Model.ClassroomSchedule.ScheduleDay;

namespace SescTool.Framework
{
    public class ClassesListLoadedEventArgs : AsyncCompletedEventArgs
    {
        public ClassesListLoadedEventArgs(string[] classes, string source, Exception error, bool cancelled,
            object userState) : base(error, cancelled, userState)
        {
            Classes = classes;
            Source = source;
        }
        public string Source { get; }
        public string[] Classes { get; }
    }

    public class WeekScheduleForClassLoadedEventArgs : AsyncCompletedEventArgs
    {
        public WeekScheduleForClassLoadedEventArgs(Dictionary<string, ScheduleWeek> schedule, string source,
            Exception error, bool cancelled, object userState) : base(error, cancelled, userState)
        {
            Schedule = schedule;
            Source = source;
        }
        public Dictionary<string, ScheduleWeek> Schedule { get; }
        public string Source { get; }
    }

    public class ClassroomsListLoadedEventArgs : AsyncCompletedEventArgs
    {
        public ClassroomsListLoadedEventArgs(string[] classrooms, string source, Exception error, bool cancelled,
            object userState) : base(error, cancelled, userState)
        {                                                                              
            Classrooms = classrooms;
            Source = source;
        }
        public string Source { get; }
        public string[] Classrooms { get; }

    }

    public class DailyScheduleForClassroomsEventArgs : AsyncCompletedEventArgs
    {
        public DailyScheduleForClassroomsEventArgs(Dictionary<string, ScheduleDay> schedule, string source,
            Exception error, bool cancelled, object userState) : base(error, cancelled, userState)
        {
            Schedule = schedule;
            Source = source;
        }
        public Dictionary<string, ScheduleDay> Schedule { get; }
        public string Source { get; }

    }
}