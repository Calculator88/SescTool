using System.Threading.Tasks;
using NUnit.Framework;
using SescTool.Services;

namespace SescTool.Tests
{
    [TestFixture]
    public class TestsApi
    {
        [Test]
        public async Task TestGetClassesList()
        {
            var downloader = new TimetableDownloader();
            var classes = await downloader.GetClassesList();
            Assert.AreNotEqual(classes, "Bad Request");
        }
        [Test]
        public async Task TestGetClassroomsList()
        {
            var downloader = new TimetableDownloader();
            var classes = await downloader.GetClassroomsList();
            Assert.AreNotEqual(classes, "Bad Request");
        }
        [Test]
        public async Task TestGetTeachersList()
        {
            var downloader = new TimetableDownloader();
            var classes = await downloader.GetTeachersList();
            Assert.AreNotEqual(classes, "Bad Request");
        }
        [Test]
        public async Task TestGetTeachersFullNameList()
        {
            var downloader = new TimetableDownloader();
            var classes = await downloader.GetTeachersFullNameList();
            Assert.AreNotEqual(classes, "Bad Request");
        }
        [Test]
        public async Task TestGetWeekScheduleForClass()
        {
            var downloader = new TimetableDownloader();
            var classes = await downloader.GetWeekScheduleForClass("11е");
            Assert.AreNotEqual(classes, "Bad Request");
            Assert.AreNotEqual(classes, "Class does not exist");
        }
        [Test]
        public async Task TestGetDailyScheduleForClassrooms()
        {
            var downloader = new TimetableDownloader();
            var classes = await downloader.GetDailyScheduleForClassrooms(1);
            Assert.AreNotEqual(classes, "Bad Request");
        }
        [Test]
        public async Task TestGetWeekScheduleForTeacher()
        {
            var downloader = new TimetableDownloader();
            var classes = await downloader.GetWeekScheduleForTeacher("Гейн Н. А.");
            Assert.AreNotEqual(classes, "Bad Request");
            Assert.AreNotEqual(classes, "Teacher does not exist or has no lessons");
        }

    }

    [TestFixture]
    public class TestsProvider
    {
        [Test]
        public async Task TestGetClassrooms()
        {
            var provider = new TimetableProvider(new TimetableDownloader());
            var classes = await provider.GetClassrooms();
            Assert.NotNull(classes);
        }
        [Test]
        public async Task TestGetTeachers()
        {
            var provider = new TimetableProvider(new TimetableDownloader());
            var classes = await provider.GetTeachers();
            Assert.NotNull(classes);
        }
        [Test]
        public async Task TestGetTeachersFullName()
        {
            var provider = new TimetableProvider(new TimetableDownloader());
            var classes = await provider.GetTeachersFullName();
            Assert.NotNull(classes);
        }
        [Test]
        public async Task GetWeekScheduleForClass()
        {
            var provider = new TimetableProvider(new TimetableDownloader());
            var classes = await provider.GetWeekScheduleForClass("11е");
            Assert.NotNull(classes);
        }
    }
}                  