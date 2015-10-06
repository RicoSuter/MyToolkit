//-----------------------------------------------------------------------
// <copyright file="HtmlTagNode.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MyToolkit.Html
{
    /// <summary>Represents an XML tag node.</summary>
    public class HtmlTagNode : HtmlNode
    {
        /// <summary>Gets the tag name.</summary>
        public string Name { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="HtmlTagNode"/> class.</summary>
        /// <param name="html">The HTML.</param>
        public HtmlTagNode(string html)
        {
            var index = html.IndexOf(' ');
            if (index > 0)
            {
                Name = html.Substring(0, index).ToLower();
                try
                {
                    ParseAttributes(html.Substring(index));
                }
                catch
                {

                }
            }
            else
                Name = html.ToLower();
        }

        private void ParseAttributes(string data)
        {
            var matches = Regex.Matches(data, "(.*?)='(.*?)'|(.*?)=\"(.*?)\"");
            foreach (Match m in matches)
            {
                var key = m.Groups[1].Value.Trim(' ', '\r', '\n');
                var val = m.Groups[2].Value.Trim(' ', '\r', '\n');
                if (!string.IsNullOrEmpty(key))
                    Attributes.Add(key.ToLower(), val);
                else
                {
                    key = m.Groups[3].Value.Trim(' ', '\r', '\n');
                    val = m.Groups[4].Value.Trim(' ', '\r', '\n');
                    if (!string.IsNullOrEmpty(key))
                        Attributes.Add(key.ToLower(), val);
                }
            }
        }
    }
}