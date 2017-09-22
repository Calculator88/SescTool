﻿using System.ComponentModel;
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
        private bool _errorLoadingClassListOccured;
        private bool _errorLoadingScheduleOccured;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _viewModel = ServiceLocator.GetService<ClassScheduleViewModel>();
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            _viewModel.ExceptionOccured += ViewModelExceptionOccured;
        }

        public override void OnDestroy()
        {
            _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            _viewModel.ExceptionOccured -= ViewModelExceptionOccured;
            base.OnDestroy();
        }

        private void ViewModelExceptionOccured(Exception exception, ClassScheduleViewModel.Method method)
        {
            if (method == ClassScheduleViewModel.Method.LoadClass) _errorLoadingClassListOccured = true;
            else _errorLoadingScheduleOccured = true;
        }
        private void OnIsScheduleLoadingChanged()
        {
            if (_viewModel.IsWeekClassSchduleLoading)
            {
                _behaviorLine.Visibility = ViewStates.Visible;
                _behaviorProgressBar.Visibility = ViewStates.Visible;
                _behaviorImageView.Visibility = ViewStates.Gone;
                _behaviorTextView.SetText(Resource.String.loading);
                _behaviorLine.SetBackgroundColor(new Color(0xd6, 0xcc, 0x15, 0xFF));
                return;
            }
            if (!_viewModel.IsWeekClassSchduleLoading && !_errorLoadingScheduleOccured)
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
            _errorLoadingScheduleOccured = false;
        }

        private void OnIsLoadingClassListChanged()
        {
            if (_viewModel.IsClassListLoading)
            {
                _selectClassMenuItem?.SetActionView(new ProgressBar(Context) {Indeterminate = true});
                _selectClassMenuItem?.SetEnabled(false);
            }
            else
            {
                _selectClassMenuItem?.SetActionView(null);
                OnClassesChanged();
            }
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

            if (_viewModel.Classes == null || _viewModel.Classes.Count == 0)
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

        private void UpdateLinks()
        {
            OnClassesChanged();
            OnCurrentClassChanged();
            OnIsLoadingClassListChanged();
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

        public void OnRefresh()
        {
            _swipeRefresh.Refreshing = false;
        }

        public void OnClassChoose(string @class)
        {
            if (@class == _viewModel.CurrentClass) return;
            _viewModel.RequestSchedule(@class);
        }

        public void Run()
        {
            _behaviorLine.Visibility = ViewStates.Gone;
        }
    }

}