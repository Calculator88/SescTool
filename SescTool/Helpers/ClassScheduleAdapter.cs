using Android.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SescTool.Model;

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
            var view = LayoutInflater.From(_context).Inflate(Resource.Layout.class_schedule_card, parent, false);
            return new ClassScheduleViewHolder(view, _context);
        }

        public override int ItemCount => Data.Timetable.Count;

        internal class ClassScheduleViewHolder : RecyclerView.ViewHolder
        {
            private readonly TextView _textViewDay;
            private readonly RecyclerView _listViewLessons;
            private readonly Context _context;
            public void BindScheduleDay(ScheduleDay schedule)
            {
                _textViewDay.Text = schedule.Day;
                _listViewLessons.SetAdapter(new ClassDayScheduleAdapter(_context, schedule.Lessons));
            }
            public ClassScheduleViewHolder(View view, Context context) : base(view)
            {
                _context = context;
                _textViewDay = view.FindViewById<TextView>(Resource.Id.text_day_of_week);
                _listViewLessons = view.FindViewById<RecyclerView>(Resource.Id.class_schedule_list_view);
                var manager = new LinearLayoutManager(context);
                _listViewLessons.SetLayoutManager(manager);
                _listViewLessons.AddItemDecoration(new DividerItemDecoration(context, manager.Orientation));
            }
        }
    }
}