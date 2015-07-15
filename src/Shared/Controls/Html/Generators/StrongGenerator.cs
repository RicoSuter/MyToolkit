using System.Collections.Generic;
using System.Linq;
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
	public class StrongGenerator : IControlGenerator
    {
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView htmlView)
        {
            var controls = node.GetChildControls(htmlView); 
            foreach (var leave in controls)
            {
                var element = leave as TextElement;
                if (element != null)
                    element.FontWeight = FontWeights.Bold;
            }
            return controls;
        }
    }
}