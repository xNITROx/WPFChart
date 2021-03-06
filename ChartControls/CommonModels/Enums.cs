﻿using System;

namespace ChartControls.CommonModels
{
    public enum Period : long
    {
        Tick = 1,
        Millisecond = 10_000,
        Second = 10_000_000,
        Minute = 600_000_000,
        Hour = 36_000_000_000,
        Day = 864_000_000_000,
        Week = 6_048_000_000_000,
        Month = 25_920_000_000_000,
        Year = 315_360_000_000_000
    }

    public static class PeriodExtentions
    {
        public static DateTime Round(this Period period, long ticks, int count = 1)
        {
            double divider = (long)period * count;
            long perTicks = (long)(Math.Floor(ticks / divider) * divider);
            return new DateTime(perTicks);
        }
    }
}