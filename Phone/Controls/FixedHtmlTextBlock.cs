using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MyToolkit.Controls.HtmlTextBlockSource;

namespace MyToolkit.Controls
{
	public class FixedHtmlTextBlock : ItemsControl, IHtmlTextBlock
	{
		public IDictionary<string, IControlGenerator> Generators { get { return generators; } }
		public List<ISizeChangedControl> SizeChangedControls { get; private set; }

		private readonly IDictionary<string, IControlGenerator> generators = HtmlParser.GetDefaultGenerators();
		
		public FixedHtmlTextBlock()
		{
			FontSize = (double)Resources["PhoneFontSizeNormal"];
			Margin = new Thickness(12, 0, 12, 0);

			SizeChanged += OnSizeChanged;
			SizeChangedControls = new List<ISizeChangedControl>();
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			foreach (var ctrl in SizeChangedControls)
				ctrl.Update(ActualWidth);
		}

		private static void OnHtmlChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var box = (FixedHtmlTextBlock)obj;
			box.Generate();
		}

		public void CallLoadedEvent()
		{
			var copy = HtmlLoaded;
			if (copy != null)
				copy(this, new EventArgs());
		}

		#region dependency properties

		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.Register("Html", typeof(String), typeof(FixedHtmlTextBlock), new PropertyMetadata(default(String), OnHtmlChanged));

		public String Html
		{
			get { return (String)GetValue(HtmlProperty); }
			set { SetValue(HtmlProperty, value); }
		}

		public event EventHandler<EventArgs> HtmlLoaded;

		public static readonly DependencyProperty ParagraphMarginProperty =
			DependencyProperty.Register("ParagraphMargin", typeof(int), typeof(FixedHtmlTextBlock), new PropertyMetadata(6));

		public int ParagraphMargin
		{
			get { return (int)GetValue(ParagraphMarginProperty); }
			set { SetValue(ParagraphMarginProperty, value); }
		}

		public static readonly DependencyProperty BaseUriProperty =
			DependencyProperty.Register("BaseUri", typeof(Uri), typeof(FixedHtmlTextBlock), new PropertyMetadata(default(Uri)));

		public Uri BaseUri
		{
			get { return (Uri)GetValue(BaseUriProperty); }
			set { SetValue(BaseUriProperty, value); }
		}

		#endregion
	}
}