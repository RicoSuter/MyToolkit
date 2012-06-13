using System.Windows;

namespace MyToolkit.Controls.HtmlTextBlockSource
{
	public abstract class SingleControlGenerator : IControlGenerator
	{
		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			var ctrl = GenerateSingle(node, textBlock);
			if (ctrl != null)
				return new [] { ctrl };
			return null; 
		}

		public abstract DependencyObject GenerateSingle(HtmlNode node, IHtmlTextBlock textBlock);
	}
}