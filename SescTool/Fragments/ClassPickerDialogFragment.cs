using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace SescTool.Fragments
{
    public class ClassPickerDialogFragment : AppCompatDialogFragment, View.IOnClickListener,
        NumberPicker.IOnValueChangeListener
    {
        private NumberPicker _classPicker;
        private NumberPicker _literPicker;
        private Button _cancelButton;
        private Button _selectButton;
        private string _selectedClass;
        private string _selectedLiter;
        private readonly IOnClassChooseListener _listener;

        public ClassPickerDialogFragment(Dictionary<string, List<string>> data, IOnClassChooseListener listener)
        {
            _data = data;
            _listener = listener;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.class_picker_fragment, null);
            _cancelButton = view.FindViewById<Button>(Resource.Id.classPicker_cancel_button);
            _selectButton = view.FindViewById<Button>(Resource.Id.classPicker_ok_button);
            _classPicker = view.FindViewById<NumberPicker>(Resource.Id.classPicker_class_picker);
            _literPicker = view.FindViewById<NumberPicker>(Resource.Id.class_Picker_liter_picker);

            _classPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            _literPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            _classPicker.SetOnValueChangedListener(this);
            _literPicker.SetOnValueChangedListener(this);
            _cancelButton.SetOnClickListener(this);
            _selectButton.SetOnClickListener(this);

            InvalidateData();
            return view;
        }
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.classPicker_ok_button:
                    _listener?.OnClassChoose(_selectedClass + _selectedLiter);
                    Dismiss();
                    break;
                case Resource.Id.classPicker_cancel_button:
                    Dismiss();
                    break;
            }
            
        }

        public interface IOnClassChooseListener
        {
            void OnClassChoose(string @class);
        }

        private readonly Dictionary<string, List<string>> _data;

        private void InvalidateData()
        {
            _classPicker.SetDisplayedValues(null);
            _classPicker.MinValue = 0;
            _classPicker.MaxValue = _data.Count - 1;
            _classPicker.SetDisplayedValues(_data.Keys.ToArray());
            _classPicker.Value = 0;
            InvalidateLiters(0);
        }
        private void InvalidateLiters(int currentPosition)
        {
            _literPicker.SetDisplayedValues(null);
            _literPicker.MinValue = 0;
            _literPicker.MaxValue = _data.ElementAt(currentPosition).Value.Count - 1;
            _literPicker.SetDisplayedValues(_data.ElementAt(currentPosition).Value.ToArray());
            _literPicker.Value = 0;
            _selectedClass = _data.ElementAt(currentPosition).Key;
            _selectedLiter = _data.ElementAt(currentPosition).Value[0];
        }
        public void OnValueChange(NumberPicker picker, int oldVal, int newVal)
        {
            switch (picker.Id)
            {
                case Resource.Id.classPicker_class_picker:
                    InvalidateLiters(newVal);
                    break;
                case Resource.Id.class_Picker_liter_picker:
                    _selectedLiter = _data[_selectedClass][newVal];
                    break;
            }
        }
    }
}