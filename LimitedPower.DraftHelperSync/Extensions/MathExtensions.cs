using System;
using System.Globalization;

namespace LimitedPower.DraftHelperSync.Extensions
{
    public static class MathExtensions
    {
        public static int TransformRating(this double value, double oldMin, double oldMax) =>
            (int) Math.Round(10 - (value - oldMin) * (10 / (oldMax - oldMin)), 0, MidpointRounding.AwayFromZero);

        public static string ToStringValue(this double value, int places) => Math.Round(value * 100, places).ToString(CultureInfo.InvariantCulture);
        
    }
}
