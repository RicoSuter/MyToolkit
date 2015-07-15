//-----------------------------------------------------------------------
// <copyright file="EmGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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
    /// <summary>Generates the UI element for an EM HTML tag.</summary>
    public class EmGenerator : SingleControlGenerator
	{
        /// <summary>Creates a single UI element for the given HTML node and HTML view.</summary>
        /// <param name="node">The node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI element.</returns>
        public override DependencyObject GenerateSingle(HtmlNode node, IHtmlView htmlView)
		{
            foreach (var child in node.GetChildControls(htmlView))
			{
				var element = child as TextElement;
			    if (element != null)
			    {
#if WINRT
                    element.FontStyle = FontStyle.Italic;
#else
                    element.FontStyle = FontStyles.Italic;
#endif
                }
            }

			return null; 
		}
	}
}