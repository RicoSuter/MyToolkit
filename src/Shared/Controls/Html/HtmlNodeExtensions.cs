//-----------------------------------------------------------------------
// <copyright file="HtmlNodeExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MyToolkit.Html;
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Controls.Html
{
    internal static class HtmlNodeExtensions
    {
        public static void WrapWithHtmlTag(this HtmlNode node)
        {
            var pTag = new HtmlTagNode("p");
            foreach (var child in node.Children)
                pTag.AddChild(child);

            var htmlTag = new HtmlTagNode("html");
            htmlTag.AddChild(pTag);

            node.Children.Clear();
            node.AddChild(htmlTag);
        }
        
        public static DependencyObject[] GetChildControls(this HtmlNode node, IHtmlView textBlock)
        {
            var list = new List<DependencyObject>();
            foreach (var c in node.Children)
            {
                var ctrl = c.GetControls(textBlock);
                if (ctrl != null)
                    list.AddRange(ctrl);
                else
                    list.AddRange(c.GetChildControls(textBlock));
            }
            return list.ToArray();
        }

        public static DependencyObject[] GetControls(this HtmlNode node, IHtmlView textBlock)
        {
            if (node.Data == null)
            {
                var value = node is HtmlTextNode ? "text" : ((HtmlTagNode)node).Name;

                var generator = textBlock.Generators.ContainsKey(value) ? textBlock.Generators[value] :
                    (textBlock.Generators.ContainsKey("unknown") ? textBlock.Generators["unknown"] : null);

                if (generator != null)
                    node.Data = generator.CreateControls(node, textBlock);
            }

            return (DependencyObject[]) node.Data;
        }
    }
}