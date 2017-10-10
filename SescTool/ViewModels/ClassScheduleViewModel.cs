using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SescTool.Framework;
using SescTool.Helpers;
using SescTool.Model.ClassSchedule;
using SescTool.Services;

namespace SescTool.ViewModels
{
    internal class ClassScheduleViewModel : ObservableObject
    {
        public ClassScheduleViewModel()
        {
            _provider = ServiceLocator.GetService<TimetableProvider>();
            _regex = new Regex("(\\d+)(\\w+)", RegexOptions.Compiled);
            _provider.ClassListLoaded += ProviderOnClassListLoaded;
            _provider.WeekScheduleForClassLoaded += ProviderOnWeekScheduleForClassLoaded;
        }
        
        #region Private fields

        private readonly TimetableProvider _provider;
        private readonly Regex _regex;
        private Dictionary<string, List<string>> _classDictionary;
        private Dictionary<string, ScheduleWeek> _schedules;
        private string _currentClass;
        private bool _isClassListLoading;
        private bool _isWeekClassScheduleLoading;
        private bool _isRefreshing;

        #endregion
        
        #region Event handlers

        private void ProviderOnWeekScheduleForClassLoaded(object sender, WeekScheduleForClassLoadedEventArgs args)
        {
            IsWeekClassSchduleLoading = false;
            IsRefreshing = false;
            if (args.Cancelled) return;
            if (args.Error != null)
            {
                ExceptionOccured?.Invoke(args.Error, Method.LoadTimetable);
                return;
            }
            var @class = args.Schedule.First().Key;
            AppendSchedule(@class, args.Schedule[@class]);
            CurrentClass = @class;
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

        #endregion

        #region Public methods

        public bool ScheduleExistsForClass(string @class)
        {
            return _schedules != null && _schedules.ContainsKey(@class);
        }

        public void RequestClasses()
        {
            if (IsClassListLoading) _provider.CancelGettingClasses();
            IsClassListLoading = true;
            _provider.GetClasses();
        }

        public void RequestSchedule(string @class, bool refresh)
        {
            if (IsWeekClassSchduleLoading) _provider.CancelGettingWeekScheduleForClass();
            if (refresh)
            {
                IsRefreshing = true;
                _provider.GetWeekScheduleForClass(@class);
                return;
            }
            if (CurrentClass == @class) return;
            if (ScheduleExistsForClass(@class))
            {
                CurrentClass = @class;
                return;
            }
            IsWeekClassSchduleLoading = true;
            _provider.GetWeekScheduleForClass(@class);
        }

        #endregion

        #region Properties

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
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

        #endregion

        #region Private methods

        private void AppendSchedule(string @class, ScheduleWeek schedule)
        {
            if (_schedules == null)
                _schedules = new Dictionary<string, ScheduleWeek>();
            _schedules[@class] = schedule;
            OnPropertyChanged(nameof(Schedules));
        }

        #endregion

        #region Public members

        public enum Method
        {
            LoadClass,
            LoadTimetable
        }
        public delegate void ExceptionOccuredEventHandler(Exception exception, Method method);

        #endregion

        #region Events

        public event ExceptionOccuredEventHandler ExceptionOccured;

        #endregion

    }
}