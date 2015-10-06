//-----------------------------------------------------------------------
// <copyright file="LinqMathExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToolkit.Utilities
{
    /// <summary>Provides LINQ extension methods for mathematical calculations. </summary>
    public static class EnumerableMathExtensions
    {
        /// <summary>Calculates the standard deviation from the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The standard deviation. </returns>
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            var valueList = values.ToList();
            int count = valueList.Count;
            if (count > 1)
            {
                var avg = valueList.Average();
                var sum = valueList.Sum(d => (d - avg) * (d - avg));
                return Math.Sqrt(sum / count);
            }

            return 0;
        }

        /// <summary>Calculates the standard deviation from the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The standard deviation. </returns>
        public static double StandardDeviation(this IEnumerable<decimal> values)
        {
            var valueList = values.ToList();
            int count = valueList.Count;
            if (count > 1)
            {
                var avg = valueList.Average();
                var sum = valueList.Sum(d => (d - avg) * (d - avg));
                return Math.Sqrt((double)(sum / count));
            }

            return 0;
        }

        /// <summary>Calculates the statistical range of the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The range. </returns>
        public static double Range(this IEnumerable<double> values)
        {
            return values.Max() - values.Min();
        }

        /// <summary>Calculates the statistical range of the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The range. </returns>
        public static decimal Range(this IEnumerable<decimal> values)
        {
            return values.Max() - values.Min();
        }

        /// <summary>Calculates the statistical variance of the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The variance. </returns>
        public static double Variance(this IEnumerable<double> values)
        {
            double mean = 0;
            double sum = 0;
            int i = 0;

            foreach (var value in values)
            {
                i++;

                var delta = value - mean;

                mean = mean + delta / i;
                sum += delta * (value - mean);
            }

            return sum / (i - 1);
        }

        /// <summary>Calculates the statistical variance of the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The variance. </returns>
        public static decimal Variance(this IEnumerable<decimal> values)
        {
            decimal mean = 0;
            decimal sum = 0;
            int i = 0;

            foreach (var value in values)
            {
                i++;

                var delta = value - mean;

                mean = mean + delta / i;
                sum += delta * (value - mean);
            }

            return sum / (i - 1);
        }

        /// <summary>Calculates the median from the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The median. </returns>
        public static double Median(this IEnumerable<double> values)
        {
            var sortedList = values.OrderBy(n => n);
            var count = sortedList.Count();
            var itemIndex = count / 2;

            if (count % 2 == 0)
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;
            return sortedList.ElementAt(itemIndex);
        }

        /// <summary>Calculates the median from the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The median. </returns>
        public static decimal Median(this IEnumerable<decimal> values)
        {
            var sortedList = values.OrderBy(n => n);
            var count = sortedList.Count();
            var itemIndex = count / 2;

            if (count % 2 == 0)
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;
            return sortedList.ElementAt(itemIndex);
        }
    }
}
