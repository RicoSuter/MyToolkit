//-----------------------------------------------------------------------
// <copyright file="PushPageAnimation.cs" company="MyToolkit">
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
    /// <summary>A push page animation. </summary>
    public class PushPageAnimation : IPageAnimation
    {
        /// <summary>Initializes a new instance of the <see cref="PushPageAnimation"/> class. </summary>
        public PushPageAnimation()
        {
            Duration = TimeSpan.FromSeconds(0.15);
        }

        /// <summary>Gets or sets the duration of the animation. </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>Gets the insertion mode for the next page.</summary>
        public PageInsertionMode PageInsertionMode
        {
            get { return PageInsertionMode.ConcurrentAbove; }
        }

        /// <summary>Animates for navigating forward from a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateForwardNavigatingFromAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            var width = previousPage.ActualWidth;
            return AnimateAsync(previousPage, nextPage, 0, -(width / 2.0), width, width / 2.0);
        }

        /// <summary>Animates for navigating forward to a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateForwardNavigatedToAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            var width = previousPage.ActualWidth;
            return AnimateAsync(previousPage, nextPage, -(width / 2.0), -width, width / 2, 0);
        }

        /// <summary>Animates for navigating backward from a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateBackwardNavigatingFromAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            var width = previousPage.ActualWidth;
            return AnimateAsync(previousPage, nextPage, 1, width / 2.0, -width, -(width / 2.0));
        }

        /// <summary>Animates for navigating backward to a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        public Task AnimateBackwardNavigatedToAsync(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            var width = previousPage.ActualWidth;
            return AnimateAsync(previousPage, nextPage, width / 2.0, width, -(width / 2.0), 0);
        }

        private Task AnimateAsync(FrameworkElement page, FrameworkElement otherPage, double fromOffsetPreviousPage, double toOffsetPreviousPage, double fromOffsetNextPage, double toOffsetNextPage)
        {
            if (page == null)
                return Task.FromResult<object>(null);
            
            page.Projection = new PlaneProjection();
            otherPage.Projection = new PlaneProjection();

            var story = new Storyboard();

            var animation1 = new DoubleAnimation
            {
                Duration = Duration,
                From = fromOffsetPreviousPage,
                To = toOffsetPreviousPage
            };
            Storyboard.SetTargetProperty(animation1, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)");
            Storyboard.SetTarget(animation1, page);

            var animation2 = new DoubleAnimation
            {
                Duration = Duration,
                From = fromOffsetNextPage,
                To = toOffsetNextPage
            };
            Storyboard.SetTargetProperty(animation2, "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)");
            Storyboard.SetTarget(animation2, otherPage);

            story.Children.Add(animation1);
            story.Children.Add(animation2);

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