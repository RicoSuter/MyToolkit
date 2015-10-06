//-----------------------------------------------------------------------
// <copyright file="Fading.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using System;
using System.Threading.Tasks;
using MyToolkit.Utilities;
#if WINRT
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
            await TaskUtilities.RunCallbackMethodAsync(FadeIn, obj, duration, endOpacity);
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
            await TaskUtilities.RunCallbackMethodAsync(FadeOut, obj, duration, endOpacity);
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
#if WINRT
            Storyboard.SetTargetProperty(animation, "Opacity");
#else
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
#endif
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
#if WINRT
            Storyboard.SetTargetProperty(animation, "Opacity");
#else
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
#endif
            if (completed != null)
                story.Completed += delegate { completed(); };
            story.Begin();
        }

#if !WINRT
        public static bool FadeInBackground(Panel panel, Uri backgroundSourceUri, double opacity, 
            int msecs, Action completed = null)
        {
            if (backgroundSourceUri == null)
                return false;

            var brush = new ImageBrush();
            var image = new BitmapImage { UriSource = backgroundSourceUri };
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

                if (completed != null)
                    story.Completed += delegate { completed(); };
                story.Begin();
            };
            return true; 
        }

        public static bool FadeOutBackground(Panel panel, int msecs, Action completed = null)
        {
            if (panel.Background == null || panel.Background.Opacity == 0.0)
                return false;

            var animation = new DoubleAnimation();
            animation.From = panel.Background.Opacity;
            animation.To = 0.0;
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, msecs));

            var story = new Storyboard();
            story.Children.Add(animation);

            Storyboard.SetTarget(animation, panel.Background);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            story.Completed += delegate
            {
                panel.Background = null;
                if (completed != null)
                    completed();
            };

            story.Begin();
            return true; 
        }
#endif
    }
}

#endif