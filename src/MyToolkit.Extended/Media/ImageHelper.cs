//-----------------------------------------------------------------------
// <copyright file="ImageHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyToolkit.Networking;
using MyToolkit.Utilities;

#if WINRT
using System.Net.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#endif

namespace MyToolkit.Media
{
    /// <summary>
    /// Provides an attached property to use authenticated URIs for images and defer image loading. 
    /// </summary>
    public class ImageHelper
    {
        private static readonly Dictionary<Image, Uri> _pendingUpdates = new Dictionary<Image, Uri>();
        private static bool _isEnabled = true;

        /// <summary>
        /// Gets or sets a value indicating whether images can be currently downloaded (when false then changes get queued and downloaded when set to true).  
        /// </summary>
        public static bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                if (_isEnabled)
                {
                    var copy = _pendingUpdates.ToArray();
                    _pendingUpdates.Clear();

                    foreach (var pair in copy)
                        LoadImage(pair.Key, pair.Value);
                }
            }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
            "Source", typeof(Uri), typeof(ImageHelper), new PropertyMetadata(default(Uri), OnSourceChanged));

        public static void SetSource(DependencyObject element, Uri value)
        {
            element.SetValue(SourceProperty, value);
        }

        public static Uri GetSource(DependencyObject element)
        {
            return (Uri)element.GetValue(SourceProperty);
        }

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var image = (Image)obj;
            var uri = (Uri)e.NewValue;

            image.Source = null; // Needed when using virtualization
            if (IsEnabled)
                LoadImage(image, uri);
            else
                _pendingUpdates[image] = uri; 
        }

#if WINRT
        private static async void LoadImage(Image image, Uri uri)
#else
        private static void LoadImage(Image image, Uri uri)
#endif
        {
            if (uri == null)
            {
                image.Source = null; 
                return;
            }

            try
            {
#if WINRT
                var handler = new HttpClientHandler();
                if (uri is AuthenticatedUri)
                    handler.Credentials = ((AuthenticatedUri) uri).Credentials;

                var client = new HttpClient(handler);
                var stream = await client.GetStreamAsync(uri);

                var source = new BitmapImage();
                image.Source = source;
                using (var memoryStream = new MemoryStream(stream.ReadToEnd()))
                    source.SetSourceAsync(memoryStream.AsRandomAccessStream());
#else
                var request = WebRequest.CreateHttp(uri);
                if (uri is AuthenticatedUri)
                    request.Credentials = ((AuthenticatedUri) uri).Credentials;
    
                request.AllowReadStreamBuffering = true;
                request.BeginGetResponse(asyncResult =>
                {
                    try
                    {
                        var response = (HttpWebResponse) request.EndGetResponse(asyncResult);
                        image.Dispatcher.BeginInvoke(delegate
                        {
                            var source = new BitmapImage();
                            source.SetSource(response.GetResponseStream());
                            image.Source = source;
                        });
                    }
                    catch
                    {
                        image.Dispatcher.BeginInvoke(delegate
                        {
                            image.Source = new BitmapImage(new Uri("http://0.0.0.0")); // Trigger ImageFailed event
                        });
                    }
                }, null);
#endif
            }
            catch
            {
                image.Source = new BitmapImage(new Uri("http://0.0.0.0")); // Trigger ImageFailed event
            }
        }
    }
}

#endif