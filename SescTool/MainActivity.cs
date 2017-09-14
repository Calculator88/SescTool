using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using SescTool.Fragments;
using SescTool.Framework;

namespace SescTool
{
	public class MainActivity : AppCompatActivity , View.IOnClickListener, NavigationView.IOnNavigationItemSelectedListener
	{
		private DrawerLayout _drawer;
	    private NavigationView _navigationView;

	    protected override void OnCreate(Bundle bundle)
	    {
            base.OnCreate(bundle);
	        SetContentView(Resource.Layout.main);
	        Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
	        var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

	        _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
	        _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navigationView.SetNavigationItemSelectedListener(this);

	        var toggle = new ActionBarDrawerToggle(this, _drawer, toolbar, Resource.String.open, Resource.String.close);
	        _drawer.AddDrawerListener(toggle);
	        _drawer.SetStatusBarBackgroundColor(ContextCompat.GetColor(this, Resource.Color.PrimaryDark));
	        toggle.SyncState();

             _navigationView.SetCheckedItem(Resource.Id.nav_class_schedule);
            OnClassScheduleSelected();
	    }

	    public void OnClick(View v)
	    {
	                                  
	    }

	    private void OnClassScheduleSelected()
	    {
	        if (SupportFragmentManager.FindFragmentByTag("class") == null)
	        {
	            SupportFragmentManager.BeginTransaction()
	                .Replace(Resource.Id.schedule_fragment_container, ServiceLocator.GetService<ClassScheduleFragment>(), "class")
	                .Commit();
	        }
	    }

	    private void OnTeacherScheduleSelected()
	    {
	        if (SupportFragmentManager.FindFragmentByTag("teacher") == null)
	        {
	            SupportFragmentManager.BeginTransaction()
	                .Replace(Resource.Id.schedule_fragment_container, new TeacherScheduleFragment(), "teacher")
	                .Commit();
	        }
	    }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
	    {
	        switch (menuItem.ItemId)
	        {
                case Resource.Id.nav_class_schedule:
                    OnClassScheduleSelected();
                    _drawer.CloseDrawers();
                    return true;
                case Resource.Id.nav_techer_schedule:
                    OnTeacherScheduleSelected();
                    _drawer.CloseDrawers();
                    return true;
                case Resource.Id.nav_classroom_timetable:
                    _drawer.CloseDrawers();
                    return true;
	        }
	        return true;
	    }
	}
}