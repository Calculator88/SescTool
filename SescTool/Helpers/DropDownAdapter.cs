using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace SescTool.Helpers
{
    public class DropDownAdapter : ArrayAdapter<string>
    {
        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            var view = base.GetDropDownView(position, convertView, parent);
            view.LayoutParameters.Height = ViewGroup.LayoutParams.WrapContent;
            return view;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = base.GetView(position, convertView, parent);
            ((TextView) view).Gravity = GravityFlags.Right;
            return view;
        }

        public DropDownAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
        }

        public DropDownAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
        }

        public DropDownAdapter(Context context, int textViewResourceId, string[] objects) : base(context, textViewResourceId, objects)
        {
        }

        public DropDownAdapter(Context context, int resource, int textViewResourceId, string[] objects) : base(context, resource, textViewResourceId, objects)
        {
        }

        public DropDownAdapter(Context context, int textViewResourceId, IList<string> objects) : base(context, textViewResourceId, objects)
        {
        }

        public DropDownAdapter(Context context, int resource, int textViewResourceId, IList<string> objects) : base(context, resource, textViewResourceId, objects)
        {
        }
    }
}