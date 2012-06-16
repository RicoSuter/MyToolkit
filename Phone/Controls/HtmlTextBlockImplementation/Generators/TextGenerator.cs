using System.Windows;
using System.Windows.Documents;

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class TextGenerator : SingleControlGenerator
	{
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlTextBlock textBlock)
		{
			return new Run { Text = node.Value };
		}
	}
}