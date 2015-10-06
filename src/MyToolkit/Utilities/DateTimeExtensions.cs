//-----------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for date and time manipulation. </summary>
    public static class DateTimeExtensions
    {
        /// <summary>Resets the time part to 00:00:00. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <returns>The new date time. </returns>
        public static DateTime ToStartOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }

        /// <summary>Sets the time part to the latest time of the day. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <returns>The new date time. </returns>
        public static DateTime ToEndOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
        }

        /// <summary>Resets the time part to 00:00:00. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <returns>The new date time. </returns>
        public static DateTime? ToStartOfDay(this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToStartOfDay() : (DateTime?)null;
        }

        /// <summary>Sets the time part to the latest time of the day. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <returns>The new date time. </returns>
        public static DateTime? ToEndOfDay(this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToEndOfDay() : (DateTime?)null;
        }

        /// <summary>Checks whether a date time is between two date times. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <param name="start">The starting date time. </param>
        /// <param name="end">The ending start time. </param>
        /// <returns>True when the date time is between. </returns>
        public static bool IsBetween(this DateTime dt, DateTime start, DateTime end)
        {
            return start <= dt && dt < end;
        }

        /// <summary>Checks whether a date time is between two date times. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <param name="start">The starting date time. </param>
        /// <param name="end">The ending start time. </param>
        /// <returns>True when the date time is between. </returns>
        public static bool IsBetween(this DateTime? dt, DateTime start, DateTime end)
        {
            return dt.HasValue && dt.Value.IsBetween(start, end);
        }

        /// <summary>Checks whether a date time is between two date times. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <param name="start">The starting date time. </param>
        /// <param name="end">The ending start time. Null means undefinitely in the future. </param>
        /// <returns>True when the date time is between. </returns>
        public static bool IsBetween(this DateTime dt, DateTime start, DateTime? end)
        {
            return start <= dt && (end == null || dt < end.Value);
        }

        /// <summary>Checks whether a date time is between two date times. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <param name="start">The starting date time. </param>
        /// <param name="end">The ending start time. Null means undefinitely in the future. </param>
        /// <returns>True when the date time is between. </returns>
        public static bool IsBetween(this DateTime? dt, DateTime start, DateTime? end)
        {
            return dt.HasValue && dt.Value.IsBetween(start, end);
        }

        /// <summary>
        /// Checks whether a date time is between two date times. 
        /// </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <param name="start">The starting date time. Null means undefinitely in the past. </param>
        /// <param name="end">The ending start time. Null means undefinitely in the future. </param>
        /// <returns>True when the date time is between. </returns>
        public static bool IsBetween(this DateTime dt, DateTime? start, DateTime? end)
        {
            return (start == null || start.Value <= dt) && (end == null || dt < end.Value);
        }

        /// <summary>Checks whether a date time is between two date times. </summary>
        /// <param name="dt">The date time to work with. </param>
        /// <param name="start">The starting date time. Null means undefinitely in the past. </param>
        /// <param name="end">The ending start time. Null means undefinitely in the future. </param>
        /// <returns>True when the date time is between. </returns>
        public static bool IsBetween(this DateTime? dt, DateTime? start, DateTime? end)
        {
            return dt.HasValue && dt.Value.IsBetween(start, end);
        }
    }
}
