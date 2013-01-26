using System;
using Windows.Foundation;

namespace MyToolkit.Mathematics
{
	public static class VectorMath
	{
		public static Point Normalize(this Point vector)
		{
			var length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			return new Point(vector.X / length, vector.Y / length);
		}

		public static double Length(this Point vector)
		{
			return Math.Sqrt(vector.X*vector.X + vector.Y*vector.Y); 
		}

		public static double Length(Point pt1, Point pt2)
		{
			return Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y));
		}

		public static double LengthSquared(Point pt1, Point pt2)
		{
			return (pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y);
		}

		public static Point MiddlePoint(Point pt1, Point pt2)
		{
			return new Point(
				pt1.X + (pt2.X - pt1.X) / 2, 
				pt1.Y + (pt2.Y - pt1.Y) / 2
			);
		}

		public static bool Instersects(this Rect rect1, Rect rect2)
		{
			// TODO [low] write unit test => implement faster
			rect1.Intersect(rect2);
			return !rect1.IsEmpty;
		}

		public static Point? Intersects(Point a1, Point a2, Point b1, Point b2)
		{
			var b = new Point(a2.X - a1.X, a2.Y - a1.Y);
			var d = new Point(b2.X - b1.X, b2.Y - b1.Y);
			var bDotDPerp = b.X * d.Y - b.Y * d.X;

			if (System.Math.Abs(bDotDPerp - 0) < 0.05)
				return null;

			var c = new Point(b1.X - a1.X, b1.Y - a1.Y);
			var t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
			if (t < 0 || t > 1)
				return null;

			var u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
			if (u < 0 || u > 1)
				return null;

			return new Point(a1.X + t * b.X, a1.Y + t * b.Y);
		}
		
		public static bool Intersects(Point a1, Point a2, Point[] polyline)
		{
			for (var i = 1; i < polyline.Length; i++)
			{
				var pt1 = polyline[i];
				var pt2 = polyline[i - 1];

				if (Intersects(a1, a2, pt1, pt2).HasValue)
					return true;
			}
			return false;
		}

		public static bool Intersects(Rect r, Point[] polyline)
		{
			for (var i = 1; i < polyline.Length; i++)
			{
				var pt1 = polyline[i];
				var pt2 = polyline[i - 1];

				if (Intersects(pt1, pt2, r))
					return true;
			}
			return false;
		}

		public static bool Intersects(Point p1, Point p2, Rect r)
		{
			return Intersects(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)).HasValue ||
				   Intersects(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)).HasValue ||
				   Intersects(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)).HasValue ||
				   Intersects(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)).HasValue ||
				   (r.Contains(p1) && r.Contains(p2));
		}

		public static bool OnLine(this Point position, Point l1, Point l2, double maxDistance)
		{
			var dist = position.DistanceFromPointToLine(l1, l2);
			return dist < maxDistance;
		}

		public static bool OnPolyline(this Point position, Point[] polyline, double maxDistance)
		{
			for (var i = 1; i < polyline.Length; i++)
			{
				var pt1 = polyline[i];
				var pt2 = polyline[i - 1];

				if (position.OnLine(pt1, pt2, maxDistance))
					return true;
			}
			return false;
		}

		private static double DistanceFromPointToLine(this Point point, Point l1, Point l2)
		{
			double nearX = 0; double nearY = 0;
			return DistanceFromPointToLine(point, l1, l2, out nearX, out nearY);
		}

		private static double DistanceFromPointToLine(this Point point, Point l1, Point l2, out double nearX, out double nearY)
		{
			var dx = l2.X - l1.X;
			var dy = l2.Y - l1.Y;

			if (dx == 0 & dy == 0) // It's a point not a line segment.
			{
				dx = point.X - l1.X;
				dy = point.Y - l1.Y;
				nearX = l1.X;
				nearY = l1.Y;
				return System.Math.Sqrt(dx * dx + dy * dy);
			}

			// Calculate the t that minimizes the distance.
			var t = ((point.X - l1.X) * dx + (point.Y - l1.Y) * dy) / (dx * dx + dy * dy);

			// See if this represents one of the segment's
			// end points or a point in the middle.
			if (t < 0)
			{
				dx = point.X - l1.X;
				dy = point.Y - l1.Y;
				nearX = l1.X;
				nearY = l1.Y;
			}
			else if (t > 1)
			{
				dx = point.X - l2.X;
				dy = point.Y - l2.Y;
				nearX = l2.X;
				nearY = l2.Y;
			}
			else
			{
				nearX = l1.X + t * dx;
				nearY = l1.Y + t * dy;
				dx = point.X - nearX;
				dy = point.Y - nearY;
			}

			return System.Math.Sqrt(dx * dx + dy * dy);
		}

		public static Point Rotate(this Point pt1, double degree)
		{
			degree = TrigonometryMath.DegreeToRadian(degree);
			var sin = System.Math.Sin(degree);
			var cos = System.Math.Cos(degree); 
			return new Point(
				pt1.X * cos + pt1.Y * sin,
				-1 * pt1.X * sin + pt1.Y * cos
			);
		}


		public static Point Subtract(this Point pt1, Point pt2)
		{
			return new Point(
				pt1.X - pt2.X,
				pt1.Y - pt2.Y
			);
		}

		public static Point Subtract(this Point pt1, double x, double y)
		{
			return new Point(
				pt1.X - x,
				pt1.Y - y
			);
		}

		public static Point Add(this Point pt1, double x, double y)
		{
			return new Point(
				pt1.X + x,
				pt1.Y + y
			);
		}

		public static Point Add(this Point pt1, Point pt2)
		{
			return new Point(
				pt1.X + pt2.X,
				pt1.Y + pt2.Y
			);
		}

		public static Point Multiply(this Point pt1, double factor)
		{
			return new Point(
				pt1.X * factor,
				pt1.Y * factor
			);
		}

		public static Point Multiply(this Point pt1, Point pt2)
		{
			return new Point(
				pt1.X * pt2.X,
				pt1.Y * pt2.Y
			);
		}

		public static Point Divide(this Point pt1, double divisor)
		{
			return new Point(
				pt1.X / divisor,
				pt1.Y / divisor
			);
		}

		public static Point Divide(this Point pt1, Point pt2)
		{
			return new Point(
				pt1.X / pt2.X,
				pt1.Y / pt2.Y
			);
		}
	}
}
