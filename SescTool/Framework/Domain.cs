using System;

namespace SescTool.Framework
{
    public delegate void ClassListLoadedEventHandler(object sender, ClassesListLoadedEventArgs args);

    public delegate void
        WeekScheduleForClassLoadedEventHandler(object sender, WeekScheduleForClassLoadedEventArgs args);
}