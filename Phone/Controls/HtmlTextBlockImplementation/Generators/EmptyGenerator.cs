using System.Windows;

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class EmptyGenerator : IControlGenerator
	{
		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			return new DependencyObject[] {};
		}
	}
}
