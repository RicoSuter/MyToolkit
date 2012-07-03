using System.Windows;

#if METRO
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
#else
using System.Windows.Documents;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class EmGenerator : SingleControlGenerator
	{
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlTextBlock textBlock)
		{
			foreach (var c in node.GetLeaves(textBlock))
			{
				var element = c as TextElement;
				if (element != null)
#if METRO
                    element.FontStyle = FontStyle.Italic;
#else
                    element.FontStyle = FontStyles.Italic;
#endif
			}
			return null; 
		}
	}
}