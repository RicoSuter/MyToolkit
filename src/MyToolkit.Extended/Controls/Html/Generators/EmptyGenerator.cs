//-----------------------------------------------------------------------
// <copyright file="EmptyGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using MyToolkit.Html;
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Controls.Html.Generators
{
    /// <summary>Generates no UI control.</summary>
    public class EmptyGenerator : IControlGenerator
    {
        /// <summary>Creates the UI elements for the given HTML node and HTML view.</summary>
        /// <param name="node">The HTML node.</param>
        /// <param name="textBlock">The HTML view.</param>
        /// <returns>The UI elements.</returns>
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView textBlock)
        {
            return new DependencyObject[] { };
        }
    }
}

#endif