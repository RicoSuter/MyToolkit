using System.Windows;
using System.Windows.Documents;

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class EmGenerator : SingleControlGenerator
	{
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlTextBlock textBlock)
		{
			foreach (var c in node.GetLeaves(textBlock))
			{
				var element = c as TextElement;
				if (element != null)
					element.FontStyle = FontStyles.Italic;
			}
			return null; 
		}
	}
}