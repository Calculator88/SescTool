using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace SescTool.Fragments
{
    public class TeacherScheduleFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = (LinearLayout)inflater.Inflate(Resource.Layout.teacher_schedule_fragment, container, false);
            return v;
        }
    }
}