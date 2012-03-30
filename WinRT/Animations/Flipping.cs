using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyToolkit.Animations
{
	public static class Flipping
	{
		public static void FlipToBack(UIElement ctrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration, Action completed = null)
		{
			Flip(ctrl, frontCtrl, backCtrl, duration, true, completed);
		}

		public static void FlipToFront(UIElement ctrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration, Action completed = null)
		{
			Flip(ctrl, frontCtrl, backCtrl, duration, false, completed);
		}

		public static void Flip(UIElement ctrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration, bool transitionToBack, Action completed = null)
		{
			duration = new TimeSpan(duration.Ticks / 2);
			var proj = ctrl.Projection is PlaneProjection ? (PlaneProjection)ctrl.Projection : new PlaneProjection();
			if (ctrl.Projection != proj)
				ctrl.Projection = proj;

			var animation = new DoubleAnimation();
			animation.From = 0.0;
			animation.To = 90.0 * (transitionToBack ? 1 : -1);
			animation.Duration = new Duration(duration);

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, proj);
			Storyboard.SetTargetProperty(animation, "RotationY");

			story.Completed += delegate
			{
				animation = new DoubleAnimation();
				animation.From = 270.0 * (transitionToBack ? 1 : -1);
				animation.To = 360.0 * (transitionToBack ? 1 : -1);
				animation.Duration = new Duration(duration);

				story = new Storyboard();
				story.Children.Add(animation);

				Storyboard.SetTarget(animation, proj);
				Storyboard.SetTargetProperty(animation, "RotationY");

				frontCtrl.Visibility = transitionToBack ? Visibility.Collapsed : Visibility.Visible;
				backCtrl.Visibility = !transitionToBack ? Visibility.Collapsed : Visibility.Visible;

				story.Completed += delegate
				{
					proj.RotationY = 0.0;
					if (completed != null)
						completed();
				};
				story.Begin();
			};
			story.Begin();
		}
	}
}
