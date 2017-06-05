using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ChartControls.CommonModels;
using ChartControls.CommonModels.DataModels;

namespace ChartControls.Controls
{
    internal class HorizontalRangeSlider : BaseChartControl
    {
        private readonly RangeValuesFormatter _valuesFormatter;
        private const int MinSliderWidth = 4;
        private bool _isSliderGrabbed;
        private Dock _grabType;
        private Point _mouseDownPoint;
        private double _stuckDiff;
        private bool _saveStuckProportion = true;
        private readonly GeometryDrawing _sliderDrawing;
        private readonly GeometryDrawing _pointsDrawing;


        public event EventHandler<Scope> ViewScopeChanged;
        public bool IsMouseOverSlider => _sliderDrawing.Geometry?.FillContains(Mouse.GetPosition(this)) ?? false;


        #region Dependecy Properties

        public static readonly DependencyProperty IsStuckToLeftProperty = DependencyProperty.Register(
            "IsStuckToLeft", typeof(bool), typeof(HorizontalRangeSlider), new PropertyMetadata(default(bool), IsStuckToLeftChanged));
        public bool IsStuckToLeft
        {
            get { return (bool)GetValue(IsStuckToLeftProperty); }
            set { SetValue(IsStuckToLeftProperty, value); }
        }

        public static readonly DependencyProperty IsStuckToRightProperty = DependencyProperty.Register(
            "IsStuckToRight", typeof(bool), typeof(HorizontalRangeSlider), new PropertyMetadata(default(bool), IsStuckToRightChanged));
        public bool IsStuckToRight
        {
            get { return (bool)GetValue(IsStuckToRightProperty); }
            set { SetValue(IsStuckToRightProperty, value); }
        }

        public static readonly DependencyProperty SliderFillProperty = DependencyProperty.Register(
            "SliderFill", typeof(Brush), typeof(HorizontalRangeSlider), new PropertyMetadata(default(Brush), SliderFillPropertyChanged));
        public Brush SliderFill
        {
            get { return (Brush)GetValue(SliderFillProperty); }
            set { SetValue(SliderFillProperty, value); }
        }

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
            "Min", typeof(double), typeof(HorizontalRangeSlider), new PropertyMetadata(default(double), MinPropertyChanged));
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
            "Max", typeof(double), typeof(HorizontalRangeSlider), new PropertyMetadata(default(double), MaxPropertyChanged));
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
            "From", typeof(double), typeof(HorizontalRangeSlider), new PropertyMetadata(default(double), FromPropertyChanged));

        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            "To", typeof(double), typeof(HorizontalRangeSlider), new PropertyMetadata(default(double), ToPropertyChanged));

        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        #endregion


        public HorizontalRangeSlider()
        {
            _valuesFormatter = new RangeValuesFormatter();
            _sliderDrawing = new GeometryDrawing(this.SliderFill, new Pen(), Geometry.Empty);
            _pointsDrawing = new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.LightSlateGray, 1), Geometry.Empty);
            EventManager.RegisterClassHandler(typeof(Window), Window.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
        }


        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var curType = GetCursorPositionOnSlider();
            if (curType != Dock.Top)
                StartDrag(curType);

            base.OnPreviewMouseLeftButtonDown(e);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var curType = GetCursorPositionOnSlider();
            SetCursor(_isSliderGrabbed ? _grabType : curType);

            if (_isSliderGrabbed)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragSlider();
                else
                    StopDrag();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(this.Background, new Pen(), new Rect(this.RenderSize));

            UpdatePointsGeometry();
            UpdateSliderGeometry();

            drawingContext.DrawDrawing(_pointsDrawing);
            drawingContext.DrawDrawing(_sliderDrawing);
            base.OnRender(drawingContext);
        }

        private void UpdateSliderGeometry()
        {
            double x1, x2;
            GetValidSliderBounds(out x1, out x2);
            RectangleGeometry sliderRec = new RectangleGeometry(new Rect(new Point(x1, 0), new Point(x2, this.Height)));
            _sliderDrawing.Geometry = sliderRec;
        }

        private void UpdatePointsGeometry()
        {
            GeometryGroup geometry = new GeometryGroup();
            var range = _valuesFormatter.GetNumberRange(Min, Max);
            if (range.Count > 0)
            {
                geometry.Children.Add(new LineGeometry(new Point(), new Point(this.RenderSize.Width, 0)));
                foreach (var point in range)
                {
                    double x = ConvertToX(point.Key);
                    double y = this.RenderSize.Height / 4;
                    LineGeometry line = new LineGeometry(new Point(x, 0), new Point(x, y));
                    var text = RangeValuesFormatter.GetFormattedText(point.Value);
                    var textGeo = text.BuildGeometry(new Point(x - text.Width / 2, y));

                    geometry.Children.Add(line);
                    geometry.Children.Add(textGeo);
                }
            }
            _pointsDrawing.Geometry = geometry;
        }

        private void SetCursor(Dock type = Dock.Top)
        {
            switch (type)
            {
                case Dock.Bottom:
                    Cursor = Cursors.Hand;
                    break;
                case Dock.Left:
                    Cursor = Cursors.SizeWE;
                    break;
                case Dock.Right:
                    Cursor = Cursors.SizeWE;
                    break;
                default:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        private Dock GetCursorPositionOnSlider()
        {
            if (IsMouseOverSlider && _sliderDrawing.Geometry is RectangleGeometry sliderRec)
            {
                Point pos = Mouse.GetPosition(this);

                if (Math.Abs(pos.X - sliderRec.Bounds.BottomLeft.X) < MinSliderWidth)
                    return Dock.Left;
                if (Math.Abs(pos.X - sliderRec.Bounds.BottomRight.X) < MinSliderWidth)
                    return Dock.Right;

                return Dock.Bottom;
            }
            return Dock.Top;
        }

        private void StartDrag(Dock grabType)
        {
            // set drag
            _isSliderGrabbed = true;
            _grabType = grabType;
            _mouseDownPoint = Mouse.GetPosition(this);
        }

        private void DragSlider()
        {
            Point point = Mouse.GetPosition((IInputElement)this.Parent ?? this);
            double mouseData = ConvertToData(point.X);
            if (mouseData > Max)
                mouseData = Max;
            else if (mouseData < Min)
                mouseData = Min;
            double newLeft = From;
            double newRight = To;
            bool accept = false;
            Dock type = _grabType;
            switch (type)
            {
                case Dock.Bottom:
                    {
                        double diff = _mouseDownPoint.X - point.X;
                        newLeft = ConvertToData(ConvertToX(From) - diff);
                        newRight = ConvertToData(ConvertToX(To) - diff);

                        if (newLeft >= Min && newRight <= Max)
                            accept = true;
                    }
                    break;
                case Dock.Left:
                    if (Math.Abs(From - mouseData) > double.Epsilon)
                    {
                        newLeft = mouseData;
                        accept = true;
                    }
                    break;
                case Dock.Right:
                    if (Math.Abs(To - mouseData) > double.Epsilon)
                    {
                        newRight = mouseData;
                        accept = true;
                    }
                    break;
            }

            _mouseDownPoint = point;
            if (!accept) return;

            if (newLeft > newRight) // change grabbed side if cross
            {
                From = newRight;
                To = newLeft;
                _grabType = type == Dock.Left ? Dock.Right : Dock.Left;
            }
            else
            {
                if (Math.Abs(From - newLeft) > 0)
                    From = newLeft;
                if (Math.Abs(To - newRight) > 0)
                    To = newRight;
            }

            // update proportions of sides
            if (_saveStuckProportion) _stuckDiff = Math.Abs(From - To);
        }

        private void StopDrag()
        {
            _isSliderGrabbed = false;
            _grabType = Dock.Top;
        }

        private void UpdateSlider()
        {
            this.InvalidateVisual();
        }

        private void GetValidSliderBounds(out double x1, out double x2)
        {
            if (From > To)
            {
                var temp = From;
                From = To;
                To = temp;
            }

            // check to reset stuck binding
            if (Math.Abs(To - Max) > 0 && IsStuckToRight)
                IsStuckToRight = false;
            if (Math.Abs(From - Min) > 0 && IsStuckToLeft)
                IsStuckToLeft = false;

            // save proportion of sides
            if (_saveStuckProportion && !_isSliderGrabbed)
            {
                if (IsStuckToRight && !IsStuckToLeft)
                    From = To - _stuckDiff;
                if (IsStuckToLeft && !IsStuckToRight)
                    To = From + _stuckDiff;
            }

            // stuck if dragging slider near to side
            if (_isSliderGrabbed)
            {
                if (!IsStuckToRight)
                {
                    double diff = Math.Abs(Max - To);
                    IsStuckToRight = diff <= 1;
                }
                if (!IsStuckToLeft)
                {
                    double diff = Math.Abs(From - Min);
                    IsStuckToLeft = diff <= 1;
                }
            }

            x1 = From > Min ? From : Min;
            x2 = To < Max ? To : Max;
            x1 = ConvertToX(x1);
            x2 = ConvertToX(x2);

            // correct slider size to MinWidth
            if (Math.Abs(x1 - x2) < MinSliderWidth)
            {
                if (Math.Abs(x1 - ConvertToX(Min)) < MinSliderWidth)
                {
                    x1 = ConvertToX(Min);
                    x2 = x1 + MinSliderWidth;
                }
                else if (Math.Abs(x2 - ConvertToX(Max)) < MinSliderWidth)
                {
                    x2 = ConvertToX(Max);
                    x1 = x2 - MinSliderWidth;
                }
                else
                {
                    x1 -= MinSliderWidth / 2.0;
                    x2 += MinSliderWidth / 2.0;
                }
            }
        }

        private double ConvertToX(double value)
        {
            double x = 0;
            if (Min < Max)
                x = (value - Min) / ((Max - Min) / this.RenderSize.Width);
            return x;
        }

        private double ConvertToData(double x)
        {
            double value = 0;
            if (Min < Max)
                value = x * ((Max - Min) / this.RenderSize.Width) + Min;
            return value;
        }

        protected virtual void OnViewScopeChanged()
        {
            ViewScopeChanged?.Invoke(this, new Scope(From, To, double.NaN, double.NaN));
        }

        #region Dependency properties callbacks

        private static void SliderFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            slider._sliderDrawing.Brush = (Brush)e.NewValue;
        }

        private static void MinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            slider.UpdateSlider();
        }

        private static void MaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            slider.UpdateSlider();
        }

        private static void FromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            slider.UpdateSlider();
            slider.OnViewScopeChanged();
        }

        private static void ToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            slider.UpdateSlider();
            slider.OnViewScopeChanged();
        }

        private static void IsStuckToLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            if ((bool)e.NewValue)
            {
                Binding binding = new Binding
                {
                    Path = new PropertyPath(MinProperty.Name),
                    Source = slider
                };
                slider.SetBinding(FromProperty, binding);
                // save proportion
                if (slider._saveStuckProportion)
                    slider._stuckDiff = Math.Abs(slider.From - slider.To);
            }
            else // reset binding
                slider.From = slider.From;
        }

        private static void IsStuckToRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            if ((bool)e.NewValue)
            {
                Binding binding = new Binding
                {
                    Path = new PropertyPath(MaxProperty.Name),
                    Source = slider
                };
                slider.SetBinding(ToProperty, binding);
                // save proportion
                if (slider._saveStuckProportion)
                    slider._stuckDiff = Math.Abs(slider.From - slider.To);
            }
            else // reset binding
                slider.To = slider.To;
        }

        #endregion
    }
}