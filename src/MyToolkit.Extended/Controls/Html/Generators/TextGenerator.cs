//-----------------------------------------------------------------------
// <copyright file="TextGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using MyToolkit.Html;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
#else
using System.Windows;
using System.Windows.Documents;
#endif

namespace MyToolkit.Controls.Html.Generators
{
    /// <summary>Generates the UI element for a HTML text node.</summary>
    public class TextGenerator : SingleControlGenerator
    {
        /// <summary>Creates a single UI element for the given HTML node and HTML view.</summary>
        /// <param name="node">The node.</param>
        /// <param name="htmlView">The text block.</param>
        /// <returns>The UI element.</returns>
        public override DependencyObject CreateControl(HtmlNode node, IHtmlView htmlView)
        {
            return new Run { Text = ((HtmlTextNode)node).Text };
        }
    }
}

#endif