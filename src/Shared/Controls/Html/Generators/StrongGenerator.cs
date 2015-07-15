using MyToolkit.Html;
#if WINRT
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
#else
using System.Windows;
using System.Windows.Documents;
#endif

namespace MyToolkit.Controls.Html.Generators
{
    /// <summary>Generates the UI element for a strong/b element.</summary>
	public class StrongGenerator : SingleControlGenerator
	{
        /// <summary>Creates a single UI element for the given HTML node and HTML view.</summary>
        /// <param name="node">The node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI element.</returns>
        public override DependencyObject GenerateSingle(HtmlNode node, IHtmlView htmlView)
		{
            foreach (var leave in node.GetChildControls(htmlView))
			{
				var element = leave as TextElement;
				if (element != null)
					element.FontWeight = FontWeights.Bold;
			}
			return null;
		}
	}
}