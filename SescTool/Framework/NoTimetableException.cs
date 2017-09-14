using System;

namespace SescTool.Framework
{
    public class NoTimetableException : Exception
    {
        public NoTimetableException() : base("Timetable does not exist")
        {
            
        }
    }
}