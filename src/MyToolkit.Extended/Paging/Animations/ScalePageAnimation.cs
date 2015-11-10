//-----------------------------------------------------------------------
// <copyright file="ScalePageTransition.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyToolkit.Paging.Animations
{
    /// <summary>Scales the page like pushing a button.</summary>
    public class ScalePageTransition : IPageAnimation
    {
        /// <summary>Initializes a new instance of the <see cref="TurnstilePageAnimation"/> class. </summary>
        public ScalePageTransition()
        {
            Duration = TimeSpan.FromMilliseconds(80);
        }

        /// <summary>Gets or sets the duration of the animation (default: 150ms). </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>Gets the insertion mode for the next page.</summary>
        public PageInsertionMode PageInsertionMode { get { return PageInsertionMode.ConcurrentAbove; } }

        /// <summary>Animates for navigating forward from a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateForwardNavigatingFromAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            previousPage.Opacity = 1;
            nextPage.Opacity = 0;
            return AnimateAsync(previousPage, 1, 1.1, 1, 0);
        }

        /// <summary>Animates for navigating forward to a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateForwardNavigatedToAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            previousPage.Opacity = 0;
            nextPage.Opacity = 1;
            return AnimateAsync(nextPage, 0.9, 1, 0, 1);
        }

        /// <summary>Animates for navigating backward from a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateBackwardNavigatingFromAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            previousPage.Opacity = 1;
            nextPage.Opacity = 0;
            return AnimateAsync(previousPage, 1, 0.9, 1, 0);
        }

        /// <summary>Animates for navigating backward to a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateBackwardNavigatedToAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            previousPage.Opacity = 0;
            nextPage.Opacity = 1;
            return AnimateAsync(nextPage, 1.1, 1, 0, 1);
        }

        private Task AnimateAsync(FrameworkElement page, double fromPreviousPage, double toPreviousPage, double opacityFrom, double opacityTo)
        {
            if (page == null)
                return Task.FromResult<object>(null);

            page.RenderTransform = new ScaleTransform
            {
                CenterX = page.ActualWidth / 2,
                CenterY = page.ActualHeight / 2
            };

            var story = new Storyboard();

            var scaleXAnimation = new DoubleAnimation
            {
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut },
                Duration = Duration,
                From = fromPreviousPage,
                To = toPreviousPage
            };
            Storyboard.SetTargetProperty(scaleXAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
            Storyboard.SetTarget(scaleXAnimation, page);

            var scaleYAnimation = new DoubleAnimation
            {
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut },
                Duration = Duration,
                From = fromPreviousPage,
                To = toPreviousPage
            };
            Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
            Storyboard.SetTarget(scaleYAnimation, page);

            var opacityAnimation = new DoubleAnimation
            {
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut },
                Duration = Duration,
                From = opacityFrom,
                To = opacityTo
            };
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            Storyboard.SetTarget(opacityAnimation, page);

            story.Children.Add(scaleXAnimation);
            story.Children.Add(scaleYAnimation);
            story.Children.Add(opacityAnimation);

            var completion = new TaskCompletionSource<object>();
            story.Completed += delegate
            {
                completion.SetResult(null);
            };
            story.Begin();
            return completion.Task;
        }
    }
}

#endif