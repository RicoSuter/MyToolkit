using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MyToolkit.Controls.HtmlTextBlockSource;

namespace MyToolkit.Controls
{
	public class HtmlTextBlock : ExtendedListBox, IHtmlSettings, IHtmlTextBlock
	{
		public HtmlTextBlock()
		{
			FontSize = (double)Resources["PhoneFontSizeNormal"];
			Margin = new Thickness(0);
			InnerMargin = new Thickness(24, 0, 24, 0);

			SizeChanged += OnSizeChanged;
			SizeChangedControls = new List<ISizeChangedControl>();
		}

		public List<ISizeChangedControl> SizeChangedControls { get; private set; }

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			foreach (var ctrl in SizeChangedControls)
				ctrl.Update(ActualWidth);
		}

		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.Register("Html", typeof(String), typeof(HtmlTextBlock), new PropertyMetadata(default(String), OnChanged));

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

		private readonly IDictionary<string, IControlGenerator> generators = HtmlParser.GetDefaultGenerators();
		public IDictionary<string, IControlGenerator> Generators
		{
			get { return generators; }
		}

		private static void OnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var box = (HtmlTextBlock)obj;
			var html = box.Html;
			box.Generate(html);
		}

		private UIElement headerItem;
		public UIElement HeaderItem
		{
			get { return headerItem; }
			set
			{
				if (headerItem != value)
				{
					var old = headerItem;
					headerItem = value;
					this.UpdateHeaderControl(headerItem, old);
				}
			}
		}

		public void CallLoadedEvent()
		{
			var copy = HtmlLoaded;
			if (copy != null)
				copy(this, new EventArgs());
		}
	}
}
