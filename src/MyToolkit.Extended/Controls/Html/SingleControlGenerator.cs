//-----------------------------------------------------------------------
// <copyright file="SingleControlGenerator.cs" company="MyToolkit">
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

namespace MyToolkit.Controls.Html
{
    /// <summary>Generator for a single UI element for a given HTML node.</summary>
    public abstract class SingleControlGenerator : IControlGenerator
    {
        /// <summary>Creates the UI elements for the given HTML node and HTML view.</summary>
        /// <param name="node">The HTML node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI elements.</returns>
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView htmlView)
        {
            var control = CreateControl(node, htmlView);
            if (control != null)
                return new [] { control };

            return new DependencyObject[] { };
        }

        /// <summary>Creates a single UI element for the given HTML node and HTML view.</summary>
        /// <param name="node">The node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI element.</returns>
        public abstract DependencyObject CreateControl(HtmlNode node, IHtmlView htmlView);
    }
}

#endif