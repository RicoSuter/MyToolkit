//-----------------------------------------------------------------------
// <copyright file="EmGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

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
    public class EmGenerator : IControlGenerator
    {
        /// <summary>Creates the UI elements for the given HTML node and HTML view.</summary>
        /// <param name="node">The HTML node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI elements.</returns>
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView htmlView)
        {
            var controls = node.GetChildControls(htmlView);
            foreach (var leave in controls)
            {
                var element = leave as TextElement;
                if (element != null)
                {
#if WINRT
                    element.FontStyle = FontStyle.Italic;
#else
                    element.FontStyle = FontStyles.Italic;
#endif
                }
            }
            return controls;
        }
    }
}

#endif