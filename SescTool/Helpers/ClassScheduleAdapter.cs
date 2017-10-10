using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SescTool.Model.ClassSchedule;

namespace SescTool.Helpers
{
    internal class ClassScheduleAdapter : RecyclerView.Adapter
    {
        public ScheduleWeek Data { get; }

        private readonly Context _context;
        public ClassScheduleAdapter(Context context, ScheduleWeek schedule)
        {
            Data = schedule;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((ClassScheduleViewHolder)holder).BindScheduleDay(Data.Timetable[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = (CardView)LayoutInflater.From(parent.Context).Inflate(Resource.Layout.class_schedule_card, parent, false);
            view.Measure(View.MeasureSpec.MakeMeasureSpec(parent.Width, MeasureSpecMode.Exactly), ViewGroup.LayoutParams.WrapContent);
            var width = view.MeasuredWidth - ((RecyclerView.LayoutParams)view.LayoutParameters).LeftMargin
                                           - ((RecyclerView.LayoutParams)view.LayoutParameters).RightMargin;
            var margins2 = (parent.Width - 1400)/2;
            if (margins2 <= 0)
                view.LayoutParameters.Width = width;
            else
            {
                view.LayoutParameters.Width = 1400;
                ((RecyclerView.LayoutParams) view.LayoutParameters).LeftMargin = margins2;
                ((RecyclerView.LayoutParams) view.LayoutParameters).RightMargin = margins2;
            }
            return new ClassScheduleViewHolder(view);
        }

        public override int ItemCount => Data.Timetable.Count;

        internal class ClassScheduleViewHolder : RecyclerView.ViewHolder
        {
            private readonly TextView _textViewDay;
            private readonly RecyclerView _listViewLessons;
            public void BindScheduleDay(ScheduleDay schedule)
            {
                if (schedule.ExistsChanges)
                {
                    ((CardView)ItemView).SetCardBackgroundColor(ContextCompat.GetColor(ItemView.Context, Resource.Color.accent));
                }
                else
                {
                    ((CardView)ItemView).SetContentPadding(0, 0, 0, 0);
                }
                _textViewDay.Text = schedule.Day;
                _listViewLessons.SetAdapter(new ClassDayScheduleAdapter(ItemView.Context, schedule.Lessons));
            }
            public ClassScheduleViewHolder(View view) : base(view)
            {
                _textViewDay = view.FindViewById<TextView>(Resource.Id.text_day_of_week);
                _listViewLessons = view.FindViewById<RecyclerView>(Resource.Id.class_schedule_list_view);
                var manager = new LinearLayoutManager(ItemView.Context);
                manager.MeasurementCacheEnabled = false;
                _listViewLessons.SetLayoutManager(manager);
                _listViewLessons.AddItemDecoration(new DividerItemDecoration(ItemView.Context, manager.Orientation));
            }
        }
    }
}