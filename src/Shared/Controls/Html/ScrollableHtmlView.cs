//-----------------------------------------------------------------------
// <copyright file="HtmlTextBlock.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyToolkit.Controls.Html;
using MyToolkit.Mvvm;
using MyToolkit.Utilities;
#if WINRT
using MyToolkit.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using MyToolkit.UI;
#endif

namespace MyToolkit.Controls
{
    /// <summary>Renders HTML using native XAML controls in a scrollbar; use the control <see cref="HtmlView"/> 
    /// to render the HTML content without a <see cref="ScrollViewer"/>. </summary>
    public class ScrollableHtmlView : ScrollableItemsControl, IHtmlView
    {
        private bool _isLoaded = false;

        private ContentPresenter _headerPresenter;
        private ContentPresenter _footerPresenter;

        private readonly IDictionary<string, IControlGenerator> _generators;

        /// <summary>Initializes a new instance of the <see cref="ScrollableHtmlView"/> class. </summary>
        public ScrollableHtmlView()
        {
            _generators = HtmlViewHelper.GetDefaultGenerators(this);

#if WINDOWS_UAP

            InnerMargin = new Thickness(0, 0, 12, 0);

            FontSize = (double)Resources["ControlContentThemeFontSize"];
            Foreground = (Brush)Resources["ApplicationForegroundThemeBrush"];

#elif WINRT

            InnerMargin = new Thickness(0, 0, 20, 0);

            FontSize = (double)Resources["ControlContentThemeFontSize"];
            Foreground = (Brush)Resources["ApplicationForegroundThemeBrush"];

#else

			InnerMargin = new Thickness(24, 0, 24, 0);

			if (Resources.Contains("PhoneFontSizeNormal"))
				FontSize = (double)Resources["PhoneFontSizeNormal"];

            if (Resources.Contains("PhoneForegroundBrush"))
                Foreground = (Brush)Resources["PhoneForegroundBrush"];

#endif

            Margin = new Thickness(0);

            SizeChanged += OnSizeChanged;
            SizeDependentControls = new List<ISizeDependentControl>();

            DependencyPropertyChangedEvent.Register(this, FontSizeProperty, delegate { this.Generate(); });
            DependencyPropertyChangedEvent.Register(this, FontFamilyProperty, delegate { this.Generate(); });
            DependencyPropertyChangedEvent.Register(this, ForegroundProperty, delegate { this.Generate(); });
            DependencyPropertyChangedEvent.Register(this, BackgroundProperty, delegate { this.Generate(); });
            DependencyPropertyChangedEvent.Register(this, InnerMaxWidthProperty, delegate { this.Generate(); });
        }

        /// <summary>Gets the list of HTML element generators. </summary>
        public IDictionary<string, IControlGenerator> Generators
        {
            get { return _generators; }
        }

        /// <summary>Gets the list of size dependent controls. </summary>
        public List<ISizeDependentControl> SizeDependentControls { get; private set; }

        /// <summary>Occurs when the HTML content has been loaded. </summary>
        public event EventHandler<EventArgs> HtmlLoaded;

        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.Register("Html", typeof(string), typeof(ScrollableHtmlView), new PropertyMetadata(default(string),
                (obj, e) => ((ScrollableHtmlView)obj).Generate()));

        /// <summary>Gets or sets the HTML content to display. </summary>
        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

#if !WINRT
		public static readonly DependencyProperty ParagraphMarginProperty =
			DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(ScrollableHtmlView), new PropertyMetadata(6));
#else
#if WINDOWS_UAP
        public static readonly DependencyProperty ParagraphMarginProperty =
            DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(ScrollableHtmlView), new PropertyMetadata(6));
#else
        public static readonly DependencyProperty ParagraphMarginProperty =
            DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(ScrollableHtmlView), new PropertyMetadata(10));
#endif
#endif

        /// <summary>Gets or sets the margin for paragraphs (added at the bottom of the element). </summary>
        public int ParagraphMargin
        {
            get { return (int)GetValue(ParagraphMarginProperty); }
            set { SetValue(ParagraphMarginProperty, value); }
        }

        public static readonly DependencyProperty HtmlBaseUriProperty =
            DependencyProperty.Register("HtmlBaseUri", typeof(Uri), typeof(ScrollableHtmlView), new PropertyMetadata(default(Uri)));

        /// <summary>Gets or sets the base URI which is used to resolve relative URIs. </summary>
        public Uri HtmlBaseUri
        {
            get { return (Uri)GetValue(HtmlBaseUriProperty); }
            set { SetValue(HtmlBaseUriProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ScrollableHtmlView), new PropertyMetadata(default(DataTemplate),
                (d, e) => ((ScrollableHtmlView)d).UpdateHeader()));

        /// <summary>Gets or sets the header template. </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(ScrollableHtmlView),
            new PropertyMetadata(true, (d, e) => ((ScrollableHtmlView)d).UpdateShowHeader()));

        /// <summary>Gets or sets a value indicating whether the header should be shown. </summary>
        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public static readonly DependencyProperty FooterTemplateProperty =
            DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(ScrollableHtmlView),
            new PropertyMetadata(default(DataTemplate), (d, e) => ((ScrollableHtmlView)d).UpdateFooter()));

        /// <summary>Gets or sets the footer template. </summary>
        public DataTemplate FooterTemplate
        {
            get { return (DataTemplate)GetValue(FooterTemplateProperty); }
            set { SetValue(FooterTemplateProperty, value); }
        }

        public static readonly DependencyProperty InnerMaxWidthProperty = DependencyProperty.Register(
            "InnerMaxWidth", typeof (double), typeof (ScrollableHtmlView), new PropertyMetadata(default(double)));

        /// <summary>Gets or sets the maximum width of the elements in the scroll viewer.</summary>
        public double InnerMaxWidth
        {
            get { return (double) GetValue(InnerMaxWidthProperty); }
            set { SetValue(InnerMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty ShowFooterProperty =
            DependencyProperty.Register("ShowFooter", typeof(bool), typeof(ScrollableHtmlView),
            new PropertyMetadata(true, (d, e) => ((ScrollableHtmlView)d).UpdateShowFooter()));

        /// <summary>Gets or sets a value indicating whether the footer should be shown. </summary>
        public bool ShowFooter
        {
            get { return (bool)GetValue(ShowFooterProperty); }
            set { SetValue(ShowFooterProperty, value); }
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

#if WINRT
        /// <summary>Attaches a binding to a FrameworkElement, using the provided binding object.</summary>
        protected override void OnApplyTemplate()
#else
        /// <summary>Attaches a binding to a FrameworkElement, using the provided binding object.</summary>
		public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            _isLoaded = true;

            UpdateHeader();
            UpdateFooter();
        }

        internal void UpdateHeader()
        {
            if (!_isLoaded)
                return;

            if (HeaderTemplate == null)
            {
                if (_headerPresenter != null)
                {
                    if (Items != null && Items.Contains(_headerPresenter))
                        Items.Remove(_headerPresenter);

                    _headerPresenter = null;
                }
            }
            else
            {
                if (_headerPresenter == null)
                {
                    _headerPresenter = new ContentPresenter();
                    _headerPresenter.Content = this.FindParentDataContext();
                    _headerPresenter.Visibility = ShowHeader ? Visibility.Visible : Visibility.Collapsed;
                }

                if (Items != null && !Items.Contains(_headerPresenter))
                    Items.Insert(0, _headerPresenter);

                _headerPresenter.ContentTemplate = HeaderTemplate;
            }
        }

        internal void UpdateFooter()
        {
            if (!_isLoaded)
                return;

            if (FooterTemplate == null)
            {
                if (_footerPresenter != null)
                {
                    if (Items != null && Items.Contains(_footerPresenter))
                        Items.Remove(_footerPresenter);
                    _footerPresenter = null;
                }
            }
            else
            {
                if (_footerPresenter == null)
                {
                    _footerPresenter = new ContentPresenter();
                    _footerPresenter.Content = this.FindParentDataContext();
                    _footerPresenter.Visibility = ShowFooter ? Visibility.Visible : Visibility.Collapsed;
                }

                if (Items != null && !Items.Contains(_footerPresenter))
                    Items.Add(_footerPresenter);

                _footerPresenter.ContentTemplate = FooterTemplate;
            }
        }

        private void UpdateShowHeader()
        {
            if (_headerPresenter != null)
                _headerPresenter.Visibility = ShowHeader && HeaderTemplate != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateShowFooter()
        {
            if (_footerPresenter != null)
                _footerPresenter.Visibility = ShowFooter && FooterTemplate != null ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void OnHtmlLoaded()
        {
            var copy = HtmlLoaded;
            if (copy != null)
                copy(this, new EventArgs());
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            foreach (var control in SizeDependentControls)
                control.Update(ActualWidth);
        }
    }

    [Obsolete("Use ScrollableHtmlView instead. 7/14/2015")]
    public class HtmlTextBlock : ScrollableHtmlView
    {
    }
}
