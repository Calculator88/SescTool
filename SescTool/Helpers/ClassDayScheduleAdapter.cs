using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Apmem;
using SescTool.Model.ClassSchedule;

namespace SescTool.Helpers
{
    internal class ClassDayScheduleAdapter : RecyclerView.Adapter
    {
        private readonly List<Lesson> _lessons;
        private readonly Context _context;
        public ClassDayScheduleAdapter(Context context, List<Lesson> lessons)
        {
            _lessons = lessons;
            _context = context;
        }

        public override int GetItemViewType(int position)
        {
            foreach (var lesson in _lessons)
            {
                if (lesson.Number == position + 1 && lesson.LessonsByGroups.Count == 1 && lesson.LessonsByGroups[0].Group == 0)
                    return 1;
                if (lesson.Number == position + 1 && lesson.LessonsByGroups.Count != 0)
                    return 2;
            }
            return 0;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewType = GetItemViewType(position);
            switch (viewType)
            {
                case 0:
                    ((EmptyLessonViewHolder) holder).BindLesson(new Lesson {Number = position + 1});
                    return;
                case 1:
                    ((CommonLessonViewHolder)holder).BindLesson(_lessons.FirstOrDefault(lesson => lesson.Number == position + 1));
                    return;
                case 2:
                    ((SplitedLessonViewHolder)holder).BindLesson(_lessons.FirstOrDefault(lesson => lesson.Number == position + 1));
                    return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case 0:
                    var viewEmpty = LayoutInflater.From(parent.Context)
                        .Inflate(Resource.Layout.class_schedule_empty_lesson, parent, false);
                    return new EmptyLessonViewHolder(viewEmpty);
                case 1:
                    var viewCommon = LayoutInflater.From(parent.Context)
                        .Inflate(Resource.Layout.common_lesson_list_item, parent, false);
                    return new CommonLessonViewHolder(viewCommon);
                default:
                    var viewSplited = LayoutInflater.From(parent.Context)
                        .Inflate(Resource.Layout.splited_lesson_list_item, parent, false);
                    return new SplitedLessonViewHolder(viewSplited);
            }
        }

        public override int ItemCount => 7;

        internal class EmptyLessonViewHolder : RecyclerView.ViewHolder
        {
            private readonly TextView _textViewLessonNum;

            public void BindLesson(Lesson lesson)
            {
                _textViewLessonNum.Text = lesson.Number.ToString();
            }

            public EmptyLessonViewHolder(View view) : base(view)
            {
                _textViewLessonNum = view.FindViewById<TextView>(Resource.Id.text_empty_lesson_number);
            }
        }

        internal class CommonLessonViewHolder : RecyclerView.ViewHolder , View.IOnClickListener
        {
            private readonly TextView _textViewLessonNum;
            private readonly TextView _textViewSubject;
            private readonly TextView _textViewClassroom;
            private Lesson _lesson;

            public void BindLesson(Lesson lesson)
            {
                _lesson = lesson;
                _textViewLessonNum.Text = lesson.Number.ToString();
                _textViewSubject.Text = lesson.LessonsByGroups[0].Subject;
                _textViewClassroom.Text = lesson.LessonsByGroups[0].Classroom;
                if (String.IsNullOrEmpty(_textViewClassroom.Text))
                    _textViewClassroom.Visibility = ViewStates.Gone;

            }
            public CommonLessonViewHolder(View view) : base(view)
            {
                _textViewLessonNum = view.FindViewById<TextView>(Resource.Id.text_common_lesson_number);
                _textViewSubject = view.FindViewById<TextView>(Resource.Id.text_common_lesson_subject);
                _textViewClassroom = view.FindViewById<TextView>(Resource.Id.text_common_lesson_classroom);
                view.FindViewById<LinearLayout>(Resource.Id.common_lesson_panel).SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                if (!String.IsNullOrEmpty(_lesson.LessonsByGroups[0].Teacher))
                    Toast.MakeText(Application.Context, "Учитель: " + _lesson.LessonsByGroups[0].Teacher,
                        ToastLength.Long).Show();
            }
        }

        internal class SplitedLessonViewHolder : RecyclerView.ViewHolder  , View.IOnClickListener
        {
            private readonly TextView _textViewLessonNum;
            private readonly TextView _textViewSubject1;
            private readonly TextView _textViewClassroom1;
            private readonly TextView _textViewSubject2;
            private readonly TextView _textViewClassroom2;
            private readonly FlowLayout _firstLes;
            private readonly FlowLayout _secondLes;

            private Lesson _lesson;

            private Drawable GetRippleBackground()
            {
                var attrs = new[] { Android.Resource.Attribute.SelectableItemBackground };
                var ta = _firstLes.Context.ObtainStyledAttributes(attrs);
                var selectedItemDrawable = ta.GetDrawable(0);
                ta.Recycle();
                return selectedItemDrawable;
            }

            public void BindLesson(Lesson lesson)
            {
                _secondLes.Background = null;
                _secondLes.Clickable = false;
                _secondLes.SetOnClickListener(null);
                _firstLes.Background = null;
                _firstLes.Clickable = false;
                _firstLes.SetOnClickListener(null);

                _lesson = lesson;
                if (lesson.LessonsByGroups.Count == 1)
                {
                    _textViewLessonNum.Text = lesson.Number.ToString();
                    if (lesson.LessonsByGroups[0].Group == 1)
                    {
                        _textViewSubject1.Text = lesson.LessonsByGroups[0].Subject;
                        _textViewClassroom1.Text = lesson.LessonsByGroups[0].Classroom;
                        _firstLes.Background = GetRippleBackground();
                        _firstLes.Clickable = true;
                        _firstLes.SetOnClickListener(this);
                    }
                    else
                    {
                        _textViewSubject2.Text = lesson.LessonsByGroups[0].Subject;
                        _textViewClassroom2.Text = lesson.LessonsByGroups[0].Classroom;
                        _secondLes.Background = GetRippleBackground();
                        _secondLes.Clickable = true;
                        _secondLes.SetOnClickListener(this);
                    }
                }
                else
                {
                    _textViewLessonNum.Text = lesson.Number.ToString();
                    _textViewSubject1.Text = lesson.LessonsByGroups[0].Subject;
                    _textViewClassroom1.Text = lesson.LessonsByGroups[0].Classroom;
                    _firstLes.Background = GetRippleBackground();
                    _firstLes.Clickable = true;
                    _firstLes.SetOnClickListener(this);
                    _textViewSubject2.Text = lesson.LessonsByGroups[1].Subject;
                    _textViewClassroom2.Text = lesson.LessonsByGroups[1].Classroom;
                    _secondLes.Background = GetRippleBackground();
                    _secondLes.Clickable = true;
                    _secondLes.SetOnClickListener(this);
                }
                if (String.IsNullOrEmpty(_textViewClassroom1.Text))
                    _textViewClassroom1.Visibility = ViewStates.Gone;
                if (String.IsNullOrEmpty(_textViewClassroom2.Text))
                    _textViewClassroom2.Visibility = ViewStates.Gone;
            }
            public SplitedLessonViewHolder(View view) : base(view)
            {
                _textViewLessonNum = view.FindViewById<TextView>(Resource.Id.text_splited_lesson_number);
                _textViewSubject1 = view.FindViewById<TextView>(Resource.Id.text_splited_lesson_first);
                _textViewSubject2 = view.FindViewById<TextView>(Resource.Id.text_splited_lesson_second);
                _textViewClassroom1 = view.FindViewById<TextView>(Resource.Id.text_splited_classroom_first);
                _textViewClassroom2 = view.FindViewById<TextView>(Resource.Id.text_splited_classroom_second);
                _firstLes = view.FindViewById<FlowLayout>(Resource.Id.splited_lesson_first_panel);
                _secondLes = view.FindViewById<FlowLayout>(Resource.Id.splited_lesson_second_panel);
            }

            public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.splited_lesson_first_panel when !String.IsNullOrEmpty(_lesson.LessonsByGroups[0].Teacher):
                        Toast.MakeText(Application.Context, "Учитель: " + _lesson.LessonsByGroups[0].Teacher,
                            ToastLength.Long).Show();
                        break;
                    case Resource.Id.splited_lesson_second_panel when _lesson.LessonsByGroups.Count == 1 && !String.IsNullOrEmpty(_lesson.LessonsByGroups[0].Teacher):
                        Toast.MakeText(Application.Context, "Учитель: " + _lesson.LessonsByGroups[0].Teacher,
                            ToastLength.Long).Show();
                        break;
                    case Resource.Id.splited_lesson_second_panel when !String.IsNullOrEmpty(_lesson.LessonsByGroups[1].Teacher):
                        Toast.MakeText(Application.Context, "Учитель: " + _lesson.LessonsByGroups[1].Teacher,
                            ToastLength.Long).Show();
                        break;
                }
            }
        }
    }
}