using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SescTool.Model.ClassroomSchedule;

namespace SescTool.Helpers
{
    public class ClassroomLessonAdapter : RecyclerView.Adapter
    {
        public List<Lesson> Data { get; }
        private readonly Context _context;
        public ClassroomLessonAdapter(List<Lesson> lesson, Context context)
        {
            Data = lesson;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((ClassroomLessonViewHolder)holder).BindLesson(Data[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(_context).Inflate(Resource.Layout.classroom_lesson, parent, false);
            return new ClassroomLessonViewHolder(view);
        }

        public override int ItemCount => Data.Count;

        public class ClassroomLessonViewHolder : RecyclerView.ViewHolder
        {
            private readonly TextView _classroomTextView;
            private readonly TextView _subjectTextView;
            private readonly TextView _classTextView;

            public void BindLesson(Lesson lesson)
            {
                _classroomTextView.Text = lesson.Classroom;
                _subjectTextView.Text = lesson.Subject;
                _classTextView.Text = lesson.Class.ToUpper();
            }

            public ClassroomLessonViewHolder(View itemView) : base(itemView)
            {
                _classroomTextView = itemView.FindViewById<TextView>(Resource.Id.text_classroom_classroom);
                _subjectTextView = itemView.FindViewById<TextView>(Resource.Id.text_classroom_subject);
                _classTextView = itemView.FindViewById<TextView>(Resource.Id.text_classroom_class);
            }
        }
    }
}