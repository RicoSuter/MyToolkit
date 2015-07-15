//-----------------------------------------------------------------------
// <copyright file="SingleControlGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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
        /// <param name="textBlock">The HTML view.</param>
        /// <returns>The UI elements.</returns>
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView textBlock)
		{
			var ctrl = GenerateSingle(node, textBlock);
			if (ctrl != null)
				return new [] { ctrl };
			return null; 
		}

        /// <summary>Creates a single UI element for the given HTML node and HTML view.</summary>
        /// <param name="node">The node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI element.</returns>
        public abstract DependencyObject GenerateSingle(HtmlNode node, IHtmlView htmlView);
	}
}