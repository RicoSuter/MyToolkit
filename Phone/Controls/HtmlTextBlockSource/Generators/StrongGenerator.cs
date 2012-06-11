using System.Windows;
using System.Windows.Documents;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class StrongGenerator : SingleGenerator
	{
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlSettings settings)
		{
			foreach (var c in node.GetLeaves(settings))
			{
				var element = c as TextElement;
				if (element != null)
					element.FontWeight = FontWeights.Bold;
			}
			return null;
		}
	}
}