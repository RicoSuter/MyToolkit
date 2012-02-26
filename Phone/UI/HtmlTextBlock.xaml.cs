using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;

using MyToolkit.MVVM;

namespace MyToolkit.UI
{
	public class ImageBlock
	{
		public Image Image { get; set; }
		public int MaxWidth { get; set; }
		public int MaxHeight { get; set; }
	}

	public partial class HtmlTextBlock : UserControl
	{
		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.Register("Html", typeof(String), typeof(HtmlTextBlock), new PropertyMetadata(default(String), OnChanged));

		public String Html
		{
			get { return (String) GetValue(HtmlProperty); }
			set { SetValue(HtmlProperty, value); }
		}

		public static readonly DependencyProperty ParagraphMarginProperty =
			DependencyProperty.Register("ParagraphMargin", typeof (int), typeof (HtmlTextBlock), new PropertyMetadata(12));

		public int ParagraphMargin
		{
			get { return (int) GetValue(ParagraphMarginProperty); }
			set { SetValue(ParagraphMarginProperty, value); }
		}

		public static readonly DependencyProperty BaseUriProperty =
			DependencyProperty.Register("BaseUri", typeof (Uri), typeof (HtmlTextBlock), new PropertyMetadata(default(Uri)));

		public Uri BaseUri
		{
			get { return (Uri) GetValue(BaseUriProperty); }
			set { SetValue(BaseUriProperty, value); }
		}

		private static void OnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var box = (HtmlTextBlock)obj;
			var html = box.Html;
			box.Generate(html);
		}

		public HtmlTextBlock()
		{
			InitializeComponent();
			SizeChanged += OnSizeChanged;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			foreach (var img in images)
			{
				var width = img.MaxWidth;
				var height = img.MaxHeight;

				var maxWidth = (int)ActualWidth - 24;
				if (width == 0)
					width = maxWidth;
				if (width > maxWidth)
				{
					width = maxWidth;
					height = height * (width / maxWidth);
				}

				img.Image.Width = width;
				if (height > 0)
					img.Image.Height = height;
			}
		}

		private readonly List<ImageBlock> images = new List<ImageBlock>(); 
		private void Generate(string html)
		{
			images.Clear();

			html = html.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim(' ');
			if (!html.StartsWith("<p>", StringComparison.CurrentCultureIgnoreCase))
				html = "<p>" + html;
			if (!html.EndsWith("</p>", StringComparison.CurrentCultureIgnoreCase))
				html = html + "</p>";

			html = Regex.Replace(html, "<img (.*?)/>", match => "</p><p>" + match.Value + "</p><p>");

			var stack = LayoutRoot;
			var boxes = new List<UIElement>();
			var matches = Regex.Matches(html, "<p>(.*?)</p>");
			for (var i = 0; i < matches.Count; i++ )
			{
				var p = matches[i];
				var text = p.Groups[1].Value;

				var m = Regex.Match(text, "<img (.*?)/>");
				if (m.Success)
				{
					try
					{
						var match = Regex.Match(m.Groups[1].Value, "src=\"(.*?)\"");
						var uri = match.Groups[1].Value;

						var height = 0;
						match = Regex.Match(m.Groups[1].Value, "height=\"(.*?)\"");
						if (match.Success)
							height = int.Parse(match.Groups[1].Value);
						var width = 0;
						match = Regex.Match(m.Groups[1].Value, "width=\"(.*?)\"");
						if (match.Success)
							width = int.Parse(match.Groups[1].Value);

						var image = new Image();
						var imgSource = new BitmapImage(new Uri(uri));
						image.HorizontalAlignment = HorizontalAlignment.Left;
						image.Source = imgSource;
						image.Margin = new Thickness(12, -ParagraphMargin, 12, 0);

						images.Add(new ImageBlock { Image = image, MaxHeight = height, MaxWidth = width });
						boxes.Add(image);
					}
					catch
					{
						if (System.Diagnostics.Debugger.IsAttached)
							System.Diagnostics.Debugger.Break();
					}
				}
				else
				{
					try
					{
						var para = new Paragraph();
						text = Regex.Replace(text, "<a |</a>|<b>|</b>|<strong>|</strong>|<em>|</em>",
						s => s.Groups[0].Value[1] != '/' ? "\n" + s : s + "\n");

						foreach (var value in text.Split('\n'))
						{
							text = value.Replace("<br/>", "\n").Replace("<br>", "\n").Replace("<br />", "\n");
							text = Regex.Replace(text, "<[^>]*>", string.Empty);
							var markup = value.ToLower();

							if (markup.StartsWith("<b>") || markup.StartsWith("<strong>"))
								para.Inlines.Add(new Run { Text = text, FontWeight = FontWeights.Bold });
							else if (markup.StartsWith("<em>"))
								para.Inlines.Add(new Run { Text = text, FontStyle = FontStyles.Italic });
							else if (markup.StartsWith("<a"))
							{
								var match = Regex.Match(value, "<a href=\"(.*?)\"(.*?)>(.*?)</a>");
								var link = match.Groups[1].Value;
								var label = match.Groups[3].Value;
								para.Inlines.Add(CreateHyperlink(label, link));
							}
							else
								para.Inlines.Add(new Run { Text = text });
						}

						var box = new RichTextBox();
						box.FontSize = FontSize;
						box.FontFamily = FontFamily;
						box.FontStretch = FontStretch;
						box.FontStyle = FontStyle;
						box.FontWeight = FontWeight;

						box.Margin = new Thickness(0, 0, 0, i == matches.Count - 1 ? 0 : ParagraphMargin);
						box.Blocks.Add(para);
						boxes.Add(box);
					}
					catch
					{
						if (System.Diagnostics.Debugger.IsAttached)
							System.Diagnostics.Debugger.Break();
					}
				}
			}

			stack.Children.Clear();
			foreach (var b in boxes)
				stack.Children.Add(b);
		}

		public virtual Inline CreateHyperlink(string label, string link)
		{
			try
			{
				var hr = new Hyperlink();
				if (link.StartsWith("mailto:"))
					hr.Command = new RelayCommand(() => new EmailComposeTask { To = link.Substring(7) }.Show());
				else
				{
					var uri = link.StartsWith("http://") || link.StartsWith("https://") ?
						new Uri(link, UriKind.Absolute) : new Uri(BaseUri, link);
					hr.Command = new RelayCommand(() => new WebBrowserTask { Uri = uri }.Show());
				}

				hr.TextDecorations = TextDecorations.Underline;
				hr.Inlines.Add(label);
				return hr;
			}
			catch
			{
#if DEBUG
				throw;
#endif
				return new Run { Text = label };
			}
		}
	}
}
