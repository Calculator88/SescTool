using System.Threading.Tasks;

namespace SescTool.Services.Abstractions
{
    public interface ITimetableDownloader
    {
        Task<string> GetClassesList();
        Task<string> GetTeachersList();
        Task<string> GetTeachersFullNameList();
        Task<string> GetClassroomsList();
        Task<string> GetWeekScheduleForClass(string @class);
        Task<string> GetChanges();
        Task<string> GetWeekScheduleForTeacher(string teacherShortName);
        Task<string> GetDailyScheduleForClassrooms(int dayNum);
    }
}