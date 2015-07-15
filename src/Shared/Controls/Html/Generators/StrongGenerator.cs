//-----------------------------------------------------------------------
// <copyright file="StrongGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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
                    element.FontWeight = FontWeights.Bold;
            }
            return controls;
        }
    }
}