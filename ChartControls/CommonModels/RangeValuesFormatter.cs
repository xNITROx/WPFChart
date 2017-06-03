using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace ChartControls.CommonModels
{
    internal class RangeValuesFormatter
    {
        private readonly int _maxValsCount;


        public RangeValuesFormatter(int maxValuesCount = 10)
        {
            _maxValsCount = maxValuesCount;
        }


        public Dictionary<double, string> GetNumberRange(double min, double max)
        {
            Dictionary<double, string> values = new Dictionary<double, string>();

            double start;
            double step = FindStep(min, max, out start);

            double currValue = start;
            while (currValue <= max)
            {
                if (currValue >= min)
                    values[currValue] = currValue.ToString();
                currValue += step;
            }

            return values;
        }

        private double FindStep(double min, double max, out double start)
        {
            if (min > max) // TODO: if min > max
            {
                start = 0;
                return 0;
            }

            double diff = max - min;
            double step = 0.000000000000001;
            while (step < diff)
                step *= 10;
            step = Math.Round(step, 15);
            while (diff < step)
                step /= 2;
            step = step * 2 / _maxValsCount;
            start = Math.Round(min / step) * step;

            return Math.Round(step, 2);
        }

        public static FormattedText GetFormattedText(string candidate)
        {
            var fontFamily = Application.Current.MainWindow.FontFamily;
            var fontStyle = Application.Current.MainWindow.FontStyle;
            var fontWeight = Application.Current.MainWindow.FontWeight;
            var fontStretch = Application.Current.MainWindow.FontStretch;
            var fontSize = Application.Current.MainWindow.FontSize;
            var foreground = Application.Current.MainWindow.Foreground;

            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch), fontSize, foreground);

            return formattedText;
        }

        public static Size MeasureString(string candidate)
        {
            var formattedText = GetFormattedText(candidate);
            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}