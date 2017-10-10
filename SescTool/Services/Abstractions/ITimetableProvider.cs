using System.Collections.Generic;
using System.Threading.Tasks;
using SescTool.Model.ClassSchedule;

namespace SescTool.Services.Abstractions
{
    public interface ITimetableProvider
    {
        Task<string[]> GetClasses();
        Task<string[]> GetTeachers();
        Task<string[]> GetTeachersFullName();
        Task<string[]> GetClassrooms();
        Task<Dictionary<string, ScheduleWeek>> GetWeekScheduleForClass(string @class);
    }
}