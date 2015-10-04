#if !WINRT

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MyToolkit.Animation
{
	public static class Control
	{
		public static void FadeIn(UIElement target, int msecs, Action finishedAction = null)
		{
			var animation = new DoubleAnimation();
			animation.From = 0.0;
			animation.To = 1.0;
			animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs));

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, target);
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

			if (finishedAction != null)
				story.Completed += delegate { finishedAction(); };

			story.Begin();
		}

		public static void FadeOut(UIElement target, int msecs, Action finishedAction = null)
		{
			var animation = new DoubleAnimation();
			animation.From = target.Opacity;
			animation.To = 0.0;
			animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs));

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, target);
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

			story.Completed += delegate
			                   	{
			                   		target.Opacity = 0.0;
			                   		if (finishedAction != null)
			                   			finishedAction();
			                   	};

			story.Begin();
		}
	}
}

#endif