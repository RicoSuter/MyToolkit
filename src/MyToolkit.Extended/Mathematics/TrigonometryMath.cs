//-----------------------------------------------------------------------
// <copyright file="TrigonometryMath.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WP8 || WP7
using System.Windows;
#elif WPF
using System.Windows;
#else
using Windows.Foundation;
#endif

namespace MyToolkit.Mathematics
{
	public static class TrigonometryMath
	{
		public static bool HasHorizontalDirection(this Point point)
		{
			var angle = Direction(point);
			if (angle >= 45 && angle < 135)
				return true;
			if (angle >= 135 && angle < 225)
				return false;
			if (angle >= 225 && angle < 315)
				return true;
			return false; 
		}

		public static double DirectionInXaml(this Point point)
		{
			return Direction(new Point(point.X, -point.Y));
		}

		/// <summary>
		/// Returns the angle of the point relative to the origin (0,0). 
		/// Degree 0 is at the top and is increased clock-wise. 
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public static double Direction(this Point point)
		{
			var angle = 0.0; 

			if (point.X == 0 && point.Y > 0)
				angle = 90;
			else if (point.X == 0 && point.Y < 0)
				angle = -90;

			else if (point.X > 0)
				angle = RadianToDegree(System.Math.Atan(point.Y / point.X));
			else if (point.X < 0 && point.Y >= 0)
				angle = RadianToDegree(System.Math.Atan(point.Y / point.X)) + 180;
			else if (point.X < 0 && point.Y < 0)
				angle = RadianToDegree(System.Math.Atan(point.Y / point.X)) - 180;

			return NormalizeDegrees(360 - (angle - 90));
		}

		public static double RadianToDegree(double angle)
		{
			return angle * (180.0 / System.Math.PI);
		}

		public static double DegreeToRadian(double angle)
		{
			return angle * (System.Math.PI / 180.0);
		}

		public static double NormalizeDegrees(double angle)
		{
			angle = angle%360;
			if (angle < 0)
				return 360 + angle;
			return angle; 
		}
	}
}
