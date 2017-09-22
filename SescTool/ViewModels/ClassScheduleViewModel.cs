using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SescTool.Framework;
using SescTool.Helpers;
using SescTool.Model;
using SescTool.Services;

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
        private readonly TimetableProvider _provider;
        private readonly Regex _regex;
        private Dictionary<string, List<string>> _classDictionary;
        private Dictionary<string, ScheduleWeek> _schedules;
        private string _currentClass;
        private bool _isClassListLoading;
        private bool _isWeekClassScheduleLoading;

        public event ExceptionOccuredEventHandler ExceptionOccured;

        public ClassScheduleViewModel()
        {
            _provider = new TimetableProvider();
            _regex = new Regex("(\\d+)(\\w+)", RegexOptions.Compiled);
            _provider.ClassListLoaded += ProviderOnClassListLoaded;
            _provider.WeekScheduleForClassLoaded += ProviderOnWeekScheduleForClassLoaded;
        }

        private void ProviderOnWeekScheduleForClassLoaded(object sender, WeekScheduleForClassLoadedEventArgs args)
        {
            if (args.Cancelled)
            {
                IsWeekClassSchduleLoading = false;
                return;
            }
            if (args.Error != null)
            {
                ExceptionOccured?.Invoke(args.Error, Method.LoadTimetable);
                IsWeekClassSchduleLoading = false;
                return;
            }
            var @class = args.Schedule.First().Key;
            AppendSchedule(@class, args.Schedule[@class]);
            CurrentClass = @class;
            IsWeekClassSchduleLoading = false;
        }

        private void ProviderOnClassListLoaded(object sender, ClassesListLoadedEventArgs args)
        {
            if (args.Cancelled)
            {
                IsClassListLoading = false;
                return;
            }
            if (args.Error != null)
            {
                ExceptionOccured?.Invoke(args.Error, Method.LoadClass);
                IsClassListLoading = false;
                return;
            }
            var classDict = new Dictionary<string, List<string>>();

            foreach (var @class in args.Classes)
            {
                var match = _regex.Match(@class);
                var number = match.Groups[1].ToString();
                var liter = match.Groups[2].ToString().ToUpper();
                if (!classDict.ContainsKey(number))
                    classDict[number] = new List<string>();
                classDict[number].Add(liter);
            }

            Classes = classDict;
            IsClassListLoading = false;
        }

        public bool ScheduleExistsForClass(string @class)
        {
            return _schedules != null && _schedules.ContainsKey(@class);
        }

        public void RequestClasses()
        {
            IsClassListLoading = true;
            _provider.GetClasses();
        }

        public void RequestSchedule(string @class)
        {
            IsWeekClassSchduleLoading = true;
            _provider.GetWeekScheduleForClass(@class);
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

        public bool IsClassListLoading
        {
            get => _isClassListLoading;
            set => SetProperty(ref _isClassListLoading, value);
        }

        public bool IsWeekClassSchduleLoading
        {
            get => _isWeekClassScheduleLoading;
            set => SetProperty(ref _isWeekClassScheduleLoading, value);
        }

        public void CancelGettingClassList()
        {
            _provider.CancelGettingClasses();
        }

        public void CancelGettingSchedule()
        {
            _provider.CancelGettingWeekScheduleForClass();
        }
    }
}