using System.Linq;
using Windows.Foundation;

namespace MyToolkit.Mathematics
{
	public static class GeometryMath
	{
		public static Rect GetEnclosingRect(Rect[] list)
		{
			var rect = list.First();
			foreach (var r in list.Skip(1))
			{
				rect = new Rect(
					new Point(System.Math.Min(rect.Left, r.Left), System.Math.Min(rect.Top, r.Top)),
					new Point(System.Math.Max(rect.Right, r.Right), System.Math.Max(rect.Bottom, r.Bottom)));
			}
			return rect;
		}

		public static bool Intersects(this Rect rect1, Rect rect2)
		{
			rect1.Intersect(rect2);
			return !rect1.IsEmpty;
		}

		public static bool Intersects(this Rect r, Point[] polyline)
		{
			for (var i = 1; i < polyline.Length; i++)
			{
				var pt1 = polyline[i];
				var pt2 = polyline[i - 1];

				if (VectorMath.Intersects(pt1, pt2, r))
					return true;
			}
			return false;
		}
	}
}
