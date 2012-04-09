using System;

#if METRO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

#endif

namespace MyToolkit.Animations
{
	public static class Fading
	{
		public static void FadeIn(UIElement obj, int duration, Action completed = null)
		{
			FadeIn(obj, new TimeSpan(0, 0, 0, 0, duration), completed);
		}

		public static void FadeIn(UIElement obj, TimeSpan duration, Action completed = null)
		{
			var animation = new DoubleAnimation();
			animation.From = obj.Opacity;
			animation.To = 1.0;
			animation.EasingFunction = new ExponentialEase { Exponent = 6, EasingMode = EasingMode.EaseOut };
			animation.Duration = new Duration(duration);

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, obj);
#if METRO
			Storyboard.SetTargetProperty(animation, "Opacity");
#else
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
#endif

			if (completed != null)
				story.Completed += delegate { completed(); };
			story.Begin();
		}

		public static void FadeOut(UIElement obj, int duration, Action completed = null)
		{
			FadeOut(obj, new TimeSpan(0, 0, 0, 0, duration), completed);
		}

		public static void FadeOut(UIElement obj, TimeSpan duration, Action completed = null)
		{
			var animation = new DoubleAnimation();
			animation.From = obj.Opacity;
			animation.To = 0.0;
			animation.EasingFunction = new ExponentialEase { Exponent = 6, EasingMode = EasingMode.EaseOut };
			animation.Duration = new Duration(duration);

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, obj);
#if METRO
			Storyboard.SetTargetProperty(animation, "Opacity");
#else
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
#endif

			if (completed != null)
				story.Completed += delegate { completed(); };
			story.Begin();
		}

#if !METRO
		public static void FadeInBackground(Panel panel, string backgroundSourceUrl, double opacity, 
			int msecs, Action finishedAction = null)
		{
			if (String.IsNullOrEmpty(backgroundSourceUrl))
				return;

			var brush = new ImageBrush();
			var image = new BitmapImage { UriSource = new Uri(backgroundSourceUrl, UriKind.RelativeOrAbsolute) };
			brush.Opacity = panel.Background != null ? panel.Background.Opacity : 0.0;
			brush.Stretch = Stretch.UniformToFill;
			brush.ImageSource = image;

			panel.Background = brush;
			brush.ImageOpened += delegate
			{
				var animation = new DoubleAnimation();
				animation.From = panel.Background != null ? panel.Background.Opacity : 0.0;
				animation.To = opacity;
				animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs));

				var story = new Storyboard();
				story.Children.Add(animation);

				Storyboard.SetTarget(animation, brush);
				Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

				if (finishedAction != null)
					story.Completed += delegate { finishedAction(); };
				story.Begin();
			};
		}

		public static void FadeOutBackground(Panel panel, int msecs, Action finishedAction = null)
		{
			if (panel.Background.Opacity == 0.0)
				return;

			var animation = new DoubleAnimation();
			animation.From = panel.Background.Opacity;
			animation.To = 0.0;
			animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs));

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, panel.Background);
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

			if (finishedAction != null)
				story.Completed += delegate { finishedAction(); };
			story.Begin();
		}
#endif
	}
}
