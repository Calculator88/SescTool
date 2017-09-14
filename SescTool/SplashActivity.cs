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
            ServiceLocator.RegisterService(new ClassScheduleViewModel(new TimetableProvider(new TimetableDownloader())));
            ServiceLocator.RegisterService(new ClassScheduleFragment());
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }
    }
}