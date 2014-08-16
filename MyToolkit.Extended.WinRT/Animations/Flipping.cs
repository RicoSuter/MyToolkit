using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyToolkit.Animations
{
    /// <summary>
    /// Methods to flip a two-sided GUI element to the back and front. 
    /// </summary>
	public static class Flipping
	{
        /// <summary>
        /// Flips the element to its back view. 
        /// </summary>
        /// <param name="panel">A panel containing the front element and the back element. </param>
        /// <param name="frontElement">The front element. </param>
        /// <param name="backElement">The back element. </param>
        /// <param name="duration">The anmiation duration. </param>
        /// <param name="completed">Callback which is called after the animation has finished. </param>
		public static void FlipToBack(UIElement panel, UIElement frontElement, UIElement backElement, TimeSpan duration, Action completed = null)
		{
			Flip(panel, frontElement, backElement, duration, true, completed);
		}

        /// <summary>
        /// Flips the element to its front view. 
        /// </summary>
        /// <param name="panel">A panel containing the front element and the back element. </param>
        /// <param name="frontElement">The front element. </param>
        /// <param name="backElement">The back element. </param>
        /// <param name="duration">The anmiation duration. </param>
        /// <param name="completed">Callback which is called after the animation has finished. </param>
        public static void FlipToFront(UIElement panel, UIElement frontElement, UIElement backElement, TimeSpan duration, Action completed = null)
		{
			Flip(panel, frontElement, backElement, duration, false, completed);
		}

        /// <summary>
        /// Flips the element. 
        /// </summary>
        /// <param name="panel">A panel containing the front element and the back element. </param>
        /// <param name="frontElement">The front element. </param>
        /// <param name="backElement">The back element. </param>
        /// <param name="duration">The anmiation duration. </param>
        /// <param name="transitionToBack">Specifies whether the element should be flipped to its back view. </param>
        /// <param name="completed">Callback which is called after the animation has finished. </param>
        public static void Flip(UIElement panel, UIElement frontElement, UIElement backElement, TimeSpan duration, bool transitionToBack, Action completed = null)
		{
			duration = new TimeSpan(duration.Ticks / 2);
			
            if (!(panel.Projection is PlaneProjection))
				panel.Projection = new PlaneProjection();

			var animation = new DoubleAnimation();
			animation.From = 0.0;
			animation.To = 90.0 * (transitionToBack ? 1 : -1);
			animation.Duration = new Duration(duration);

			var story = new Storyboard();
			story.Children.Add(animation);

            Storyboard.SetTarget(animation, panel.Projection);
			Storyboard.SetTargetProperty(animation, "RotationY");

			story.Completed += delegate
			{
				animation = new DoubleAnimation();
				animation.From = 270.0 * (transitionToBack ? 1 : -1);
				animation.To = 360.0 * (transitionToBack ? 1 : -1);
				animation.Duration = new Duration(duration);

				story = new Storyboard();
				story.Children.Add(animation);

                Storyboard.SetTarget(animation, panel.Projection);
				Storyboard.SetTargetProperty(animation, "RotationY");

				frontElement.Visibility = transitionToBack ? Visibility.Collapsed : Visibility.Visible;
				backElement.Visibility = !transitionToBack ? Visibility.Collapsed : Visibility.Visible;

				story.Completed += delegate
				{
                    ((PlaneProjection)panel.Projection).RotationY = 0.0;
					if (completed != null)
						completed();
				};
				story.Begin();
			};
			story.Begin();
		}
	}
}
