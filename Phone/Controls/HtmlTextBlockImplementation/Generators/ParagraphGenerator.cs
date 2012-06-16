using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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

				var tb = new RichTextBox();
				tb.Blocks.Add(p);
				tb.FontSize = textBlock.FontSize;
				tb.FontFamily = textBlock.FontFamily;
				tb.Margin = new Thickness(-12, textBlock.ParagraphMargin, -12, textBlock.ParagraphMargin);
				
				list.Add(tb);
				current.Clear();
			}
		}
	}
}