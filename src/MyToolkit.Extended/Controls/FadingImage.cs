//-----------------------------------------------------------------------
// <copyright file="FadingImage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Animations;
using MyToolkit.Events;
using MyToolkit.Media;
using MyToolkit.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
    public class FadingImage : Control
    {
        private Uri _lastSource = null;
        private bool _isAnimating = false;
        private Grid _canvas;

        public FadingImage()
        {
            DefaultStyleKey = typeof(FadingImage);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = (Grid)GetTemplateChild("canvas");
            UpdateSource();
        }

        public static readonly DependencyProperty FadingOpacityProperty =
            DependencyProperty.Register("FadingOpacity", typeof(double), typeof(FadingImage), new PropertyMetadata(1.0));

        public double FadingOpacity
        {
            get { return (double)GetValue(FadingOpacityProperty); }
            set { SetValue(FadingOpacityProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(FadingImage), new PropertyMetadata(default(Stretch)));

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }


        public static readonly DependencyProperty FadingDurationProperty =
            DependencyProperty.Register("FadingDuration", typeof(TimeSpan), typeof(FadingImage), new PropertyMetadata(new TimeSpan(0, 0, 1)));

        public TimeSpan FadingDuration
        {
            get { return (TimeSpan)GetValue(FadingDurationProperty); }
            set { SetValue(FadingDurationProperty, value); }
        }


        public static readonly DependencyProperty WaitForNextImageProperty =
            DependencyProperty.Register("WaitForNextImage", typeof(bool), typeof(FadingImage), new PropertyMetadata(false));

        public bool WaitForNextImage
        {
            get { return (bool)GetValue(WaitForNextImageProperty); }
            set { SetValue(WaitForNextImageProperty, value); }
        }


        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(FadingImage), new PropertyMetadata(default(Uri), OnSourceChanged));

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var ctrl = (FadingImage)obj;
            ctrl.UpdateSource();
        }

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private async void UpdateSource()
        {
            if (_canvas == null || _isAnimating || _lastSource == Source) // is animating or no change
                return;

            _isAnimating = true;
            var currentSource = Source;
            if (_lastSource == null)
            {
                ForegroundImage.Opacity = 0.0;

                var success = SingleEvent.WaitForEventAsync(ForegroundImage,
                    (image, handler) => image.ImageOpened += handler,
                    (image, handler) => image.ImageOpened -= handler);

                var failure = SingleEvent.WaitForEventAsync(ForegroundImage,
                    (image, handler) => image.ImageFailed += handler,
                    (image, handler) => image.ImageFailed -= handler);

                ImageHelper.SetSource(ForegroundImage, currentSource);

                var task = await Task.WhenAny(new[] { success, failure });
                if (task == success)
                    await Fading.FadeInAsync(ForegroundImage, FadingDuration, FadingOpacity);

                // TODO Deregister not called event
            }
            else if (currentSource != null) // exchange images
            {
                BackgroundImage.Opacity = 0.0;

                var success = SingleEvent.WaitForEventAsync(BackgroundImage,
                    (image, handler) => image.ImageOpened += handler,
                    (image, handler) => image.ImageOpened -= handler);

                var failure = SingleEvent.WaitForEventAsync(BackgroundImage,
                    (image, handler) => image.ImageFailed += handler,
                    (image, handler) => image.ImageFailed -= handler);

                ImageHelper.SetSource(BackgroundImage, currentSource);

                if (!WaitForNextImage)
                    Fading.FadeOutAsync(ForegroundImage, FadingDuration);

                var task = await Task.WhenAny(new[] { success, failure });
                if (task == success)
                {
                    if (WaitForNextImage)
                        await Task.WhenAll(new[]
                            {
                                Fading.FadeInAsync(BackgroundImage, FadingDuration, FadingOpacity),
                                Fading.FadeOutAsync(ForegroundImage, FadingDuration)
                            });
                    else
                        await Fading.FadeInAsync(BackgroundImage, FadingDuration, FadingOpacity);

                    ImageHelper.SetSource(ForegroundImage, null);

                    // reverse image order
                    var fore = ForegroundImage;
                    var back = BackgroundImage;
                    _canvas.Children.Clear();
                    _canvas.Children.Add(fore);
                    _canvas.Children.Add(back);
                }
                else
                    await Fading.FadeOutAsync(ForegroundImage, FadingDuration);
            }
            else
            {
                BackgroundImage.Opacity = 0.0;
                await Fading.FadeOutAsync(ForegroundImage, FadingDuration);
            }

            _isAnimating = false;
            _lastSource = currentSource;
            UpdateSource();
        }

        private Image ForegroundImage
        {
            get { return (Image)_canvas.Children.Last(); }
        }

        private Image BackgroundImage
        {
            get { return (Image)_canvas.Children.First(); }
        }
    }
}

#endif