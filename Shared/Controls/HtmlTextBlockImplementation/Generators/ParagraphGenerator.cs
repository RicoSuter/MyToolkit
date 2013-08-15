using System.Collections.Generic;

#if WINRT
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class ParagraphGenerator : IControlGenerator
	{
		public double FontSize { get; set; }
		public FontFamily FontFamily { get; set; }
		public Brush Foreground { get; set; }
		public FontStyle FontStyle { get; set; }

		public ParagraphGenerator()
		{
#if WINRT
			FontStyle = FontStyle.Normal;
#else
			FontStyle = FontStyles.Normal;
#endif
		}

		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			var list = new List<DependencyObject>();

			var current = new List<Inline>();
			foreach (var c in node.GetLeaves(textBlock))
			{
				if (c is Inline)
					current.Add((Inline)c);
				else
				{
					CreateTextBox(list, current, textBlock);
					list.Add(c);
				}
			}
			CreateTextBox(list, current, textBlock);

			if (list.Count == 0)
				return null;
			return list.ToArray();
		}

		private void CreateTextBox(List<DependencyObject> list, List<Inline> current, IHtmlTextBlock textBlock)
		{
			if (current.Count > 0)
			{
				var p = new Paragraph();
				foreach (var r in current)
					p.Inlines.Add(r);

#if !WINRT
				var tb = new RichTextBox();
				tb.Background = textBlock.Background;
				tb.Margin = new Thickness(-12, textBlock.ParagraphMargin, -12, textBlock.ParagraphMargin);
#else
				var tb = new RichTextBlock();
				tb.IsTextSelectionEnabled = false;
				tb.Margin = new Thickness(0, textBlock.ParagraphMargin, 0, textBlock.ParagraphMargin);
#endif
				tb.Blocks.Add(p);
				tb.Foreground = Foreground ?? textBlock.Foreground;
				tb.FontSize = FontSize == 0 ? textBlock.FontSize : FontSize;
				tb.FontFamily = FontFamily ?? textBlock.FontFamily;
				tb.FontStyle = FontStyle;
				
				list.Add(tb);
				current.Clear();
			}
		}
	}
}