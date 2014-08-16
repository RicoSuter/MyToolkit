using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

// TODO animations are coded twice?

namespace MyToolkit.Animations
{
	public static class Fading
	{
#if WINRT || WP8

        /// <summary>
        /// Fades an element in. 
        /// </summary>
        /// <param name="obj">The element to animate. </param>
        /// <param name="duration">The animation duration. </param>
        /// <param name="endOpacity">The opacity at the end of the animation. </param>
        /// <returns>Returns a task. </returns>
		async public static Task FadeInAsync(UIElement obj, TimeSpan duration, double endOpacity = 1.0)
		{
			await TaskHelper.RunCallbackMethod(FadeIn, obj, duration, endOpacity);
		}

        /// <summary>
        /// Fades an element out. 
        /// </summary>
        /// <param name="obj">The element to animate. </param>
        /// <param name="duration">The animation duration. </param>
        /// <param name="endOpacity">The opacity at the end of the animation. </param>
        /// <returns>Returns a task. </returns>
        async public static Task FadeOutAsync(UIElement obj, TimeSpan duration, double endOpacity = 0.0)
		{
			await TaskHelper.RunCallbackMethod(FadeOut, obj, duration, endOpacity);
		}
#endif

        /// <summary>
        /// Fades an element in. 
        /// </summary>
        /// <param name="obj">The element to animate. </param>
        /// <param name="duration">The animation duration. </param>
        /// <param name="endOpacity">The opacity at the end of the animation. </param>
        /// <returns>Returns a task. </returns>
		public static void FadeIn(UIElement obj, TimeSpan duration, double endOpacity = 1.0, Action completed = null)
		{
			var animation = new DoubleAnimation();
			animation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut, Exponent = 1 };
			animation.From = obj.Opacity;
			animation.To = endOpacity;
			animation.Duration = new Duration(duration);

			var story = new Storyboard();
			story.Children.Add(animation);

			Storyboard.SetTarget(animation, obj);
			Storyboard.SetTargetProperty(animation, "Opacity");

			if (completed != null)
				story.Completed += delegate { completed(); };
			story.Begin();
		}

        /// <summary>
        /// Fades an element out. 
        /// </summary>
        /// <param name="obj">The element to animate. </param>
        /// <param name="duration">The animation duration. </param>
        /// <param name="endOpacity">The opacity at the end of the animation. </param>
        /// <returns>Returns a task. </returns>
		public static void FadeOut(UIElement obj, TimeSpan duration, double endOpacity = 0.0, Action completed = null)
		{
			var animation = new DoubleAnimation();
			animation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut, Exponent = 1 };
			animation.From = obj.Opacity;
			animation.To = endOpacity;
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
