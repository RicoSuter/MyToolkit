//-----------------------------------------------------------------------
// <copyright file="FixedHtmlTextBlock.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyToolkit.Controls.HtmlTextBlockImplementation;

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
    /// <summary>Renders HTML using native XAML controls without a scrollbar; use the control 
    /// <see cref="HtmlTextBlock"/> to render the HTML content in a <see cref="ScrollViewer"/>. </summary>
	public class FixedHtmlTextBlock : ItemsControl, IHtmlTextBlock
	{
        private readonly IDictionary<string, IControlGenerator> _generators = HtmlParser.GetDefaultGenerators();

        /// <summary>Initializes a new instance of the <see cref="FixedHtmlTextBlock"/> class. </summary>
        public FixedHtmlTextBlock()
		{
#if !WINRT
			if (Resources.Contains("PhoneFontSizeNormal"))
				FontSize = (double)Resources["PhoneFontSizeNormal"];

			if (Resources.Contains("PhoneForegroundBrush"))
				Foreground = (Brush)Resources["PhoneForegroundBrush"];

			Margin = new Thickness(12, 0, 12, 0);
#else
            FontSize = (double)Resources["ControlContentThemeFontSize"];
			Foreground = (Brush)Resources["ApplicationForegroundThemeBrush"];
#endif

			SizeChanged += OnSizeChanged;
			SizeDependentControls = new List<ISizeDependentControl>();
		}

        /// <summary>Gets the list of HTML element generators. </summary>
        public IDictionary<string, IControlGenerator> Generators { get { return _generators; } }

        /// <summary>Gets the list of size dependent controls. </summary>
        public List<ISizeDependentControl> SizeDependentControls { get; private set; }

		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.Register("Html", typeof(String), typeof(FixedHtmlTextBlock), new PropertyMetadata(default(String), 
                (obj, e) => ((FixedHtmlTextBlock) obj).Generate()));

        /// <summary>Gets or sets the HTML content to display. </summary>
        public String Html
		{
			get { return (String)GetValue(HtmlProperty); }
			set { SetValue(HtmlProperty, value); }
		}

        /// <summary>Occurs when the HTML content has been loaded. </summary>
		public event EventHandler<EventArgs> HtmlLoaded;

#if !WINRT
		public static readonly DependencyProperty ParagraphMarginProperty =
			DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(FixedHtmlTextBlock), new PropertyMetadata(6));
#else
        public static readonly DependencyProperty ParagraphMarginProperty =
            DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(FixedHtmlTextBlock), new PropertyMetadata(10));
#endif

        /// <summary>Gets or sets the margin for paragraphs (added at the bottom of the element). </summary>
		public int ParagraphMargin
		{
			get { return (int)GetValue(ParagraphMarginProperty); }
			set { SetValue(ParagraphMarginProperty, value); }
		}

		public static readonly DependencyProperty BaseUriProperty =
			DependencyProperty.Register("HtmlBaseUri", typeof(Uri), typeof(FixedHtmlTextBlock), new PropertyMetadata(default(Uri)));

        /// <summary>Gets or sets the base URI which is used to resolve relative URIs. </summary>
		public Uri HtmlBaseUri
		{
			get { return (Uri)GetValue(BaseUriProperty); }
			set { SetValue(BaseUriProperty, value); }
		}

        /// <summary>Calls the <see cref="HtmlLoaded"/> event. </summary>
        internal void CallHtmlLoaded()
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
}