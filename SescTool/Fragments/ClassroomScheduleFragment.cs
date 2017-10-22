using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SescTool.Framework;
using SescTool.Helpers;
using SescTool.ViewModels;
using Exception = System.Exception;

namespace SescTool.Fragments
{
    public class ClassroomScheduleFragment : Fragment, ClassroomScheduleAdepter.IToItemScroller, ClassroomScheduleAdepter.IFreeClassrooomsShower
    {
        #region Private fields

        private ClassroomDailyScheduleViewModel _viewModel;
        private SwipeRefreshLayout _swipeRefresh;
        private RecyclerView _recycler;
        private IMenuItem _menuItem;
        private MainActivity.ILoadingToolbarShower _loader;

        #endregion

        #region Fragment lifecycle

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _loader = Activity as MainActivity.ILoadingToolbarShower;
            _viewModel = ServiceLocator.GetService<ClassroomDailyScheduleViewModel>();
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            _viewModel.ExceptionOccured += ViewModelOnExceptionOccured;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.classroom_schedule_fragment, container, false);
            _recycler = view.FindViewById<RecyclerView>(Resource.Id.classrooms_timetable_recycler);
            _recycler.SetLayoutManager(new LinearLayoutManager(Context) { SmoothScrollbarEnabled = true });
            _swipeRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayoutClassrooms);
            _swipeRefresh.Refresh += (sender, args) => { _viewModel.ReqestClassroomSchedule(_viewModel.CurrentDay, true); };
            _swipeRefresh.SetColorSchemeColors(ContextCompat.GetColor(Context, Resource.Color.accent));
            HasOptionsMenu = true;

            if (!_viewModel.IsClassroomListLoading && (_viewModel.Classrooms == null || _viewModel.Classrooms.Count == 0))
                _viewModel.ReqestClassroomList();
            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.classroom_schedule_menu, menu);
            _menuItem = menu.FindItem(Resource.Id.classroom_select_day_spinner);
            UpdateLinks();
        }

        public override void OnDestroy()
        {
            _viewModel.ExceptionOccured -= ViewModelOnExceptionOccured;
            _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            base.OnDestroy();
        }

        #endregion

        #region Event handlers

        private void ViewModelOnExceptionOccured(Exception exception)
        {
            Toast.MakeText(Context, "An exception occured", ToastLength.Long).Show();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(_viewModel.CurrentDay):
                    OnCurrentDayChanged();
                    break;
                case nameof(_viewModel.IsLoading):
                    OnIsLoadingChanged();
                    break;
                case nameof(_viewModel.IsRefreshing):
                    OnIsRefreshingChanged();
                    break;
                case nameof(_viewModel.IsClassroomListLoading):
                    OnIsClassroomListLoading();
                    break;
            }
        }

        private void SpinnerOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            var item = (TextView)itemSelectedEventArgs.Parent.GetChildAt(0);
            item.SetTextColor(Color.White);
            item.TextAlignment = TextAlignment.Center;
            item.SetTextSize(ComplexUnitType.Pt, 9);
            var day = Resources.GetStringArray(Resource.Array.classroom_days)[itemSelectedEventArgs.Position];
            _viewModel.ReqestClassroomSchedule(day, false);
        }

        #endregion

        #region ViewModel listeners

        private void UpdateLinks()
        {
            OnCurrentDayChanged();
            OnIsClassroomListLoading();
            OnIsLoadingChanged();
            OnIsRefreshingChanged();
        }

        private void OnIsClassroomListLoading()
        {
            if (_viewModel.IsClassroomListLoading)
            {
                _menuItem?.SetActionView(new ProgressBar(Context)
                {
                    Indeterminate = true,
                    IndeterminateTintList =
                        ColorStateList.ValueOf(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_text))),
                    LayoutParameters = new AppBarLayout.LayoutParams
                    (
                        (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 30, Context.Resources.DisplayMetrics),
                        (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 30, Context.Resources.DisplayMetrics))
                });
                _menuItem?.SetEnabled(false);
                return;
            }
            var days = Resources.GetStringArray(Resource.Array.classroom_days).ToList();
            var currentDay = _viewModel.CurrentDay;
            if (String.IsNullOrEmpty(currentDay)) currentDay = "Понедельник";
            var dayindex = days.IndexOf(currentDay);

            var spinner = new Spinner(Context, null) { Prompt = "Выберите день" };
            var adapter = ArrayAdapter<string>.CreateFromResource(Context, Resource.Array.classroom_days,
                Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Resource.Layout.spinner_item);
            spinner.Adapter = adapter;
            spinner.SetSelection(dayindex);
            spinner.ItemSelected += SpinnerOnItemSelected;
            _menuItem?.SetActionView(spinner);
            _menuItem?.SetEnabled(_viewModel.Classrooms != null && _viewModel.Classrooms.Count != 0);
        }

        private void OnIsRefreshingChanged()
        {
            _swipeRefresh.Refreshing = _viewModel.IsRefreshing;
        }

        private void OnIsLoadingChanged()
        {
            if (_viewModel.IsLoading)
            {
                _loader?.ShowLoading();
                return;
            }
            _loader?.StopLoading();
        }

        private void OnCurrentDayChanged()
        {
            if (!_viewModel.ScheduleExistsForDay(_viewModel.CurrentDay))
            {
                _recycler?.SetAdapter(null);
                return;
            }
            _recycler?.SetAdapter(new ClassroomScheduleAdepter(_viewModel.Schedules[_viewModel.CurrentDay], this, this));
        }

        #endregion

        #region Public methods

        public void ScrollTo(int position)
        {
            ((LinearLayoutManager)_recycler?.GetLayoutManager())?.StartSmoothScroll(new SmoothScroller(Context) { TargetPosition = position });
        }

        public void ShowFreeClassrooms(int lesson)
        {
            if (_viewModel.Classrooms == null || _viewModel.Classrooms.Count == 0) return;
            var freeCr = _viewModel.Classrooms.Except(_viewModel.Schedules[_viewModel.CurrentDay].Timetable[lesson]
                    .Classrooms.Where(arg => !String.IsNullOrEmpty(arg.Class) && !String.IsNullOrEmpty(arg.Subject))
                    .Select(arg => arg.Classroom)).Except(new[] { "F", "S" }).GroupBy(arg => arg[0])
                .Select(arg => arg.Aggregate((first, second) => first + ' ' + second));

            new AlertDialog.Builder(Context)
                .SetTitle("Свободные аудитории")
                .SetMessage(freeCr.Aggregate((first, second) => first + '\n' + second))
                .SetPositiveButton(Android.Resource.String.Ok, (sender, args) => { })
                .Create().Show();
        }

        #endregion

        #region Private members

        private class SmoothScroller : LinearSmoothScroller
        {
            public SmoothScroller(Context context) : base(context)
            {
            }

            protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
            {
                return displayMetrics.Density / 30;
                // return TypedValue.ApplyDimension(ComplexUnitType.Dip, 0.03f, displayMetrics);
            }

            protected override int VerticalSnapPreference { get; } = SnapToStart;
        }

        #endregion

    }
}