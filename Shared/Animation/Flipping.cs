using System;

#if METRO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
#endif

namespace MyToolkit.Animations
{
	public static class Flipping
	{
		public static void FlipToBack(UIElement parentCtrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration, Action completed = null)
		{
			Flip(parentCtrl, frontCtrl, backCtrl, duration, true, completed);
		}

		public static void FlipToFront(UIElement parentCtrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration, Action completed = null)
		{
			Flip(parentCtrl, frontCtrl, backCtrl, duration, false, completed);
		}

		public static void Flip(UIElement parentCtrl, UIElement frontCtrl, UIElement backCtrl, TimeSpan duration, bool transitionToBack, Action completed = null)
		{
			duration = new TimeSpan(duration.Ticks / 2);
			var proj = parentCtrl.Projection is PlaneProjection ? (PlaneProjection)parentCtrl.Projection : new PlaneProjection();
			if (parentCtrl.Projection != proj)
				parentCtrl.Projection = proj;

			var animation = new DoubleAnimation();
			animation.From = 0.0;
			animation.To = 90.0 * (transitionToBack ? 1 : -1);
			animation.Duration = new Duration(duration);

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, proj);
#if METRO
			Storyboard.SetTargetProperty(animation, "RotationY");
#else
			Storyboard.SetTargetProperty(animation, new PropertyPath("RotationY"));
#endif

			story.Completed += delegate
			{
				animation = new DoubleAnimation();
				animation.From = 270.0 * (transitionToBack ? 1 : -1);
				animation.To = 360.0 * (transitionToBack ? 1 : -1);
				animation.Duration = new Duration(duration);

				story = new Storyboard();
				story.Children.Add(animation);

				Storyboard.SetTarget(animation, proj);
#if METRO
				Storyboard.SetTargetProperty(animation, "RotationY");
#else
				Storyboard.SetTargetProperty(animation, new PropertyPath("RotationY"));
#endif

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
