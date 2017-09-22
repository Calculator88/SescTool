using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SescTool.Framework;
using SescTool.Model;

namespace SescTool.Services
{
    public class TimetableProvider
    {
        public event ClassListLoadedEventHandler ClassListLoaded;
        public event WeekScheduleForClassLoadedEventHandler WeekScheduleForClassLoaded;
        
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
              //TODO
        }

        private void DownloaderOnGetClassroomsListCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            //TODO
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

        public async Task<string[]> GetTeachers()
        {
            var respString = await _downloader.GetClassesList();
            return respString.Split('\n');
        }

        public async Task<string[]> GetTeachersFullName()
        {
            var respString = await _downloader.GetClassesList();
            return respString.Split('\n');
        }

        public async Task<string[]> GetClassrooms()
        {
            var respString = await _downloader.GetClassesList();
            return respString.Split('\n');
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
    }
}