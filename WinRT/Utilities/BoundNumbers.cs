using System;

namespace MyToolkit.MachineLearning.WinRT.Utilities
{
    class BoundNumbers
    {
        public const double MinValue = -1.0E20;
        public const double MaxValue = 1.0E20;

        public static double Bound(double d)
        {
            if (d < MinValue)
                return MinValue;
            if (d > MaxValue)
                return MaxValue;
            return d;
        }

        public static double Exp(double d)
        {
            return Bound(Math.Exp(d));
        }
    }
}
