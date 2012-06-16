using System.Windows;

namespace MyToolkit.Controls.HtmlTextBlockImplementation
{
	public interface IControlGenerator
	{
		DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock);
	}
}