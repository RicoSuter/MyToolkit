using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class ParagraphGenerator : IControlGenerator
	{
		public DependencyObject Generate(HtmlNode node, IHtmlSettings settings)
		{
			var list = new List<UIElement>();

			var current = new List<Inline>();
			foreach (var c in node.GetLeaves(settings))
			{
				if (c is Inline)
					current.Add((Inline)c);
				else
				{
					CreateTextBox(list, current, settings);
					list.Add((UIElement)c);
				}
			}
			CreateTextBox(list, current, settings);

			if (list.Count == 1)
				return list.First();

			var panel = new StackPanel();
			foreach (var c in list)
				panel.Children.Add(c);
			return panel;
		}

		private static void CreateTextBox(List<UIElement> list, List<Inline> current, IHtmlSettings settings)
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
				tb.Margin = new Thickness(-12, settings.ParagraphMargin / 2, -12, settings.ParagraphMargin / 2);
				
				list.Add(tb);
				current.Clear();
			}
		}
	}
}