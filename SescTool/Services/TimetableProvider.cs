using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SescTool.Framework;
using SescTool.Model.ClassSchedule;

namespace SescTool.Services
{
    public class TimetableProvider
    {
        public event ClassListLoadedEventHandler ClassListLoaded;
        public event WeekScheduleForClassLoadedEventHandler WeekScheduleForClassLoaded;
        public event ClassroomsListLoadedEventHandler ClassroomListLoaded;
        public event DailyScheduleForClassroomEventHandler DailyScheduleForClassroomLoaded;
        
        private readonly TimetableDownloader _downloader;
        public TimetableProvider()
        {
            _downloader = new TimetableDownloader();
            _downloader.GetClassesListCompleted += DownloaderOnGetClassesListCompleted;
            _downloader.GetChangesCompleted += DownloaderOnGetChangesCompleted;
            _downloader.GetClassroomsListCompleted += DownloaderOnGetClassroomsListCompleted;
            _downloader.GetDailyScheduleForClassroomsCompleted += DownloaderOnGetDailyScheduleForClassroomsCompleted;
            _downloader.GetTeachersFullNameListCompleted += DownloaderOnGetTeachersFullNameListCompleted;
            _downloader.GetTeachersListCompleted += DownloaderOnGetTeachersListCompleted;
            _downloader.GetWeekScheduleForClassCompleted += DownloaderOnGetWeekScheduleForClassCompleted;
            _downloader.GetWeekScheduleForTeacherCompleted += DownloaderOnGetWeekScheduleForTeacherCompleted;
        }

        private void DownloaderOnGetWeekScheduleForTeacherCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            // TODO
        }

        private void DownloaderOnGetWeekScheduleForClassCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            if (downloadStringCompletedEventArgs.Error != null || downloadStringCompletedEventArgs.Cancelled)
            {
                WeekScheduleForClassLoaded?.Invoke(sender,
                    new WeekScheduleForClassLoadedEventArgs(null, null,
                        downloadStringCompletedEventArgs.Error, downloadStringCompletedEventArgs.Cancelled,
                        downloadStringCompletedEventArgs.UserState));
                return;
            }
            if (String.IsNullOrEmpty(downloadStringCompletedEventArgs.Result) ||
                downloadStringCompletedEventArgs.Result == "Class does not exist" ||
                downloadStringCompletedEventArgs.Result == "Timetable does not exist")
            {
                WeekScheduleForClassLoaded?.Invoke(sender,
                    new WeekScheduleForClassLoadedEventArgs(null, downloadStringCompletedEventArgs.Result,
                        new NoTimetableException(), false, downloadStringCompletedEventArgs.UserState));
                return;
            }
            var result = JsonConvert.DeserializeObject<Dictionary<string, ScheduleWeek>>(downloadStringCompletedEventArgs.Result);
            WeekScheduleForClassLoaded?.Invoke(sender,
                new WeekScheduleForClassLoadedEventArgs(result, downloadStringCompletedEventArgs.Result, null, false,
                    downloadStringCompletedEventArgs.UserState));

        }

        private void DownloaderOnGetTeachersListCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            //TODO
        }

        private void DownloaderOnGetTeachersFullNameListCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            //TODO
            
        }

        private void DownloaderOnGetDailyScheduleForClassroomsCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            if (downloadStringCompletedEventArgs.Error != null || downloadStringCompletedEventArgs.Cancelled)
            {
                DailyScheduleForClassroomLoaded?.Invoke(sender,
                    new DailyScheduleForClassroomsEventArgs(null, null,
                        downloadStringCompletedEventArgs.Error, downloadStringCompletedEventArgs.Cancelled,
                        downloadStringCompletedEventArgs.UserState));
                return;
            }
            if (String.IsNullOrEmpty(downloadStringCompletedEventArgs.Result) ||
                downloadStringCompletedEventArgs.Result == "Class does not exist" ||
                downloadStringCompletedEventArgs.Result == "Timetable does not exist" ||
                downloadStringCompletedEventArgs.Result == "This day has no lessons")
            {
                DailyScheduleForClassroomLoaded?.Invoke(sender,
                    new DailyScheduleForClassroomsEventArgs(null, downloadStringCompletedEventArgs.Result,
                        new NoTimetableException(), false, downloadStringCompletedEventArgs.UserState));
                return;
            }
            var result = JsonConvert.DeserializeObject<Dictionary<string, Model.ClassroomSchedule.ScheduleDay>>(downloadStringCompletedEventArgs.Result);
            DailyScheduleForClassroomLoaded?.Invoke(sender,
                new DailyScheduleForClassroomsEventArgs(result, downloadStringCompletedEventArgs.Result, null, false,
                    downloadStringCompletedEventArgs.UserState));
        }

        private void DownloaderOnGetClassroomsListCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            if (downloadStringCompletedEventArgs.Error != null || downloadStringCompletedEventArgs.Cancelled)
            {
                ClassroomListLoaded?.Invoke(sender,
                    new ClassroomsListLoadedEventArgs(null, null,
                        downloadStringCompletedEventArgs.Error, downloadStringCompletedEventArgs.Cancelled,
                        downloadStringCompletedEventArgs.UserState));
                return;
            }
            if (String.IsNullOrEmpty(downloadStringCompletedEventArgs.Result) ||
                downloadStringCompletedEventArgs.Result == "Bad Request" ||
                downloadStringCompletedEventArgs.Result == "Timetable does not exist" ||
                downloadStringCompletedEventArgs.Result == "This day has no lessons")
            {
                var error = new NoClassesException();
                ClassroomListLoaded?.Invoke(sender,
                    new ClassroomsListLoadedEventArgs(null, downloadStringCompletedEventArgs.Result, error,
                        false, downloadStringCompletedEventArgs.UserState));
                return;
            }
            var res = downloadStringCompletedEventArgs.Result.Split(' ', '\n');
            ClassroomListLoaded?.Invoke(sender,
                new ClassroomsListLoadedEventArgs(res, downloadStringCompletedEventArgs.Result, null, false,
                    downloadStringCompletedEventArgs.UserState));
        }

        private void DownloaderOnGetChangesCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            //TODO

        }

        private void DownloaderOnGetClassesListCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            if (downloadStringCompletedEventArgs.Error != null || downloadStringCompletedEventArgs.Cancelled)
            {
                ClassListLoaded?.Invoke(sender,
                    new ClassesListLoadedEventArgs(null, null,
                        downloadStringCompletedEventArgs.Error, downloadStringCompletedEventArgs.Cancelled,
                        downloadStringCompletedEventArgs.UserState));
                return;
            }
            if (String.IsNullOrEmpty(downloadStringCompletedEventArgs.Result) &&
                downloadStringCompletedEventArgs.Result == "Bad Request" &&
                downloadStringCompletedEventArgs.Result == "Timetable does not exist")
            {
                var error = new NoClassesException();
                ClassListLoaded?.Invoke(sender,
                    new ClassesListLoadedEventArgs(null, downloadStringCompletedEventArgs.Result, error,
                        false, downloadStringCompletedEventArgs.UserState));
                return;
            }
            var res = downloadStringCompletedEventArgs.Result.Split(' ', '\n');
            ClassListLoaded?.Invoke(sender,
                new ClassesListLoadedEventArgs(res, downloadStringCompletedEventArgs.Result, null, false,
                    downloadStringCompletedEventArgs.UserState));
        }

        public Task<string> GetClasses()
        {
           return _downloader.GetClassesList();
        }

        public Task<string> GetClassrooms()
        {
            return _downloader.GetClassroomsList();
        }

        public Task<string> GetDailyScheduleForClassroom(int day)
        {
            return _downloader.GetDailyScheduleForClassrooms(day);
        }

        public Task<string> GetDailyScheduleForClassroom(string day)
        {
            switch (day.ToLower())
            {
                case "понедельник": return GetDailyScheduleForClassroom(1);
                case "вторник": return GetDailyScheduleForClassroom(2);
                case "среда": return GetDailyScheduleForClassroom(3);
                case "четверг": return GetDailyScheduleForClassroom(4);
                case "пятница": return GetDailyScheduleForClassroom(5);
                case "суббота": return GetDailyScheduleForClassroom(6);
                default: return GetDailyScheduleForClassroom(0);
            }
        }

        public Task<string> GetWeekScheduleForClass(string @class)
        {
            return _downloader.GetWeekScheduleForClass(@class);
        }

        public void CancelGettingClasses()
        {
            _downloader.GetClassesListCancel();
        }

        public void CancelGettingWeekScheduleForClass()
        {
            _downloader.GetWeekScheduleForClassCancel();
        }

        public void CancelGettingClassrooms()
        {
            _downloader.GetClassroomsListCancel();
        }

        public void CancelGeetingDailyClassroomSchedule()
        {
            _downloader.GetDailyScheduleForClassroomsCancel();
        }
    }
}