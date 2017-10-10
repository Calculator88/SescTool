using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SescTool.Fragments;
using SescTool.Framework;

namespace SescTool
{
	public class MainActivity : AppCompatActivity, MainActivity.ILoadingToolbarShower
	{
        #region Private fields

        private DrawerLayout _drawer;
	    private NavigationView _navigationView;
	    private LinearLayout _appbarLoading;
	    private int _activeFragment;
	    private const int ClassSchedule = 0;
	    private const int TeacherSchedule = 1;
	    private const int ClassroomSchedule = 2;
	    private const string ActiveFragmentTag = "com.sesc.timetable.MainActivity.ActiveFragment";

        #endregion

        #region Fragment lifecycle

	    protected override void OnCreate(Bundle bundle)
	    {
	        base.OnCreate(bundle);
	        SetContentView(Resource.Layout.main);
	        Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
	        var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
	        _appbarLoading = FindViewById<LinearLayout>(Resource.Id.toolbar_loading_panel);
	        SetSupportActionBar(toolbar);

	        _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
	        _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
	        _navigationView.NavigationItemSelected += OnNavigationItemSelected;

	        var toggle = new ActionBarDrawerToggle(this, _drawer, toolbar, Resource.String.open, Resource.String.close);
	        _drawer.AddDrawerListener(toggle);
	        _drawer.SetStatusBarBackgroundColor(ContextCompat.GetColor(this, Resource.Color.primary_dark));
	        toggle.SyncState();

	        _navigationView.SetCheckedItem(Resource.Id.nav_class_schedule);

	        var transaction = SupportFragmentManager.BeginTransaction();
            if (SupportFragmentManager.FindFragmentByTag("class") == null)
	            transaction.Add(Resource.Id.schedule_fragment_container, new ClassScheduleFragment(), "class");
            if (SupportFragmentManager.FindFragmentByTag("classroom") == null)
	            transaction.Add(Resource.Id.schedule_fragment_container, new ClassroomScheduleFragment(), "classroom");
	        transaction.CommitNow();

	        var activeFragment = bundle?.GetInt(ActiveFragmentTag, 0) ?? 0;
	        switch (activeFragment)
	        {
                case ClassSchedule:
                    OnClassScheduleSelected();
                    break;
                case TeacherSchedule:
                    OnTeacherScheduleSelected();
                    break;
                case ClassroomSchedule:
                    OnClassroomScheduleSelected();
                    break;
	        }
	    }

	    protected override void OnSaveInstanceState(Bundle outState)
	    {
            outState.PutInt(ActiveFragmentTag, _activeFragment);
	        base.OnSaveInstanceState(outState);
	    }

	    #endregion

        #region Private methods

	    private void OnClassScheduleSelected()
	    {
	        _activeFragment = ClassSchedule;
	        SupportFragmentManager.BeginTransaction()
	            .Attach(SupportFragmentManager.FindFragmentByTag("class"))
	            .Detach(SupportFragmentManager.FindFragmentByTag("classroom"))
	            .Commit();
	        SupportActionBar.Subtitle = Resources.GetStringArray(Resource.Array.subtitles)[_activeFragment];
        }

        private void OnTeacherScheduleSelected()
	    {
	        // TODO
	    }

        private void OnClassroomScheduleSelected()
        {
            _activeFragment = ClassroomSchedule;
	        SupportFragmentManager.BeginTransaction()
	            .Attach(SupportFragmentManager.FindFragmentByTag("classroom"))
	            .Detach(SupportFragmentManager.FindFragmentByTag("class"))
	            .Commit();
            SupportActionBar.Subtitle = Resources.GetStringArray(Resource.Array.subtitles)[_activeFragment];
        }

        #endregion

        #region Event handlers

        private void OnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs navigationItemSelectedEventArgs)
	    {
	        switch (navigationItemSelectedEventArgs.MenuItem.ItemId)
	        {
	            case Resource.Id.nav_class_schedule:
	                OnClassScheduleSelected();
	                _drawer.CloseDrawers();
	                navigationItemSelectedEventArgs.Handled = true;
	                break;
	            case Resource.Id.nav_techer_schedule:
	                OnTeacherScheduleSelected();
	                _drawer.CloseDrawers();
	                navigationItemSelectedEventArgs.Handled = true;
	                break;
	            case Resource.Id.nav_classroom_timetable:
	                OnClassroomScheduleSelected();
	                _drawer.CloseDrawers();
	                navigationItemSelectedEventArgs.Handled = true;
	                break;
	        }
	    }

        #endregion

        #region Interface interactions

	    public void ShowLoading()
	    {
	        Title = null;
	        SupportActionBar.Subtitle = null;
	        _appbarLoading.Visibility = ViewStates.Visible;
	    }
	    public void StopLoading()
	    {
	        SetTitle(Resource.String.nav_timetable);
	        SupportActionBar.Subtitle = Resources.GetStringArray(Resource.Array.subtitles)[_activeFragment];
            _appbarLoading.Visibility = ViewStates.Gone;
	    }

        #endregion

        public interface ILoadingToolbarShower
        {
            void ShowLoading();
            void StopLoading();
        }
	}
}