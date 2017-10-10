using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SescTool.Services.Abstractions;

namespace SescTool.Services
{
    public class TimetableDownloader : ITimetableDownloader
    {
        private readonly Lazy<WebClient> _classListClient;
        private readonly Lazy<WebClient> _teacherListClient;
        private readonly Lazy<WebClient> _teacherFullNameClient;
        private readonly Lazy<WebClient> _classroomListClient;
        private readonly Lazy<WebClient> _weekClassScheduleClient;
        private readonly Lazy<WebClient> _weekTeacherScheduleClient;
        private readonly Lazy<WebClient> _dailyClassroomScheduleClient;
        private readonly Lazy<WebClient> _changesClient;

        public string ApiUrl => "http://lyceum.urfu.ru/study/mobile.php?f={0}&{1}={2}";
        public string ChangesUrl => "http://lyceum.urfu.ru/study/izmenHtml.php";
        public string TeachersListFunction => "5";
        public string ClassListFunction => "4";
        public string TeachersFullNameListFunction => "7";
        public string ClassroomsListFunction => "6";
        public string WeekScheduleForClass => "1";
        public string WeekScheduleForTeacher => "2";
        public string DailyScheduleForClassrooms => "3";
        public string ClassArgument => "k";
        public string DayArgument => "d";
        public string TeacherArgumnet => "p";

        public event DownloadStringCompletedEventHandler GetClassesListCompleted;
        public event DownloadStringCompletedEventHandler GetTeachersListCompleted;
        public event DownloadStringCompletedEventHandler GetTeachersFullNameListCompleted;
        public event DownloadStringCompletedEventHandler GetClassroomsListCompleted;
        public event DownloadStringCompletedEventHandler GetWeekScheduleForClassCompleted;
        public event DownloadStringCompletedEventHandler GetChangesCompleted;
        public event DownloadStringCompletedEventHandler GetWeekScheduleForTeacherCompleted;
        public event DownloadStringCompletedEventHandler GetDailyScheduleForClassroomsCompleted;

        private static WebClient NewClientInstance()
        {
            return new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
        }

        public TimetableDownloader()
        {
            _classListClient = new Lazy<WebClient>(NewClientInstance);
            _teacherListClient = new Lazy<WebClient>(NewClientInstance);
            _teacherFullNameClient = new Lazy<WebClient>(NewClientInstance);
            _classroomListClient = new Lazy<WebClient>(NewClientInstance);
            _weekClassScheduleClient = new Lazy<WebClient>(NewClientInstance);
            _weekTeacherScheduleClient = new Lazy<WebClient>(NewClientInstance);
            _dailyClassroomScheduleClient = new Lazy<WebClient>(NewClientInstance);
            _changesClient = new Lazy<WebClient>(NewClientInstance);
        }

        private string MakeRequestUrl(string function, string paramName, string paramValue)
        {
            var encod = Encoding.GetEncoding("windows-1251").GetBytes(paramValue);
            var hex = BitConverter.ToString(encod);
            var result = "%" + hex.Replace("-", "%");
            return String.Format(ApiUrl, function, paramName, result);
        }

        private string MakeRequestUrl(string function)
        {
            return String.Format(ApiUrl, function, "l", "");
        }

        public Task<string> GetClassesList()
        {
            var requestUrl = MakeRequestUrl(ClassListFunction);
            if (!_classListClient.IsValueCreated)
                _classListClient.Value.DownloadStringCompleted += ClassListOnDownloadCompleted;
            var response = _classListClient.Value.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        private void ClassListOnDownloadCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            GetClassesListCompleted?.Invoke(sender, downloadStringCompletedEventArgs);
        }

        public Task<string> GetTeachersList()
        {
            var requestUrl = MakeRequestUrl(TeachersListFunction);
            if (!_teacherListClient.IsValueCreated) _teacherListClient.Value.DownloadStringCompleted += TeacherListOnDownloadCompleted;
            var response = _teacherListClient.Value.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        private void TeacherListOnDownloadCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            GetTeachersListCompleted?.Invoke(sender, downloadStringCompletedEventArgs);
        }

        public Task<string> GetTeachersFullNameList()
        {
            var requestUrl = MakeRequestUrl(TeachersFullNameListFunction);
            if (!_teacherFullNameClient.IsValueCreated) _teacherFullNameClient.Value.DownloadStringCompleted += TeacherFullNameOnDownloadCompleted;
            var response = _teacherFullNameClient.Value.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        private void TeacherFullNameOnDownloadCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            GetTeachersFullNameListCompleted?.Invoke(sender, downloadStringCompletedEventArgs);
        }

        public Task<string> GetClassroomsList()
        {
            var requestUrl = MakeRequestUrl(ClassroomsListFunction);
            if (!_classroomListClient.IsValueCreated) _classroomListClient.Value.DownloadStringCompleted += ClassroomListOnDownloadCompleted; 
            var response = _classroomListClient.Value.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        private void ClassroomListOnDownloadCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            GetClassroomsListCompleted?.Invoke(sender, downloadStringCompletedEventArgs);
        }

        public Task<string> GetWeekScheduleForClass(string @class)
        {
            var requestUrl = MakeRequestUrl(WeekScheduleForClass, ClassArgument, @class);
            if (!_weekClassScheduleClient.IsValueCreated) _weekClassScheduleClient.Value.DownloadStringCompleted += WeekClassScheduleOnDownloadCompleted;
            var response = _weekClassScheduleClient.Value.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        private void WeekClassScheduleOnDownloadCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            GetWeekScheduleForClassCompleted?.Invoke(sender, downloadStringCompletedEventArgs);
        }

        public Task<string> GetChanges()
        {
            var requestUrl = ChangesUrl;
            if (!_changesClient.IsValueCreated) _changesClient.Value.DownloadStringCompleted += ChangesOnDownloadCompleted;
            var response = _changesClient.Value.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        private void ChangesOnDownloadCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            GetChangesCompleted?.Invoke(sender, downloadStringCompletedEventArgs);
        }

        public Task<string> GetWeekScheduleForTeacher(string teacherShortName)
        {
            var requestUrl = MakeRequestUrl(WeekScheduleForTeacher, TeacherArgumnet, teacherShortName);
            if (!_weekTeacherScheduleClient.IsValueCreated) _weekTeacherScheduleClient.Value.DownloadStringCompleted += WeekTeacherScheduleOnDownloadCompleted;
            var response = _weekTeacherScheduleClient.Value.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        private void WeekTeacherScheduleOnDownloadCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            GetWeekScheduleForTeacherCompleted?.Invoke(sender, downloadStringCompletedEventArgs);
        }

        public Task<string> GetDailyScheduleForClassrooms(int dayNum)
        {
            var requestUrl = MakeRequestUrl(DailyScheduleForClassrooms, DayArgument, dayNum.ToString());
            if (!_dailyClassroomScheduleClient.IsValueCreated) _dailyClassroomScheduleClient.Value.DownloadStringCompleted += DailyClassroomScheduleOnDownloadCompleted;
            var response = _dailyClassroomScheduleClient.Value.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        private void DailyClassroomScheduleOnDownloadCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            GetDailyScheduleForClassroomsCompleted?.Invoke(sender, downloadStringCompletedEventArgs);
        }

        public void GetTeacherListCancel()
        {
            _teacherListClient.Value.CancelAsync();
        }

        public void GetClassesListCancel()
        {
            _classListClient.Value.CancelAsync();
        }

        public void GetTeachersFullNameListCancel()
        {
            _teacherFullNameClient.Value.CancelAsync();
        }

        public void GetClassroomsListCancel()
        {
            _classroomListClient.Value.CancelAsync();
        }

        public void GetWeekScheduleForClassCancel()
        {
            _weekClassScheduleClient.Value.CancelAsync();
        }

        public void GetChangesCancel()
        {
            _changesClient.Value.CancelAsync();
        }

        public void GetWeekScheduleForTeacherCancel()
        {
            _weekTeacherScheduleClient.Value.CancelAsync();
        }

        public void GetDailyScheduleForClassroomsCancel()
        {
            _dailyClassroomScheduleClient.Value.CancelAsync();
        }
    }
}