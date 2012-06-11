using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class ParagraphGenerator : IControlGenerator
	{
		public DependencyObject[] Generate(HtmlNode node, IHtmlSettings settings)
		{
			var list = new List<DependencyObject>();

			var current = new List<Inline>();
			foreach (var c in node.GetLeaves(settings))
			{
				if (c is Inline)
					current.Add((Inline)c);
				else
				{
					CreateTextBox(list, current, settings);
					list.Add(c);
				}
			}
			CreateTextBox(list, current, settings);

			if (list.Count == 0)
				return null;
			return list.ToArray();
		}

		private static void CreateTextBox(List<DependencyObject> list, List<Inline> current, IHtmlSettings settings)
		{
			if (current.Count > 0)
			{
				var p = new Paragraph();
				foreach (var r in current)
					p.Inlines.Add(r);

				var tb = new RichTextBox();
				tb.Blocks.Add(p);
				tb.FontSize = settings.FontSize;
				tb.FontFamily = settings.FontFamily;
				tb.Margin = new Thickness(-12, settings.ParagraphMargin, -12, settings.ParagraphMargin);
				
				list.Add(tb);
				current.Clear();
			}
		}
	}
}