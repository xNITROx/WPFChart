using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChartControls.Controls
{
    internal class HorizontalRangeSlider : BaseChartControl
    {
        private bool _isSliderGrabbed;
        private Dock _sliderGrabbType;
        private Point _mouseDownPoint;
        private readonly GeometryDrawing _sliderDrawing;

        public bool IsMouseOverSlider => _sliderDrawing.Geometry.FillContains(Mouse.GetPosition(this));

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
            "From", typeof(double), typeof(HorizontalRangeSlider), new PropertyMetadata(default(double), FromPropertyChanged, FromPropertyCoerce));

        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            "To", typeof(double), typeof(HorizontalRangeSlider), new PropertyMetadata(default(double), ToPropertyChanged, ToPropertyCoerce));

        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        #endregion


        public HorizontalRangeSlider()
        {
            _sliderDrawing = new GeometryDrawing(this.SliderFill, new Pen(), null);
            EventManager.RegisterClassHandler(typeof(Window), Window.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = Mouse.GetPosition((IInputElement)sender);
            var grabbedType = GetCursorPositionOnSlider();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // start dragging slider
                if (!_isSliderGrabbed && IsMouseOverSlider)
                {
                    _isSliderGrabbed = true;
                    _sliderGrabbType = grabbedType;
                    _mouseDownPoint = mousePoint;
                }
            }
            else
            {
                // reset drag slider
                _isSliderGrabbed = false;
                _sliderGrabbType = Dock.Top;
                _mouseDownPoint = new Point();
            }

            if (_isSliderGrabbed)
                DragSlider(mousePoint, _sliderGrabbType);

            SetCursor(_isSliderGrabbed ? _sliderGrabbType : grabbedType);
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(this.Background, new Pen(), new Rect(this.RenderSize));

            UpdateSlider();
            drawingContext.DrawDrawing(_sliderDrawing);

            base.OnRender(drawingContext);
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

                if (Math.Abs(pos.X - sliderRec.Bounds.BottomLeft.X) < 3)
                    return Dock.Left;
                if (Math.Abs(pos.X - sliderRec.Bounds.BottomRight.X) < 3)
                    return Dock.Right;

                return Dock.Bottom;
            }
            return Dock.Top;
        }

        private void DragSlider(Point point, Dock type)
        {
            double diff = _mouseDownPoint.X - point.X;
            switch (type)
            {
                case Dock.Bottom:
                    {
                        double newLeft = ConvertToData(ConvertToX(From) - diff);
                        double newRight = ConvertToData(ConvertToX(To) - diff);

                        if (newLeft >= Min && newRight <= Max)
                        {
                            From = newLeft;
                            To = newRight;
                            _mouseDownPoint = point;
                        }
                    }
                    break;
                case Dock.Left:
                    {
                        double newLeft = ConvertToData(ConvertToX(From) - diff);
                        if (newLeft >= Min)
                        {
                            From = newLeft;
                            _mouseDownPoint = point;
                        }
                    }
                    break;
                case Dock.Right:
                    {
                        double newRight = ConvertToData(ConvertToX(To) - diff);
                        if (newRight <= Max)
                        {
                            To = newRight;
                            _mouseDownPoint = point;
                        }
                    }
                    break;
            }
        }

        private void UpdateSlider()
        {
            if (From > To)
                return;

            double x1 = ConvertToX(From);
            double x2 = ConvertToX(To);
            RectangleGeometry sliderRec = new RectangleGeometry(new Rect(new Point(x1, 0), new Point(x2, this.Height)));
            _sliderDrawing.Geometry = sliderRec;
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
            if ((double)e.NewValue > slider.From)
                slider.From = (double)e.NewValue;

            slider.UpdateSlider();
        }

        private static void MaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            if ((double)e.NewValue < slider.To)
                slider.To = (double)e.NewValue;

            slider.UpdateSlider();
        }

        private static void FromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            slider.UpdateSlider();
        }
        private static object FromPropertyCoerce(DependencyObject d, object basevalue)
        {
            var slider = (HorizontalRangeSlider)d;
            if ((double)basevalue < slider.Min)
                return slider.Min;
            return basevalue;
        }

        private static void ToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (HorizontalRangeSlider)d;
            slider.UpdateSlider();
        }

        private static object ToPropertyCoerce(DependencyObject d, object basevalue)
        {
            var slider = (HorizontalRangeSlider)d;
            if ((double)basevalue > slider.Max)
                return slider.Max;
            return basevalue;
        }
    }
}