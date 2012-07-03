#if METRO
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

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
