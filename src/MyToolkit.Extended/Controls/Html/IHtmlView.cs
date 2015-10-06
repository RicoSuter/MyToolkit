//-----------------------------------------------------------------------
// <copyright file="IHtmlView.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using System;
using System.Collections.Generic;

#if WINRT
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
#endif

namespace MyToolkit.Controls.Html
{
    /// <summary>Describes the interface of a HTML rendering control. </summary>
    public interface IHtmlView
    {
        /// <summary>Gets or sets the HTML content to display. </summary>
        string Html { get; }

        /// <summary>Gets the list of HTML element generators. </summary>
        IDictionary<string, IControlGenerator> Generators { get; }

        /// <summary>Gets the list of size dependent controls. </summary>
        List<ISizeDependentControl> SizeDependentControls { get; }

        /// <summary>Gets or sets the base URI which is used to resolve relative URIs. </summary>
        Uri HtmlBaseUri { get; }

        /// <summary>Gets or sets the margin for paragraphs (added at the bottom of the element). </summary>
        int ParagraphMargin { get; }

        /// <summary>Gets or sets a brush that describes the foreground color. </summary>
        Brush Foreground { get; }

        /// <summary>Gets or sets a brush that describes the background of a control. </summary>
        Brush Background { get; }

        /// <summary>Gets or sets the font size. </summary>
        double FontSize { get; }

        /// <summary>Gets or sets the font family of the control. </summary>
        FontFamily FontFamily { get; }

        /// <summary>Gets the rendered width of this element. </summary>
        double ActualWidth { get; }
    }
}

#endif