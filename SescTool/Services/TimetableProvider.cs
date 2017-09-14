using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SescTool.Framework;
using SescTool.Model;
using SescTool.Services.Abstractions;

namespace SescTool.Services
{
    public class TimetableProvider : ITimetableProvider
    {
        private readonly ITimetableDownloader _downloader;
        public TimetableProvider(ITimetableDownloader downloader)
        {
            _downloader = downloader;
        }
        public async Task<string[]> GetClasses()
        {
            var respString = await _downloader.GetClassesList();
            if (String.IsNullOrEmpty(respString) || respString == "Timetable does not exist")
            {
                throw new NoClassesException();
            }
            return respString.Split('\n');
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

        public async Task<Dictionary<string, ScheduleWeek>> GetWeekScheduleForClass(string @class)
        {
            var respString = await _downloader.GetWeekScheduleForClass(@class);
            if (String.IsNullOrEmpty(respString) || respString == "Class does not exist" ||
                respString == "Timetable does not exist")
            {
                throw new NoTimetableException();
            }
            var result = JsonConvert.DeserializeObject<Dictionary<string, ScheduleWeek>>(respString);
            return result;
        }
    }
}