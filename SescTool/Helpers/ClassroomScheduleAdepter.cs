using System;
using System.Linq;
using Android.Animation;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SescTool.Model.ClassroomSchedule;

namespace SescTool.Helpers
{
    public class ClassroomScheduleAdepter : RecyclerView.Adapter
    {
        public ScheduleDay Data { get; }
        private readonly Context _context;
        private readonly IToItemScroller _scroller;
        private readonly IFreeClassrooomsShower _shower;
        public ClassroomScheduleAdepter(ScheduleDay day, Context context, IToItemScroller scroller, IFreeClassrooomsShower shower)
        {
            Data = day;
            _context = context;
            _scroller = scroller;
            _shower = shower;
            _shower = shower;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((ClassroomScheduleViewHolder)holder).BindScheduleLesson(Data.Timetable[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(_context).Inflate(Resource.Layout.classroom_schedule_card, parent, false);
            view.Measure(View.MeasureSpec.MakeMeasureSpec(parent.Width, MeasureSpecMode.Exactly), ViewGroup.LayoutParams.WrapContent);
            var width = view.MeasuredWidth - ((RecyclerView.LayoutParams)view.LayoutParameters).LeftMargin
                        - ((RecyclerView.LayoutParams)view.LayoutParameters).RightMargin;
            var margins2 = (parent.Width - 1400) / 2;
            if (margins2 <= 0)
                view.LayoutParameters.Width = width;
            else
            {
                view.LayoutParameters.Width = 1400;
                ((RecyclerView.LayoutParams)view.LayoutParameters).LeftMargin = margins2;
                ((RecyclerView.LayoutParams)view.LayoutParameters).RightMargin = margins2;
            }
            return new ClassroomScheduleViewHolder(view, _context, _scroller, _shower);
        }

        public override int ItemCount => Data.Timetable.Count;

        public class ClassroomScheduleViewHolder : RecyclerView.ViewHolder
        {
            private int _lessonNum;
            private readonly TextView _textViewLesson;
            private readonly RecyclerView _listViewLessons;
            private readonly Context _context;
            private readonly IFreeClassrooomsShower _shower;

            public void BindScheduleLesson(ScheduleLesson schedule)
            {
                _textViewLesson.Text = schedule.LessonNumber + " урок";
                _lessonNum = schedule.LessonNumber - 1;
                _listViewLessons.SetAdapter(new ClassroomLessonAdapter(
                    schedule.Classrooms
                        .Where(arg => !String.IsNullOrEmpty(arg.Class) || !String.IsNullOrEmpty(arg.Subject)).ToList(),
                    _context));
            }
            private ValueAnimator SlideAnimator(int start, int end)
            {

                var animator = ValueAnimator.OfInt(start, end);
                animator.Update +=
                    (sender, e) => {
                        var value = (int)animator.AnimatedValue;
                        var layoutParams = _listViewLessons.LayoutParameters;
                        layoutParams.Height = Math.Abs(e.Animation.AnimatedFraction - 1f) < 0.0001f ? ViewGroup.LayoutParams.WrapContent : value;
                        _listViewLessons.LayoutParameters = layoutParams;
                    };
                return animator;
            }
            public ClassroomScheduleViewHolder(View itemView, Context context, IToItemScroller scroller, IFreeClassrooomsShower shower) : base(itemView)
            {
                _context = context;
                _textViewLesson = itemView.FindViewById<TextView>(Resource.Id.text_number_of_lesson);
                _listViewLessons = itemView.FindViewById<RecyclerView>(Resource.Id.classroom_schedule_list_view);
                itemView.FindViewById<Button>(Resource.Id.free_classroooms_button).Click += FreeClassroomsOnClick;
                _shower = shower;
                var manager = new LinearLayoutManager(context);
                _listViewLessons.SetLayoutManager(manager);
                _listViewLessons.AddItemDecoration(new DividerItemDecoration(context, manager.Orientation));
                _listViewLessons.Visibility = ViewStates.Gone;

                if (itemView.Clickable) return;
                itemView.Clickable = true;
                itemView.Click += (sender, args) =>
                {
                    if (_listViewLessons.Visibility == ViewStates.Gone)
                    {
                        
                        _listViewLessons.Measure(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                        _listViewLessons.Visibility = ViewStates.Visible;

                        var mAnimator = SlideAnimator(0, _listViewLessons.MeasuredHeight);
                        mAnimator.Start();
                        mAnimator.SetDuration(300);
                        mAnimator.AnimationEnd += (o, eventArgs) =>
                        {
                        scroller.ScrollTo(_lessonNum);
                        };
                    }
                    else
                    {
                        var finalHeight = _listViewLessons.Height;

                        var mAnimator = SlideAnimator(finalHeight, 0);
                        mAnimator.Start();
                        mAnimator.SetDuration(300);
                        mAnimator.AnimationEnd += (intentSender, arg) =>
                        {
                            _listViewLessons.Visibility = ViewStates.Gone;
                        };

                    }
                };

            }

            private void FreeClassroomsOnClick(object sender1, EventArgs eventArgs1)
            {
                _shower.ShowFreeClassrooms(_lessonNum);
            }
        }
        public interface IToItemScroller
        {
            void ScrollTo(int position);
        }
        public interface IFreeClassrooomsShower
        {
            void ShowFreeClassrooms(int lesson);
        }
    }
}