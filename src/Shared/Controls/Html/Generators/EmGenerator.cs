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
    public class EmGenerator : IControlGenerator
	{
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