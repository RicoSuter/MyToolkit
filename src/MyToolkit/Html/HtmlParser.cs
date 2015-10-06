//-----------------------------------------------------------------------
// <copyright file="HtmlParser.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MyToolkit.Utilities;

namespace MyToolkit.Html
{
    /// <summary>An HTML parser implementation.</summary>
    public class HtmlParser
    {
        /// <summary>Gets or sets a value indicating whether malformed HTML is ignored (default: true).</summary>
        public bool IgnoreMalformedHtml { get; set; }

        /// <summary>Initializes a new instance of the <see cref="HtmlParser"/> class.</summary>
        public HtmlParser()
        {
            IgnoreMalformedHtml = true;
        }

        /// <summary>Parses the HTML to an <see cref="HtmlNode"/>.</summary>
        /// <param name="html">The HTML.</param>
        /// <returns>The HTML node.</returns>
        /// <exception cref="InvalidOperationException">The HTML is malformed.</exception>
        public HtmlNode Parse(string html)
        {
            html = CleanUpHtml(html);

            var matches = Regex.Matches(html, "<(.*?)>|</(.*?)>|<(.*?)/>|([^<>]*)");
            var tokens = matches
                .OfType<Group>()
                .Select(m => m.Value)
                .ToArray();

            var root = new HtmlTagNode("html");
            var stack = new Stack<HtmlNode>();
            stack.Push(root);

            foreach (var token in tokens)
            {
                if (token.StartsWith("</") && token.EndsWith(">")) // end tag
                {
                    var tag = token.Substring(2, token.Length - 3);
                    var node = stack.Peek();
                    if (node is HtmlTagNode && ((HtmlTagNode)node).Name == tag)
                        stack.Pop();
                    else if (!IgnoreMalformedHtml)
                      throw new InvalidOperationException("The HTML is malformed at token '<"+ token + ">'.");
                }
                else if (token.StartsWith("<") && token.EndsWith("/>")) // full tag
                {
                    var value = token.Substring(1, token.Length - 3);
                    stack.Peek().AddChild(new HtmlTagNode(value));
                }
                else if (token.StartsWith("<") && token.EndsWith(">")) // start tag
                {
                    var value = token.Substring(1, token.Length - 2);
                    var node = new HtmlTagNode(value);
                    stack.Peek().AddChild(node);
                    stack.Push(node);
                }
                else if (!string.IsNullOrEmpty(token)) // text
                    stack.Peek().AddChild(new HtmlTextNode(token));
            }

            return root;
        }

        private string CleanUpHtml(string html)
        {
            if (html == null)
                html = "";

            html = html.RemoveHtmlWhitespaces();
            html = html.RemoveHtmlComments();
            html = html.ConvertUtf8Characters();
            html = html.ConvertHtmlCharacters();

            html = html
                .Replace("<br>", "\n")
                .Replace("<br />", "\n")
                .Replace("<br/>", "\n")
                .Replace("<BR>", "\n")
                .Replace("<BR />", "\n")
                .Replace("<BR/>", "\n");

            html = html.Trim('\n', '\r', ' ');
            html = "<p>" + html + "</p>";
            return html;
        }
    }
}
