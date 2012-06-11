using System.Windows;

namespace MyToolkit.Controls.HtmlTextBlockSource
{
	public abstract class SingleGenerator : IControlGenerator
	{
		public DependencyObject[] Generate(HtmlNode node, IHtmlSettings settings)
		{
			var ctrl = GenerateSingle(node, settings);
			if (ctrl != null)
				return new [] { ctrl };
			return null; 
		}

		public abstract DependencyObject GenerateSingle(HtmlNode node, IHtmlSettings settings);
	}
}