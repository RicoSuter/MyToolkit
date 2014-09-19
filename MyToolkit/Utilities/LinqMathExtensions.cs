//-----------------------------------------------------------------------
// <copyright file="LinqMathExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToolkit.Utilities
{
    /// <summary>Provides LINQ extension methods for mathematical calculations. </summary>
    public static class LinqMathExtensions
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
                double avg = valueList.Average();
                double sum = valueList.Sum(d => (d - avg) * (d - avg));
                return Math.Sqrt(sum / count);
            }

            return 0;
        }

        /// <summary>Calculates the median from the given values. </summary>
        /// <param name="values">The values. </param>
        /// <returns>The median. </returns>
        public static double Median(this IEnumerable<double> values)
        {
            IOrderedEnumerable<double> sortedList = values.OrderBy(n => n);
            int count = sortedList.Count();
            int itemIndex = count / 2;

            if (count % 2 == 0)
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;
            return sortedList.ElementAt(itemIndex);
        }
    }
}
