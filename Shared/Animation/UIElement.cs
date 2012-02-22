using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MyToolkit.Animation
{
	public static class UIElementExtension
	{
		public static void FadeIn(UIElement element, int msecs, Action finishedAction = null)
		{
			if (element.Opacity == 1.0)
				return;

			var animation = new DoubleAnimation();
			animation.From = element.Opacity;
			animation.To = 1.0;
			animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs));

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, element);
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

			if (finishedAction != null)
				story.Completed += delegate { finishedAction(); };
			story.Begin();
		}

		public static void FadeOut(UIElement element, int msecs, Action finishedAction = null)
		{
			if (element.Opacity == 0.0)
				return;

			var animation = new DoubleAnimation();
			animation.From = element.Opacity;
			animation.To = 0.0;
			animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs));

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, element);
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

			if (finishedAction != null)
				story.Completed += delegate { finishedAction(); };
			story.Begin();
		}
	}
}
