using System;

namespace SescTool.Framework
{
    public class ClassNotExistsException : Exception
    {
        public ClassNotExistsException() : base("Class does not exist")
        {
            
        }
    }
}