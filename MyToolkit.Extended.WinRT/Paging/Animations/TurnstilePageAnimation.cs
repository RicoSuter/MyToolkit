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
    public class TurnstilePageAnimation : IPageAnimation
    {
        public TurnstilePageAnimation()
        {
            Duration = TimeSpan.FromSeconds(0.15);
        }
        
        public TimeSpan Duration { get; set; }

        public Task NavigatingFromForward(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            nextPage.Opacity = 0;
            return Turnstile(previousPage, 5, 75, 1, 0);
        }

        public Task NavigatedToForward(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            previousPage.Opacity = 0;
            return Turnstile(nextPage, -75, 0, 0, 1);
        }

        public Task NavigatingFromBackward(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            nextPage.Opacity = 0;
            return Turnstile(previousPage, -5, -75, 1, 0);
        }

        public Task NavigatedToBackward(FrameworkElement previousPage, FrameworkElement nextPage)
        {
            previousPage.Opacity = 0;
            return Turnstile(nextPage, 75, 0, 0, 1);
        }

        private Task Turnstile(FrameworkElement source, double fromRotation, double toRotation, double fromOpacity, double toOpacity)
        {
            if (source == null)
                return Task.FromResult<object>(null);

            source.Opacity = 1;
            source.Projection = new PlaneProjection { CenterOfRotationX = 0 };

            var story = new Storyboard();

            var turnstileAnimation = new DoubleAnimation
            {
                Duration = Duration,
                From = fromRotation, 
                To = toRotation
            };
            Storyboard.SetTargetProperty(turnstileAnimation, "(UIElement.Projection).(PlaneProjection.RotationY)");
            Storyboard.SetTarget(turnstileAnimation, source);

            var opacityAnimation = new DoubleAnimation
            {
                Duration = Duration,
                From = fromOpacity,
                To = toOpacity,
            };
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            Storyboard.SetTarget(opacityAnimation, source);

            story.Children.Add(turnstileAnimation);
            story.Children.Add(opacityAnimation);

            var completion = new TaskCompletionSource<object>();
            story.Completed += delegate
            {
                ((PlaneProjection) source.Projection).RotationY = toRotation;
                source.Opacity = toOpacity;
                completion.SetResult(null);
            };
            story.Begin();
            return completion.Task;
        }
    }
}