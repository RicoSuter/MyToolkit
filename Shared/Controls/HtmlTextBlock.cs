using System;
using System.Collections.Generic;
using MyToolkit.Controls.HtmlTextBlockImplementation;

#if WINRT
using MyToolkit.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
using MyToolkit.UI;

#endif

namespace MyToolkit.Controls
{
	public class HtmlTextBlock : ExtendedListBox, IHtmlTextBlock
	{
		public IDictionary<string, IControlGenerator> Generators { get { return generators; } }
		public List<ISizeDependentControl> SizeDependentControls { get; private set; }

		private readonly IDictionary<string, IControlGenerator> generators = HtmlParser.GetDefaultGenerators();

		public HtmlTextBlock()
		{
#if !WINRT
			InnerMargin = new Thickness(24, 0, 24, 0);
			FontSize = (double)Resources["PhoneFontSizeNormal"];
#else
			
#endif
			Margin = new Thickness(0);

			SizeChanged += OnSizeChanged;
			SizeDependentControls = new List<ISizeDependentControl>();
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			foreach (var ctrl in SizeDependentControls)
				ctrl.Update(ActualWidth);
		}

		private static void OnHtmlChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var box = (HtmlTextBlock)obj;
			box.Generate();
		}

		public void CallLoadedEvent()
		{
			var copy = HtmlLoaded;
			if (copy != null)
				copy(this, new EventArgs());
		}

		private bool loaded = false;
#if !WINRT
		public override void OnApplyTemplate()
#else
		protected override void OnApplyTemplate()
#endif
		{
			base.OnApplyTemplate();

			loaded = true; 
			UpdateHeader();
			UpdateFooter();
		}

		#region dependency properties

		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.Register("Html", typeof(String), typeof(HtmlTextBlock), new PropertyMetadata(default(String), OnHtmlChanged));

		public String Html
		{
			get { return (String)GetValue(HtmlProperty); }
			set { SetValue(HtmlProperty, value); }
		}

		public event EventHandler<EventArgs> HtmlLoaded;

		public static readonly DependencyProperty ParagraphMarginProperty =
			DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(HtmlTextBlock), new PropertyMetadata(6));

		public int ParagraphMargin
		{
			get { return (int)GetValue(ParagraphMarginProperty); }
			set { SetValue(ParagraphMarginProperty, value); }
		}

		public static readonly DependencyProperty BaseUriProperty =
			DependencyProperty.Register("BaseUri", typeof(Uri), typeof(HtmlTextBlock), new PropertyMetadata(default(Uri)));

		public Uri BaseUri
		{
			get { return (Uri)GetValue(BaseUriProperty); }
			set { SetValue(BaseUriProperty, value); }
		}

		#endregion

		#region Templates

		private ContentPresenter headerPresenter;
		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HtmlTextBlock), new PropertyMetadata(default(DataTemplate), UpdateHeader));

		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty ShowHeaderProperty =
			DependencyProperty.Register("ShowHeader", typeof (bool), typeof (HtmlTextBlock), new PropertyMetadata(default(bool), UpdateShowHeader));

		private static void UpdateShowHeader(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (HtmlTextBlock)d;
			if (ctrl.headerPresenter != null)
				ctrl.headerPresenter.Visibility = ctrl.ShowHeader ? Visibility.Visible : Visibility.Collapsed;
		}

		public bool ShowHeader
		{
			get { return (bool) GetValue(ShowHeaderProperty); }
			set { SetValue(ShowHeaderProperty, value); }
		}

		private static void UpdateHeader(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (HtmlTextBlock)d;
			ctrl.UpdateHeader();
		}

		internal void UpdateHeader()
		{
			if (!loaded)
				return; 

			if (HeaderTemplate == null)
			{
				if (headerPresenter != null)
				{
					if (Items.Contains(headerPresenter))
						Items.Remove(headerPresenter);
					headerPresenter = null;
				}
			}
			else
			{
				if (headerPresenter == null)
				{
					headerPresenter = new ContentPresenter();
					headerPresenter.Content = this.FindParentDataContext();
					headerPresenter.Visibility = ShowHeader ? Visibility.Visible : Visibility.Collapsed;
				}

				if (!Items.Contains(headerPresenter))
					Items.Insert(0, headerPresenter);

				headerPresenter.ContentTemplate = HeaderTemplate;
			}
		}

		private ContentPresenter footerPresenter;
		public static readonly DependencyProperty FooterTemplateProperty =
			DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(HtmlTextBlock), new PropertyMetadata(default(DataTemplate), UpdateFooter));

		public DataTemplate FooterTemplate
		{
			get { return (DataTemplate)GetValue(FooterTemplateProperty); }
			set { SetValue(FooterTemplateProperty, value); }
		}

		public static readonly DependencyProperty ShowFooterProperty =
			DependencyProperty.Register("ShowFooter", typeof (bool), typeof (HtmlTextBlock), new PropertyMetadata(default(bool), UpdateShowFooter));

		private static void UpdateShowFooter(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (HtmlTextBlock)d;
			if (ctrl.footerPresenter != null)
				ctrl.footerPresenter.Visibility = ctrl.ShowFooter ? Visibility.Visible : Visibility.Collapsed;
		}

		public bool ShowFooter
		{
			get { return (bool) GetValue(ShowFooterProperty); }
			set { SetValue(ShowFooterProperty, value); }
		}

		private static void UpdateFooter(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (HtmlTextBlock)d;
			ctrl.UpdateFooter();
		}

		internal void UpdateFooter()
		{
			if (!loaded)
				return; 

			if (FooterTemplate == null)
			{
				if (footerPresenter != null)
				{
					if (Items.Contains(footerPresenter))
						Items.Remove(footerPresenter);
					footerPresenter = null;
				}
			}
			else
			{
				if (footerPresenter == null)
				{
					footerPresenter = new ContentPresenter();
					headerPresenter.Content = this.FindParentDataContext();
					footerPresenter.Visibility = ShowFooter ? Visibility.Visible : Visibility.Collapsed;
				}

				if (!Items.Contains(footerPresenter))
					Items.Add(footerPresenter);

				footerPresenter.ContentTemplate = FooterTemplate;
			}
		}

		#endregion
	}
}
