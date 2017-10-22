using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using SescTool.Fragments;
using SescTool.Framework;
using SescTool.Services;
using SescTool.ViewModels;

namespace SescTool
{
    internal class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ServiceLocator.RegisterService(new TimetableProvider());
            ServiceLocator.RegisterService(new ClassScheduleFragment());
            ServiceLocator.RegisterService(new ClassroomScheduleFragment());
            ServiceLocator.RegisterService(new ClassroomDailyScheduleViewModel());
            ServiceLocator.RegisterService(new ClassScheduleViewModel());
            ServiceLocator.RegisterService(new Cacher());
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }
    }
}