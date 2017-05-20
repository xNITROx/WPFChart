using System;
using System.Globalization;
using System.Windows;

namespace ChartControls.CommonModels
{
    internal static class Extentions
    {
        public static readonly IFormatProvider DefaultFormat = new CultureInfo("EN");

        public static string GetString(this Point point)
        {
            return point.X.ToString(DefaultFormat) + "," + point.Y.ToString(DefaultFormat);
        }
    }
}