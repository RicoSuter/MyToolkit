//-----------------------------------------------------------------------
// <copyright file="HtmlNode.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyToolkit.Html
{
    /// <summary>Represents an HTML tag or text node.</summary>
    public abstract class HtmlNode
    {
        /// <summary>Gets the HTML tag's attributes.</summary>
        public Dictionary<string, string> Attributes { get; private set; }

        /// <summary>Gets the child tags.</summary>
        public List<HtmlNode> Children { get; private set; }

        /// <summary>Gets or sets custom data.</summary>
        public object Data { get; set; }

        /// <summary>Initializes a new instance of the <see cref="HtmlNode"/> class.</summary>
        /// <param name="children">The children.</param>
        protected HtmlNode(List<HtmlNode> children = null)
        {
            Children = children == null ? new List<HtmlNode>() : new List<HtmlNode>(children);
            Attributes = new Dictionary<string, string>();
        }

        /// <summary>Adds a child node.</summary>
        /// <param name="node">The node to add.</param>
        public void AddChild(HtmlNode node)
        {
            Children.Add(node);
        }
    }
}