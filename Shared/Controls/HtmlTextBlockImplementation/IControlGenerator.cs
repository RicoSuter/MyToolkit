#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation
{
	public interface IControlGenerator
	{
		DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock);
	}
}