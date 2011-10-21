using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;

using MyToolkit.MVVM;

namespace MyToolkit.UI
{
	public partial class RichTextBlock : UserControl
	{
		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.Register("Html", typeof(String), typeof(RichTextBlock), new PropertyMetadata(default(String), OnHtmlChanged));

		private static void OnHtmlChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var box = (RichTextBlock)obj;
			var html = (string)e.NewValue;
			box.Html = html;
			box.Generate(html);
		}

		public String Html
		{
			get { return (String) GetValue(HtmlProperty); }
			set { SetValue(HtmlProperty, value); }
		}

		private int paragraphMargin = 12; 
		public int ParagraphMargin
		{
			get { return paragraphMargin; }
			set 
			{ 
				paragraphMargin = value;
				Generate(Html);
			}
		}

		public RichTextBlock()
		{
			InitializeComponent();
		}

		private void Generate(string html)
		{
			html = html.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim(' ');
			if (!html.StartsWith("<p>", StringComparison.CurrentCultureIgnoreCase))
				html = "<p>" + html;
			if (!html.EndsWith("</p>", StringComparison.CurrentCultureIgnoreCase))
				html = html + "</p>";

			var stack = LayoutRoot;
			stack.Children.Clear();

			var matches = Regex.Matches(html, "<p>(.*?)</p>");
			for (var i = 0; i < matches.Count; i++ )
			{
				var p = matches[i];
				var text = p.Groups[1].Value;
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
				box.Margin = new Thickness(0, 0, 0, i == matches.Count - 1 ? 0 : ParagraphMargin);
				box.Blocks.Add(para);
				stack.Children.Add(box);
			}
		}

		public virtual Hyperlink CreateHyperlink(string label, string link)
		{
			var hr = new Hyperlink();
			hr.Command = new RelayCommand(() => new WebBrowserTask {Uri = new Uri(link)}.Show());
			hr.TextDecorations = TextDecorations.Underline;
			hr.Inlines.Add(label);
			return hr;
		}
	}
}
