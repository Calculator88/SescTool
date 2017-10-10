using System;
using System.Collections.Generic;
using System.Linq;
using SescTool.Framework;
using SescTool.Helpers;
using SescTool.Model.ClassroomSchedule;
using SescTool.Services;

namespace SescTool.ViewModels
{
    public class ClassroomDailyScheduleViewModel : ObservableObject
    {

        public ClassroomDailyScheduleViewModel()
        {
            _provider = ServiceLocator.GetService<TimetableProvider>();
            _provider.ClassroomListLoaded += ProviderOnClassroomListLoaded;
            _provider.DailyScheduleForClassroomLoaded += ProviderOnDailyScheduleForClassroomLoaded;
        }

        #region Private fields

        private readonly TimetableProvider _provider;
        private string _currentDay;
        private Dictionary<string, ScheduleDay> _schedules;
        private List<string> _classrooms;
        private List<string> _freeClassrooms;
        private bool _isLoading;
        private bool _isClassroomListLoading;
        private bool _isRefreshing;

        #endregion

        #region Public methods

        public bool ScheduleExistsForDay(string day)
        {
            return Schedules?.ContainsKey(day) ?? false;
        }

        public void ReqestClassroomList()
        {
            if (IsClassroomListLoading) _provider.CancelGettingClassrooms();
            IsClassroomListLoading = true;
            _provider.GetClassrooms();
        }

        public void ReqestClassroomSchedule(string day, bool refresh)
        {
            if (IsLoading) _provider.CancelGeetingDailyClassroomSchedule();
            if (refresh)
            {
                IsRefreshing = true;
                _provider.GetDailyScheduleForClassroom(day);
                return;
            }
            if (day == CurrentDay) return;
            if (ScheduleExistsForDay(day))
            {
                CurrentDay = day;
                return;
            }
            IsLoading = true;
            _provider.GetDailyScheduleForClassroom(day);
        }

        #endregion

        #region Properties

        public Dictionary<string, ScheduleDay> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string CurrentDay
        {
            get => _currentDay;
            set => SetProperty(ref _currentDay, value);
        }

        public bool IsClassroomListLoading
        {
            get => _isClassroomListLoading;
            set => SetProperty(ref _isClassroomListLoading, value);
        }

        public List<string> FreeClassrooms
        {
            get => _freeClassrooms;
            set => SetProperty(ref _freeClassrooms, value);
        }

        public List<string> Classrooms
        {
            get => _classrooms;
            set => SetProperty(ref _classrooms, value);
        }

        #endregion

        #region Event handlers

        private void ProviderOnClassroomListLoaded(object sender, ClassroomsListLoadedEventArgs args)
        {
            IsClassroomListLoading = false;
            if (args.Cancelled) return;
            if (args.Error != null)
            {
                ExceptionOccured?.Invoke(args.Error);
                return;
            }
            Classrooms = args.Classrooms.ToList();
        }

        private void ProviderOnDailyScheduleForClassroomLoaded(object sender, DailyScheduleForClassroomsEventArgs args)
        {
            IsLoading = false;
            IsRefreshing = false;
            if (args.Cancelled) return;
            if (args.Error != null)
            {
                ExceptionOccured?.Invoke(args.Error);
                return;
            }
            var day = args.Schedule.Keys.First();
            AppendSchedule(day, args.Schedule[day]);
            CurrentDay = day;
        }

        #endregion

        #region Private methods

        private void AppendSchedule(string day, ScheduleDay schedule)
        {
            if (Schedules == null)
            {
                Schedules = new Dictionary<string, ScheduleDay> { { day, schedule } };
                return;
            }
            _schedules[day] = schedule;
            OnPropertyChanged(nameof(Schedules));
        }

        #endregion

        #region Events

        public event ExceptionOccuredEventHandler ExceptionOccured;

        #endregion

        #region Public members

        public delegate void ExceptionOccuredEventHandler(Exception exception);

        #endregion

    }
}