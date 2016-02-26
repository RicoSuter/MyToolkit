//-----------------------------------------------------------------------
// <copyright file="FixedHtmlTextBlock.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using System;
using System.Collections.Generic;
using MyToolkit.Controls.Html;
using MyToolkit.Utilities;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
#endif

namespace MyToolkit.Controls
{
    /// <summary>Renders HTML using native XAML controls without a scrollbar; use the  <see cref="ScrollableHtmlView"/> 
    /// control to render the HTML content in a <see cref="ScrollViewer"/>. </summary>
    public class HtmlView : ItemsControl, IHtmlView
    {
        private readonly IDictionary<string, IControlGenerator> _generators;

        /// <summary>Initializes a new instance of the <see cref="HtmlView"/> class. </summary>
        public HtmlView()
        {
            _generators = HtmlViewHelper.GetDefaultGenerators(this);

#if !WINRT
            if (Resources.Contains("PhoneFontSizeNormal"))
                FontSize = (double)Resources["PhoneFontSizeNormal"];

            if (Resources.Contains("PhoneForegroundBrush"))
                Foreground = (Brush)Resources["PhoneForegroundBrush"];

            Margin = new Thickness(12, 0, 12, 0);
#else
            FontSize = (double)Resources["ContentControlFontSize"];
            Foreground = (Brush)Resources["ApplicationForegroundThemeBrush"];
#endif

            SizeChanged += OnSizeChanged;
            SizeDependentControls = new List<ISizeDependentControl>();

            DependencyPropertyChangedEvent.Register(this, FontSizeProperty, delegate { this.Generate(); });
            DependencyPropertyChangedEvent.Register(this, FontFamilyProperty, delegate { this.Generate(); });
            DependencyPropertyChangedEvent.Register(this, ForegroundProperty, delegate { this.Generate(); });
            DependencyPropertyChangedEvent.Register(this, BackgroundProperty, delegate { this.Generate(); });
        }

        /// <summary>Gets the list of HTML element generators. </summary>
        public IDictionary<string, IControlGenerator> Generators { get { return _generators; } }

        /// <summary>Gets the list of size dependent controls. </summary>
        public List<ISizeDependentControl> SizeDependentControls { get; private set; }

        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.Register("Html", typeof(string), typeof(HtmlView), new PropertyMetadata(default(string), 
                (obj, e) => ((HtmlView)obj).Generate()));

        /// <summary>Gets or sets the HTML content to display. </summary>
        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        /// <summary>Occurs when the HTML content has been loaded. </summary>
        public event EventHandler<EventArgs> HtmlLoaded;

#if !WINRT
        public static readonly DependencyProperty ParagraphMarginProperty =
            DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(HtmlView), new PropertyMetadata(6));
#else
        public static readonly DependencyProperty ParagraphMarginProperty =
            DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(HtmlView), new PropertyMetadata(10));
#endif

        /// <summary>Gets or sets the margin for paragraphs (added at the bottom of the element). </summary>
        public int ParagraphMargin
        {
            get { return (int)GetValue(ParagraphMarginProperty); }
            set { SetValue(ParagraphMarginProperty, value); }
        }

        public static readonly DependencyProperty BaseUriProperty =
            DependencyProperty.Register("HtmlBaseUri", typeof(Uri), typeof(HtmlView), new PropertyMetadata(default(Uri)));

        /// <summary>Gets or sets the base URI which is used to resolve relative URIs. </summary>
        public Uri HtmlBaseUri
        {
            get { return (Uri)GetValue(BaseUriProperty); }
            set { SetValue(BaseUriProperty, value); }
        }

        /// <summary>Gets the generator for the tag name or creates a new one.</summary>
        /// <typeparam name="TGenerator">The type of the generator.</typeparam>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>The generator.</returns>
        public TGenerator GetGenerator<TGenerator>(string tagName)
            where TGenerator : IControlGenerator, new()
        {
            if (Generators.ContainsKey(tagName))
                return (TGenerator)Generators[tagName];
            Generators[tagName] = new TGenerator();
            return (TGenerator)Generators[tagName];
        }

        /// <summary>Refreshes the rendered HTML (should be called when changing the generators).</summary>
        public void Refresh()
        {
            this.Generate();
        }

        /// <summary>Calls the <see cref="HtmlLoaded"/> event. </summary>
        internal void OnHtmlLoaded()
        {
            var copy = HtmlLoaded;
            if (copy != null)
                copy(this, new EventArgs());
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            foreach (var ctrl in SizeDependentControls)
                ctrl.Update(ActualWidth);
        }
    }

    [Obsolete("Use HtmlView instead. 7/14/2015")]
    public class FixedHtmlTextBlock : HtmlView
    {
    }
}

#endif