#if METRO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

#else
using System.Windows;
using System.Windows.Documents;
#endif
namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class TextGenerator : SingleControlGenerator
	{
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlTextBlock textBlock)
		{
			return new Run { Text = node.Value };
		}
	}
}