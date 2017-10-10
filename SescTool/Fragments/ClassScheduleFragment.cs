using System.ComponentModel;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
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
    public class ClassScheduleFragment : Fragment, ClassPickerDialogFragment.IOnClassChooseListener
    {
        #region Private fields

        private RecyclerView _recycler;
        private SwipeRefreshLayout _swipeRefresh;
        private IMenuItem _selectClassMenuItem;
        private MainActivity.ILoadingToolbarShower _loader;
        private ClassScheduleViewModel _viewModel;

        #endregion

        #region Fragment lifecycle

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _loader = Activity as MainActivity.ILoadingToolbarShower;
            _viewModel = ServiceLocator.GetService<ClassScheduleViewModel>();
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            _viewModel.ExceptionOccured += ViewModelExceptionOccured;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.class_timetable_fragment, container, false);
            _recycler = v.FindViewById<RecyclerView>(Resource.Id.classes_timetable_recycler);
            _recycler.SetLayoutManager(new LinearLayoutManager(Context));
            _swipeRefresh = v.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefresh.Refresh += (sender, args) => { _viewModel.RequestSchedule(_viewModel.CurrentClass, true); };
            _swipeRefresh.SetColorSchemeColors(ContextCompat.GetColor(Context, Resource.Color.accent));
            HasOptionsMenu = true;

            if (!_viewModel.IsClassListLoading && (_viewModel.Classes == null || _viewModel.Classes.Count == 0))
                _viewModel.RequestClasses();
            return v;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.class_schedule_menu, menu);
            _selectClassMenuItem = menu.FindItem(Resource.Id.choose_class_menu_item);
            UpdateLinks();
        }

        public override void OnDestroy()
        {
            _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            _viewModel.ExceptionOccured -= ViewModelExceptionOccured;
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.choose_class_menu_item:
                    new ClassPickerDialogFragment(_viewModel.Classes, this, _viewModel.CurrentClass).Show(FragmentManager, "classpicker");
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Event handlers

        private void ViewModelExceptionOccured(Exception exception, ClassScheduleViewModel.Method method)
        {
            Toast.MakeText(Context, "An exception occured", ToastLength.Long).Show();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(_viewModel.Classes):
                    OnClassesChanged();
                    break;
                case nameof(_viewModel.CurrentClass):
                    OnCurrentClassChanged();
                    break;
                case nameof(_viewModel.IsWeekClassSchduleLoading):
                    OnIsScheduleLoadingChanged();
                    break;
                case nameof(_viewModel.IsClassListLoading):
                    OnIsLoadingClassListChanged();
                    break;
                case nameof(_viewModel.IsRefreshing):
                    OnRefreshChanged();
                    break;
            }
        }

        #endregion

        #region ViewModel listeners

        private void OnIsScheduleLoadingChanged()
        {
            if (_viewModel.IsWeekClassSchduleLoading)
            {
                _loader?.ShowLoading();
                return;
            }
            _loader?.StopLoading();
        }
        private void OnIsLoadingClassListChanged()
        {
            if (_viewModel.IsClassListLoading)
            {
                _selectClassMenuItem?.SetActionView(new ProgressBar(Context)
                {
                    Indeterminate = true,
                    IndeterminateTintList =
                        ColorStateList.ValueOf(new Color(ContextCompat.GetColor(Context, Resource.Color.primary_text))),
                    LayoutParameters = new AppBarLayout.LayoutParams
                    (
                        (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 30, Context.Resources.DisplayMetrics),
                        (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 30, Context.Resources.DisplayMetrics)),
                });
                _selectClassMenuItem?.SetEnabled(false);
            }
            else
            {
                _selectClassMenuItem?.SetActionView(null);
                OnClassesChanged();
            }
        }
        private void OnClassesChanged()
        {
            _selectClassMenuItem?.SetEnabled(_viewModel.Classes != null && _viewModel.Classes.Count != 0);
        }
        private void OnCurrentClassChanged()
        {
            if (!_viewModel.ScheduleExistsForClass(_viewModel.CurrentClass))
            {
                _recycler?.SetAdapter(null);
                _selectClassMenuItem?.SetTitle(Resource.String.choose_class);
                return;
            }
            _recycler?.SetAdapter(new ClassScheduleAdapter(Context, _viewModel.Schedules[_viewModel.CurrentClass]));
            _selectClassMenuItem?.SetTitle(_viewModel.CurrentClass);
        }
        private void OnRefreshChanged()
        {
            _swipeRefresh.Refreshing = _viewModel.IsRefreshing;
        }
        private void UpdateLinks()
        {
            OnClassesChanged();
            OnCurrentClassChanged();
            OnIsLoadingClassListChanged();
            OnRefreshChanged();
        }

        #endregion

        #region Public methods

        public void OnClassChoose(string @class)
        {
            if (@class == _viewModel.CurrentClass) return;
            _viewModel.RequestSchedule(@class, false);
        }

        #endregion

    }

}