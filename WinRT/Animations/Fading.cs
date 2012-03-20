using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace MyToolkit.Animations
{
	public static class Fading
	{
		public static void FadeIn(UIElement obj, TimeSpan duration, Action completed = null)
		{
			var animation = new DoubleAnimation();
			animation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 6 };
			animation.From = 0.0;
			animation.To = 1.0;
			animation.EasingFunction = new ExponentialEase { Exponent = 6, EasingMode = EasingMode.EaseOut };
			animation.Duration = new Duration(duration);

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, obj);
			Storyboard.SetTargetProperty(animation, "Opacity");

			if (completed != null)
				story.Completed += delegate { completed(); };
			story.Begin();
		}

		public static void FadeOut(UIElement obj, TimeSpan duration, Action completed = null)
		{
			var animation = new DoubleAnimation();
			animation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 6 };
			animation.From = 1.0;
			animation.To = 0.0;
			animation.EasingFunction = new ExponentialEase { Exponent = 6, EasingMode = EasingMode.EaseOut };
			animation.Duration = new Duration(duration);

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, obj);
			Storyboard.SetTargetProperty(animation, "Opacity");

			if (completed != null)
				story.Completed += delegate { completed(); };
			story.Begin();
		}
	}
}
