//-----------------------------------------------------------------------
// <copyright file="ParagraphGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using System.Collections.Generic;
using System.Linq;
using MyToolkit.Html;
#if WINRT
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
#endif

namespace MyToolkit.Controls.Html.Generators
{
    /// <summary>Generates the UI element for a paragraph (p) HTML element.</summary>
    public class ParagraphGenerator : IControlGenerator
    {
        /// <summary>Gets or sets the size of the font in percent of the HTML view's font size (default: 1.0 = 100%).</summary>
        public double FontSize { get; set; }

        /// <summary>Gets or sets the font family.</summary>
        public FontFamily FontFamily { get; set; }

        /// <summary>Gets or sets the foreground.</summary>
        public Brush Foreground { get; set; }

        /// <summary>Gets or sets the font style.</summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>Gets or sets the font weight.</summary>
        public FontWeight FontWeight { get; set; }

        /// <summary>Gets or sets a value indicating whether the text should be split up in multiple RichTextBlocks to avoid the 2048px element size limit (default: true).</summary>
        public bool UseTextSplitting { get; set; }

        /// <summary>Initializes a new instance of the <see cref="ParagraphGenerator"/> class.</summary>
        public ParagraphGenerator()
        {
#if WINRT
            FontStyle = FontStyle.Normal;
            FontWeight = FontWeights.Normal; 
#else
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
#endif
            FontSize = 1.0;
            UseTextSplitting = true; 
        }

        /// <summary>Creates the UI elements for the given HTML node and HTML view.</summary>
        /// <param name="node">The HTML node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI elements.</returns>
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView htmlView)
        {
            var list = new List<DependencyObject>();

            var isFirst = true; 
            var addTopMargin = true; 
            var current = new List<Inline>();

            foreach (var child in node.GetChildControls(htmlView))
            {
                if (isFirst)
                {
                    if (child is Run)
                    {
                        var run = (Run)child;
                        run.Text = run.Text.TrimStart(' ', '\t'); // TODO: Trim all white spaces
                    }
                    isFirst = false;
                }

                if (child is Run && UseTextSplitting && ((Run)child).Text.Contains("\n")) // used to avoid 2048px max control size
                {
                    // split text
                    var run = (Run) child;
                    var splits = run.Text.Split('\n');

                    // join some splits to avoid small junks 
                    var currentSplit = "";
                    var newSplits = new List<string>();
                    for (var i = 0; i < splits.Length; i++)
                    {
                        var split = splits[i];
                        if (i != 0 && currentSplit.Length + split.Length > 16)
                        {
                            newSplits.Add(currentSplit);
                            currentSplit = split;
                        }
                        else
                            currentSplit += (i != 0 ? "\n" : "") + split;
                    }
                    newSplits.Add(currentSplit);

                    // create multiple text blocks
                    splits = newSplits.ToArray();
                    for (var i = 0; i < splits.Length; i++)
                    {
                        var split = splits[i];
                        current.Add(new Run { Text = split });
                        if (i < splits.Length - 1) // dont create for last
                            CreateTextBox(list, current, htmlView, i == 0 && addTopMargin, false);
                    }
                    addTopMargin = list.Count == 0; 
                } else if (child is Inline)
                    current.Add((Inline)child);
                else
                {
                    CreateTextBox(list, current, htmlView, addTopMargin, true);
                    list.Add(child);
                    addTopMargin = true; 
                }
            }

            CreateTextBox(list, current, htmlView, addTopMargin, true);

            if (list.Count == 0)
                return null;

            return list.ToArray();
        }

        private void CreateTextBox(List<DependencyObject> list, List<Inline> current, IHtmlView htmlView, bool addTopMargin, bool addBottomMargin)
        {
            if (current.Count > 0)
            {
                var p = new Paragraph();
                foreach (var r in current)
                    p.Inlines.Add(r);

#if !WINRT
                var textBlock = new RichTextBox();
                textBlock.Background = textBlock.Background;
                textBlock.Margin = new Thickness(-12, addTopMargin ? htmlView.ParagraphMargin : 0, -12, addBottomMargin ? htmlView.ParagraphMargin : 0);
#else
                var textBlock = new RichTextBlock();
                textBlock.IsTextSelectionEnabled = false; 
                textBlock.Margin = new Thickness(0, addTopMargin ? htmlView.ParagraphMargin : 0, 0, addBottomMargin ? htmlView.ParagraphMargin : 0);
#endif

                textBlock.Blocks.Add(p);
                textBlock.FontSize = htmlView.FontSize * FontSize;
                textBlock.Foreground = Foreground ?? htmlView.Foreground;
                textBlock.FontFamily = FontFamily ?? htmlView.FontFamily;
                textBlock.FontStyle = FontStyle;
                textBlock.FontWeight = FontWeight;
                
                list.Add(textBlock);
                current.Clear();
            }
        }
    }
}

#endif