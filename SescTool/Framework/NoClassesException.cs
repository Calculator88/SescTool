using System;

namespace SescTool.Framework
{
    public class NoClassesException : Exception
    {
        public NoClassesException() : base("Classes do not exist")
        {
            
        }
    }
}