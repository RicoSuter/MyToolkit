//-----------------------------------------------------------------------
// <copyright file="TurnstilePageAnimation.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyToolkit.Paging.Animations
{
    /// <summary>A turnstile animation. </summary>
    public class TurnstilePageAnimation : IPageAnimation
    {
        /// <summary>Initializes a new instance of the <see cref="TurnstilePageAnimation"/> class. </summary>
        public TurnstilePageAnimation()
        {
            Duration = TimeSpan.FromSeconds(0.15);
        }

        /// <summary>Gets or sets the duration of the animation. </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>Animates for navigating forward from a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateForwardNavigatingFromAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            nextPage.Opacity = 0;
            return AnimateAsync(previousPage, 5, 75, 1, 0);
        }

        /// <summary>Animates for navigating forward to a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateForwardNavigatedToAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            previousPage.Opacity = 0;
            return AnimateAsync(nextPage, -75, 0, 0, 1);
        }

        /// <summary>Animates for navigating backward from a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateBackwardNavigatingFromAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            nextPage.Opacity = 0;
            return AnimateAsync(previousPage, -5, -75, 1, 0);
        }

        /// <summary>Animates for navigating backward to a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateBackwardNavigatedToAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            previousPage.Opacity = 0;
            return AnimateAsync(nextPage, 75, 0, 0, 1);
        }

        private Task AnimateAsync(FrameworkElement page, double fromRotation, double toRotation, double fromOpacity, double toOpacity)
        {
            if (page == null)
                return Task.FromResult<object>(null);

            page.Opacity = 1;
            page.Projection = new PlaneProjection { CenterOfRotationX = 0 };

            var story = new Storyboard();

            var turnstileAnimation = new DoubleAnimation
            {
                Duration = Duration,
                From = fromRotation, 
                To = toRotation
            };
            Storyboard.SetTargetProperty(turnstileAnimation, "(UIElement.Projection).(PlaneProjection.RotationY)");
            Storyboard.SetTarget(turnstileAnimation, page);

            var opacityAnimation = new DoubleAnimation
            {
                Duration = Duration,
                From = fromOpacity,
                To = toOpacity,
            };
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            Storyboard.SetTarget(opacityAnimation, page);

            story.Children.Add(turnstileAnimation);
            story.Children.Add(opacityAnimation);

            var completion = new TaskCompletionSource<object>();
            story.Completed += delegate
            {
                ((PlaneProjection) page.Projection).RotationY = toRotation;
                page.Opacity = toOpacity;
                completion.SetResult(null);
            };
            story.Begin();
            return completion.Task;
        }
    }
}