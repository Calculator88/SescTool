using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SescTool.Views
{
    public class ClassPicker : FrameLayout, View.IOnClickListener
    {
        private List<Point> _classesCirclesCenters;
        private List<Point> _litersCircleCenteres;
        private int _classesCount;
        private int _litersCount;

        private int _currentClassIndex;
        private int _currentLitterIndex;
        private bool _showModClass;
        private bool _layouted;

        private TextView _classTitleTextView;
        private TextView _classLiterTextView;
        private RelativeLayout _circleClasses;
        private RelativeLayout _circleLiters;

        private const double CircleAngle = 2 * Math.PI;
        private const double AngleOrigin = Math.PI / 2;
        private const double AngleStepMultiplicator = -1;

        private IDictionary<string, List<string>> _data;
        public ClassPicker(Context context) : base(context)
        {
            LoadView();
        }                                                                                      

        public ClassPicker(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            LoadView();
        }
                                                                                                                                     
        public ClassPicker(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            LoadView();
        }
                                                                                                                                     
        public ClassPicker(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            LoadView();
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            if (_layouted) return;
            _layouted = true;
            InvalidateData();
        }

        private void LoadView()
        {
            var view = LayoutInflater.From(Context).Inflate(Resource.Layout.classPicker, null);
            _classLiterTextView = view.FindViewById<TextView>(Resource.Id.class_liter);
            _classTitleTextView = view.FindViewById<TextView>(Resource.Id.class_title);
            _circleClasses = view.FindViewById<RelativeLayout>(Resource.Id.class_circle_classes);
            _circleLiters = view.FindViewById<RelativeLayout>(Resource.Id.class_circle_liters);

            _classLiterTextView.SetOnClickListener(this);
            _classTitleTextView.SetOnClickListener(this);

            AddView(view);
        }

        private void InvalidateData()
        {
            if (!_layouted) return;
            CalculateClassAngles(Data.Count);
            DrawClassCircles();
            _showModClass = false;
        }

        private void CalculateClassAngles(int count)
        {
            if (_classesCount == count) return;

            _classesCount = count;

            var radius = Resources.GetDimension(Resource.Dimension.classPicker_circle_in_diamener) / 2;
            _classesCirclesCenters = new List<Point>();    

            var currentAngle = AngleOrigin;
            var stepAngle = CircleAngle / count;
            var verticalOffset = _circleClasses.Height / 2;
            var horizontalOffset = _circleClasses.Width / 2;

            for (var i = 0; i < count; i++)
            {          
                var x = (int)(Math.Sin(currentAngle) * radius);
                var y = (int)(Math.Cos(currentAngle) * radius);
                _classesCirclesCenters.Add(new Point(x + horizontalOffset, y + verticalOffset));
                currentAngle += stepAngle * AngleStepMultiplicator;
            }
        }

        private void CalculateLiterAngles(int count)
        {
            if (_litersCount == count) return;

            _litersCount = count;

            var radius = Resources.GetDimension(Resource.Dimension.classPicker_circle_in_diamener) / 2;
            _litersCircleCenteres = new List<Point>();

            var currentAngle = AngleOrigin;
            var stepAngle = CircleAngle / count;
            var verticalOffset = _circleLiters.Height / 2;
            var horizontalOffset = _circleLiters.Width / 2;

            for (var i = 0; i < count; i++)
            {
                var x = (int)(Math.Sin(currentAngle) * radius);
                var y = (int)(Math.Cos(currentAngle) * radius);
                _litersCircleCenteres.Add(new Point(x + horizontalOffset, y + verticalOffset));
                currentAngle += stepAngle * AngleStepMultiplicator;
            }
        }

        private void DrawClassCircles()
        {
            _circleClasses.RemoveAllViews();

            for (var index = 0; index < _classesCirclesCenters.Count; index++)
            {
                var center = _classesCirclesCenters[index];
                var itemView = LayoutInflater.From(Context).Inflate(Resource.Layout.class_circle_item, null);
                itemView.SetOnClickListener(this);
                itemView.FindViewById<TextView>(Resource.Id.class_item_text).Text = Data.ElementAt(index).Key;
                itemView.Id = index;
                var coordX = center.X - itemView.Height / 2;
                var coordY = center.Y - itemView.Width / 2;
                var @params = new RelativeLayout.LayoutParams
                    (ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    LeftMargin = coordY,
                    TopMargin = coordX
                };
                _circleClasses.AddView(itemView, @params);
            }
        }

        private void DrawLitersCircles(int classIndex)
        {
            _circleLiters.RemoveAllViews();

            for (var index = 0; index < _litersCircleCenteres.Count; index++)
            {
                var center = _litersCircleCenteres[index];
                var itemView = LayoutInflater.From(Context).Inflate(Resource.Layout.class_circle_item, null);
                itemView.SetOnClickListener(this);
                itemView.FindViewById<TextView>(Resource.Id.class_item_text).Text = Data.ElementAt(classIndex).Value.ElementAt(index);
                itemView.Id = index;
                var coordX = center.X - itemView.Height / 2;
                var coordY = center.Y - itemView.Width / 2;
                var @params = new RelativeLayout.LayoutParams
                (ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    LeftMargin = coordY,
                    TopMargin = coordX
                };
                _circleLiters.AddView(itemView, @params);
            }
        }

        public IDictionary<string, List<string>> Data
        {
            get => _data;
            set
            {
                if (_data == value) return;
                _data = value;
                InvalidateData();
            }
        }

        public void OnClick(View v)
        {
            if (_showModClass)
            {
                _currentClassIndex = v.Id;
                CalculateLiterAngles(Data.ElementAt(_currentClassIndex).Value.Count);
                DrawLitersCircles(_currentClassIndex);
                _circleClasses.Visibility = ViewStates.Gone;
                _circleLiters.Visibility = ViewStates.Visible;
            }
            else
            {
                _currentLitterIndex = v.Id;
            }
        }
    }
}