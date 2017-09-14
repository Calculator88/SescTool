using System.ComponentModel;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using SescTool.Framework;
using SescTool.Helpers;
using SescTool.ViewModels;
using Exception = System.Exception;

namespace SescTool.Fragments
{
    public class ClassScheduleFragment : Fragment, SwipeRefreshLayout.IOnRefreshListener, ClassPickerDialogFragment.IOnClassChooseListener, IRunnable
    {
        private RecyclerView _recycler;
        private SwipeRefreshLayout _swipeRefresh;
        private IMenuItem _selectClassMenuItem;
        private ClassScheduleViewModel _viewModel;
        private LinearLayout _behaviorLine;
        private ImageView _behaviorImageView;
        private ProgressBar _behaviorProgressBar;
        private TextView _behaviorTextView;
        private bool _errorOccured;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _viewModel = ServiceLocator.GetService<ClassScheduleViewModel>();
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            _viewModel.OnExceptionOccured += ViewModelOnExceptionOccured;
        }

        public override void OnDestroy()
        {
            _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            _viewModel.OnExceptionOccured -= ViewModelOnExceptionOccured;
            base.OnDestroy();
        }

        private void ViewModelOnExceptionOccured(Exception exception, ClassScheduleViewModel.Method method)
        {
            _errorOccured = true;
        }

        private void UpdateVmBindings()
        {
            OnClassesChanged();
            OnCurrentClassChanged();
            OnIsBusyChanged();
        }

        private void OnIsBusyChanged()
        {
            if (_viewModel.IsBusy)
            {
                _behaviorLine.Visibility = ViewStates.Visible;
                _behaviorProgressBar.Visibility = ViewStates.Visible;
                _behaviorImageView.Visibility = ViewStates.Gone;
                _behaviorTextView.SetText(Resource.String.loading);
                _behaviorLine.SetBackgroundColor(new Color(0xd6, 0xcc, 0x15, 0xFF));
                return;
            }
            if (!_viewModel.IsBusy && !_errorOccured)
            {
                _behaviorLine.Visibility = ViewStates.Visible;
                _behaviorProgressBar.Visibility = ViewStates.Gone;
                _behaviorImageView.Visibility = ViewStates.Visible;
                _behaviorImageView.SetImageResource(Resource.Drawable.ok_sign);
                _behaviorTextView.SetText(Resource.String.loaded_fine);
                _behaviorLine.SetBackgroundColor(new Color(0x15, 0xd6, 0x55, 0xFF));
                _behaviorLine.Animate().SetStartDelay(1000).WithEndAction(this).Start();
                return;
            }
            _behaviorLine.Visibility = ViewStates.Visible;
            _behaviorProgressBar.Visibility = ViewStates.Gone;
            _behaviorImageView.Visibility = ViewStates.Visible;
            _behaviorImageView.SetImageResource(Resource.Drawable.warning_sign);
            _behaviorTextView.SetText(Resource.String.error_occured);
            _behaviorLine.SetBackgroundColor(new Color(0xd6, 0x15, 0x15, 0xFF));
            _behaviorLine.Animate().SetStartDelay(1000).WithEndAction(this).Start();
            _errorOccured = false;
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
                case nameof(_viewModel.IsBusy):
                    OnIsBusyChanged();
                    break;
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
                _recycler.SetAdapter(null);
                return;
            }
            _recycler?.SetAdapter(new ClassScheduleAdapter(Context, _viewModel.Schedules[_viewModel.CurrentClass]));
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.class_timetable_fragment, container, false);
            _recycler = v.FindViewById<RecyclerView>(Resource.Id.classes_timetable_recycler);
            _recycler.SetLayoutManager(new LinearLayoutManager(Context));
            _swipeRefresh = v.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _behaviorProgressBar = v.FindViewById<ProgressBar>(Resource.Id.behavior_progressBar);
            _behaviorTextView = v.FindViewById<TextView>(Resource.Id.behavior_text);
            _behaviorImageView = v.FindViewById<ImageView>(Resource.Id.behavior_image);
            _behaviorLine = v.FindViewById<LinearLayout>(Resource.Id.classSchedule_behavior_line);
            _swipeRefresh.SetOnRefreshListener(this);
            HasOptionsMenu = true;

            UpdateVmBindings();

            if (_viewModel.Classes == null || _viewModel.Classes.Count == 0)
                _viewModel.LoadClasses();

            return v;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.class_schedule_menu, menu);
            _selectClassMenuItem = menu.FindItem(Resource.Id.choose_class_menu_item);
            _selectClassMenuItem.SetEnabled(_viewModel.Classes != null && _viewModel.Classes.Count != 0);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.choose_class_menu_item:
                    new ClassPickerDialogFragment(_viewModel.Classes, this).Show(FragmentManager, "classpicker");
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnRefresh()
        {
            _swipeRefresh.Refreshing = false;
        }

        public void OnClassChoose(string @class)
        {
            _viewModel.LoadSchedule(@class);
        }

        public void Run()
        {
            _behaviorLine.Visibility = ViewStates.Gone;
        }
    }

}