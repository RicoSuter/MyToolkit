//-----------------------------------------------------------------------
// <copyright file="PanAndZoomImage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using System;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

namespace MyToolkit.Controls
{
    public class PanAndZoomImage : PanAndZoomViewer
    {
        private Image image;
        public PanAndZoomImage()
        {
            DefaultStyleKey = typeof(PanAndZoomImage);
        }

#if WINRT

        public event RoutedEventHandler ImageOpened;
        public event ExceptionRoutedEventHandler ImageFailed;
#else
        public event EventHandler<RoutedEventArgs> ImageOpened;
        public event EventHandler<ExceptionRoutedEventArgs> ImageFailed;

#endif

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            image = (Image)GetTemplateChild("image");
            image.IsHitTestVisible = false;
            image.ImageOpened += OnImageOpened;
            image.ImageFailed += OnImageFailed;

            SizeChanged += delegate { UpdateMaxZoomFactor(); };
        }

        private void OnImageOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var copy = ImageOpened;
            if (copy != null)
                copy(this, new RoutedEventArgs());

            UpdateMaxZoomFactor();
        }

        private void OnImageFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            var copy = ImageFailed;
            if (copy != null)
                copy(this, null);

            UpdateMaxZoomFactor();
        }

        private void UpdateMaxZoomFactor()
        {
            if (AutomaticZoomFactor && ActualHeight > 0 && ActualWidth > 0)
            {
                var bitmap = image.Source as BitmapImage;
                if (bitmap != null && bitmap.PixelWidth > 0 && bitmap.PixelHeight > 0)
                    CalculateMaxZoomFactor(bitmap.PixelWidth, bitmap.PixelHeight);
            }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(PanAndZoomImage), new PropertyMetadata(default(Uri)));

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(PanAndZoomImage), new PropertyMetadata(default(Stretch)));

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty AutomaticMaxZoomFactorProperty =
            DependencyProperty.Register("AutomaticMaxZoomFactor", typeof(bool), typeof(PanAndZoomImage), new PropertyMetadata(true));

        public bool AutomaticZoomFactor
        {
            get { return (bool)GetValue(AutomaticMaxZoomFactorProperty); }
            set { SetValue(AutomaticMaxZoomFactorProperty, value); }
        }

        public static readonly DependencyProperty InnerMarginProperty =
            DependencyProperty.Register("InnerMargin", typeof(Thickness), typeof(PanAndZoomImage), new PropertyMetadata(default(Thickness)));

        public Thickness InnerMargin
        {
            get { return (Thickness)GetValue(InnerMarginProperty); }
            set { SetValue(InnerMarginProperty, value); }
        }
    }
}

#endif