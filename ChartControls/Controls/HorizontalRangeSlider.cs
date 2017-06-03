using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ChartControls.CommonModels;

namespace ChartControls.Controls
{
    internal class HorizontalRangeSlider : BaseChartControl
    {
        private readonly RangeValuesFormatter _valuesFormatter;
        private const int MinSliderWidth = 4;
        private bool _isSliderGrabbed;
        private Dock _grabType;
        private Point _mouseDownPoint;
        private readonly GeometryDrawing _sliderDrawing;
        private readonly GeometryDrawing _pointsDrawing;


        public bool IsMouseOverSlider => _sliderDrawing.Geometry?.FillContains(Mouse.GetPosition(this)) ?? false;


        #region Dependecy Properties

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
            {
                // set drag
                _isSliderGrabbed = true;
                _grabType = curType;
                _mouseDownPoint = Mouse.GetPosition(this);
            }

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
                {
                    // reset draging
                    _isSliderGrabbed = false;
                    _grabType = Dock.Top;
                }
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
            foreach (var point in range)
            {
                double x = ConvertToX(point.Key);
                LineGeometry line = new LineGeometry(new Point(x, 0), new Point(x, this.Height));
                var text = RangeValuesFormatter.GetFormattedText(point.Value);
                var textGeo = text.BuildGeometry(new Point(x + _pointsDrawing.Pen.Thickness, (this.Height - text.Height) / 2));

                geometry.Children.Add(line);
                geometry.Children.Add(textGeo);
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
                From = newLeft;
                To = newRight;
            }
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
        }

        private static void ToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            slider.UpdateSlider();
        }
    }
}