using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MyToolkit.Animation
{
	public static class Background
	{
		public static void FadeIn(Panel panel, string sourceUrl, double opacity, int msecs, Action finishedAction = null)
		{
			if (String.IsNullOrEmpty(sourceUrl))
				return;

			var brush = new ImageBrush();
			var image = new BitmapImage { UriSource = new Uri(sourceUrl, UriKind.RelativeOrAbsolute) };
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

		public static void FadeOut(Panel panel, int msecs, Action finishedAction = null)
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
	}
}
