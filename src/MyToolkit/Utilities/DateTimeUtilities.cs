//-----------------------------------------------------------------------
// <copyright file="DateTimeUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Utilities
{
    /// <summary>Provides date time utility methods. </summary>
    public static class DateTimeUtilities
    {
        /// <summary>Converts a unix timestamp to a DateTime. </summary>
        /// <param name="unixTimeStamp">The unix timestamp. </param>
        /// <param name="kind">The kind of the unit timestamp and return value. </param>
        /// <returns>The date time. </returns>
        public static DateTime FromUnixTimeStamp(double unixTimeStamp, DateTimeKind kind = DateTimeKind.Local)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, kind);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        /// <summary>Converts a DateTime to an unix timestamp. </summary>
        /// <param name="dateTime">The date time. </param>
        /// <param name="kind">The kind of the date time and return value. </param>
        /// <returns>The unix timestamp. </returns>
        public static double ToUnixTimeStamp(this DateTime dateTime, DateTimeKind kind = DateTimeKind.Local)
        {
            dateTime = kind == DateTimeKind.Local ? dateTime.ToLocalTime() : dateTime.ToUniversalTime();
            return (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, kind)).TotalSeconds;
        }

        /// <summary>Changes only the time part of the DateTime. </summary>
        /// <param name="date">The date. </param>
        /// <param name="hour">The hour. </param>
        /// <param name="minute">The minute. </param>
        /// <param name="second">The second. </param>
        /// <returns></returns>
        public static DateTime SetTimeTakeDate(this DateTime date, int hour, int minute, int second)
        {
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
        }
    }
}
