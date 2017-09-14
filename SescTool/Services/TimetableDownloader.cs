using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SescTool.Services.Abstractions;

namespace SescTool.Services
{
    public class TimetableDownloader : ITimetableDownloader
    {
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

        public async Task<string> GetClassesList()
        {
            var client = new WebClient {Encoding = Encoding.GetEncoding("windows-1251")};
            var requestUrl = MakeRequestUrl(ClassListFunction);
            var response = await client.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        public async Task<string> GetTeachersList()
        {
            var client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var requestUrl = MakeRequestUrl(TeachersListFunction);
            var response = await client.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        public async Task<string> GetTeachersFullNameList()
        {
            var client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var requestUrl = MakeRequestUrl(TeachersFullNameListFunction);
            var response = await client.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        public async Task<string> GetClassroomsList()
        {
            var client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var requestUrl = MakeRequestUrl(ClassListFunction);
            var response = await client.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        public async Task<string> GetWeekScheduleForClass(string @class)
        {
            var client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var requestUrl = MakeRequestUrl(WeekScheduleForClass, ClassArgument, @class);
            var response = await client.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        public async Task<string> GetChanges()
        {
            var client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var requestUrl = ChangesUrl;
            var response = await client.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        public async Task<string> GetWeekScheduleForTeacher(string teacherShortName)
        {
            var client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var requestUrl = MakeRequestUrl(WeekScheduleForTeacher, TeacherArgumnet, teacherShortName);
            var response = await client.DownloadStringTaskAsync(requestUrl);
            return response;
        }

        public async Task<string> GetDailyScheduleForClassrooms(int dayNum)
        {
            var client = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var requestUrl = MakeRequestUrl(DailyScheduleForClassrooms, DayArgument, dayNum.ToString());
            var response = await client.DownloadStringTaskAsync(requestUrl);
            return response;
        }
    }
}