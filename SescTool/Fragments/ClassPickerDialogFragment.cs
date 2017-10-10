using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace SescTool.Fragments
{
    public class ClassPickerDialogFragment : AppCompatDialogFragment
    {
        public ClassPickerDialogFragment()
        {
            
        }
        public ClassPickerDialogFragment(Dictionary<string, List<string>> data, IOnClassChooseListener listener, string startValue = null)
        {
            _data = data;
            _listener = listener;
            _startValue = startValue;
        }
        
        #region Private fields

        private NumberPicker _classPicker;
        private NumberPicker _literPicker;
        private Button _cancelButton;
        private Button _selectButton;
        private string _selectedClass;
        private string _selectedLiter;
        private string _startValue;
        private readonly IOnClassChooseListener _listener;
        private readonly Dictionary<string, List<string>> _data;
        private const string SelectedClassTag = "com.sesc.timetable.ClassPickerFragment.SelectedClass";

        #endregion

        #region Fragment lifecycle

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RetainInstance = true;
            var view = LayoutInflater.Inflate(Resource.Layout.class_picker_fragment, null);
            _cancelButton = view.FindViewById<Button>(Resource.Id.classPicker_cancel_button);
            _selectButton = view.FindViewById<Button>(Resource.Id.classPicker_ok_button);
            _classPicker = view.FindViewById<NumberPicker>(Resource.Id.classPicker_class_picker);
            _literPicker = view.FindViewById<NumberPicker>(Resource.Id.class_Picker_liter_picker);

            _classPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            _literPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            _classPicker.ValueChanged += (sender, args) => { InvalidateLiters(args.NewVal); };
            _literPicker.ValueChanged += (sender, args) => { _selectedLiter = _data[_selectedClass][args.NewVal]; };
            _selectButton.Click += (sender, args) =>
            {
                _listener?.OnClassChoose(_selectedClass + _selectedLiter);
                Dismiss();
            };
            _cancelButton.Click += (sender, args) => { Dismiss(); };

            InvalidateData();

            _startValue = savedInstanceState?.GetString(SelectedClassTag) ?? _startValue;

            if (String.IsNullOrEmpty(_startValue)) return view;

            var regex = new Regex("(\\d+)(\\w+)", RegexOptions.Compiled);
            var match = regex.Match(_startValue);
            var number = match.Groups[1].ToString();
            var liter = match.Groups[2].ToString();
            var numberAt = _data.Keys.ToList().IndexOf(number);
            var literAt = _data[number].IndexOf(liter);
            _classPicker.Value = numberAt;
            InvalidateLiters(numberAt);
            _literPicker.Value = literAt;
            _selectedLiter = liter;

            return view;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            var value = _selectedClass + _selectedLiter;
            outState.PutString(SelectedClassTag, value);
        }

        public override void OnDestroyView()
        {
            if (Dialog != null && RetainInstance)
                Dialog.SetDismissMessage(null);
            base.OnDestroyView();
        }

        #endregion

        #region Private methods

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

        #endregion

        #region Public members

        public interface IOnClassChooseListener
        {
            void OnClassChoose(string @class);
        }

        #endregion

    }
}