using System.Windows;
using System.Windows.Documents;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class TextGenerator : SingleGenerator
	{
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlSettings settings)
		{
			return new Run { Text = node.Value };
		}
	}
}