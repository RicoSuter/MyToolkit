//-----------------------------------------------------------------------
// <copyright file="GeolocationUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Utilities
{
    /// <summary>Provides methods to handle geo locations. </summary>
    public static class GeolocationUtilities
    {
        /// <summary>Calculates the distance between to coordinates. </summary>
        /// <param name="lat1">Latitude of the first coordinate. </param>
        /// <param name="long1">Longitude of the first coordinate. </param>
        /// <param name="lat2">Latitude of the second coordinate. </param>
        /// <param name="long2">Longitude of the second coordinate. </param>
        /// <returns>The distance. </returns>
        /// <exception cref="ArgumentException">Latitude or longitude is not a number.</exception>
        public static double GetDistanceTo(double lat1, double long1, double lat2, double long2)
        {
            if (double.IsNaN(lat1) || double.IsNaN(long1) || double.IsNaN(lat2) || double.IsNaN(long2))
                throw new ArgumentException("Latitude or longitude is not a number.");

            double latitude = lat1 * 0.0174532925199433;
            double longitude = long1 * 0.0174532925199433;
            double num = lat2 * 0.0174532925199433;
            double longitude1 = long2 * 0.0174532925199433;
            double num1 = longitude1 - longitude;
            double num2 = num - latitude;
            double num3 = Math.Pow(Math.Sin(num2 / 2), 2) + Math.Cos(latitude) * Math.Cos(num) * Math.Pow(Math.Sin(num1 / 2), 2);
            double num4 = 2 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1 - num3));
            double num5 = 6376500 * num4;
            return num5;
        }
    }
}
