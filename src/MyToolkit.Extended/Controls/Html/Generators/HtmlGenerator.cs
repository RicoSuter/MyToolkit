//-----------------------------------------------------------------------
// <copyright file="HtmlGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using System.Collections.Generic;
using System.Linq;
using MyToolkit.Html;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace MyToolkit.Controls.Html.Generators
{
    /// <summary>Generates the UI element for the HTML tag.</summary>
    public class HtmlGenerator : IControlGenerator
    {
        /// <summary>Creates the UI elements for the given HTML node and HTML view.</summary>
        /// <param name="node">The HTML node.</param>
        /// <param name="textBlock">The HTML view.</param>
        /// <returns>The UI elements.</returns>
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView textBlock)
        {
            var leaves = node.GetChildControls(textBlock);
            if (leaves.Length == 1 && leaves[0] is Panel)
                leaves = new[] { leaves[0] };

            if (leaves.Length > 0)
            {
                var element = leaves.First() as FrameworkElement;
                if (element != null)
                    element.Margin = new Thickness(element.Margin.Left, 0, element.Margin.Right, element.Margin.Bottom);

                element = leaves.Last() as FrameworkElement;
                if (element != null)
                    element.Margin = new Thickness(element.Margin.Left, element.Margin.Top, element.Margin.Right, 0);
            }

            return leaves;
        }
    }
}

#endif