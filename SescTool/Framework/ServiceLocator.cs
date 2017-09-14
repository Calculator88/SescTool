using System;
using System.Collections.Generic;

namespace SescTool.Framework
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Data = new Dictionary<Type, object>();

        public static void RegisterService<T>(T service)
        {
            Data[typeof(T)] = service;
        }

        public static T GetService<T>()
        {
            return (T)Data[typeof(T)];
        }
    }
}