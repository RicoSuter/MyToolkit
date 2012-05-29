using System.Windows;
using System.Windows.Documents;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class StrongGenerator : IControlGenerator
	{
		public DependencyObject Generate(HtmlNode node, IHtmlSettings settings)
		{
			foreach (var c in node.GetLeaves(settings))
			{
				var run = c as Run;
				if (run != null)
					run.FontWeight = FontWeights.Bold;
			}
			return null;
		}
	}
}