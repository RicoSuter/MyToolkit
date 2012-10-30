using System.Collections.Generic;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class ParagraphGenerator : IControlGenerator
	{
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

		private static void CreateTextBox(List<DependencyObject> list, List<Inline> current, IHtmlTextBlock textBlock)
		{
			if (current.Count > 0)
			{
				var p = new Paragraph();
				foreach (var r in current)
					p.Inlines.Add(r);

#if !WINRT
				var tb = new RichTextBox();
				tb.Background = textBlock.Background;
#else
				var tb = new RichTextBlock();
				tb.IsTextSelectionEnabled = false;
#endif
				tb.Blocks.Add(p);
				tb.Foreground = textBlock.Foreground;
				tb.FontSize = textBlock.FontSize;
				tb.FontFamily = textBlock.FontFamily;
				tb.Margin = new Thickness(-12, textBlock.ParagraphMargin, -12, textBlock.ParagraphMargin);
				
				list.Add(tb);
				current.Clear();
			}
		}
	}
}