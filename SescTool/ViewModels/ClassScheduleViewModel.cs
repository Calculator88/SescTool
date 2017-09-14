using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Android.Util;
using SescTool.Helpers;
using SescTool.Model;
using SescTool.Services.Abstractions;

namespace SescTool.ViewModels
{
    internal class ClassScheduleViewModel : ObservableObject
    {
        internal enum Method
        {
            LoadClass,
            LoadTimetable
        }
        internal delegate void ExceptionOccuredEventHandler(Exception exception, Method method);
        private readonly ITimetableProvider _provider;
        private readonly Regex _regex;
        private Dictionary<string, List<string>> _classDictionary;
        private Dictionary<string, ScheduleWeek> _schedules;
        private string _currentClass;
        private bool _isBusy;

        public event ExceptionOccuredEventHandler OnExceptionOccured;

        public ClassScheduleViewModel(ITimetableProvider provider)
        {
            _provider = provider;
            _regex = new Regex("(\\d+)(\\w+)", RegexOptions.Compiled);
        }

        public bool ScheduleExistsForClass(string @class)
        {
            return _schedules != null && _schedules.ContainsKey(@class);
        }

        public async void LoadClasses()
        {
            IsBusy = true;
            string[] classes;
            try
            {
                classes = await _provider.GetClasses();
            }
            catch (Exception e)
            {
                Log.Error("sesctool", e.Message);
                OnExceptionOccured?.Invoke(e, Method.LoadClass);
                IsBusy = false;
                return;
            }

            var classDict = new Dictionary<string, List<string>>();

            foreach (var @class in classes)
            {
                var match = _regex.Match(@class);
                var number = match.Groups[1].ToString();
                var liter = match.Groups[2].ToString();
                if (!classDict.ContainsKey(number))
                    classDict[number] = new List<string>();
                classDict[number].Add(liter);
            }

            Classes = classDict;
            IsBusy = false;
        }

        public async void LoadSchedule(string @class)
        {
            IsBusy = true;
            Dictionary<string, ScheduleWeek> schedule;
            try
            {
                schedule = await _provider.GetWeekScheduleForClass(@class);
            }
            catch (Exception e)
            {
                Log.Error("sesctool", e.Message);
                OnExceptionOccured?.Invoke(e, Method.LoadTimetable);
                IsBusy = false;
                return;
            }

            AppendSchedule(@class, schedule[@class]);
            CurrentClass = @class;

            IsBusy = false;
        }

        private void AppendSchedule(string @class, ScheduleWeek schedule)
        {
            if (_schedules == null)
                _schedules = new Dictionary<string, ScheduleWeek>();
            _schedules[@class] = schedule;
            OnPropertyChanged(nameof(Schedules));
        }

        public Dictionary<string, List<string>> Classes
        {
            get => _classDictionary;
            set => SetProperty(ref _classDictionary, value);
        }

        public Dictionary<string, ScheduleWeek> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }

        public string CurrentClass
        {
            get => _currentClass;
            set => SetProperty(ref _currentClass, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
    }
}