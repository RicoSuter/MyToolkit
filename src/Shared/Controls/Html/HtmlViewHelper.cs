//-----------------------------------------------------------------------
// <copyright file="HtmlTextBlockHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyToolkit.Controls.Html.Generators;
using MyToolkit.Html;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.System.Threading;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace MyToolkit.Controls.Html
{
    /// <summary>Common HTML view helper methods.</summary>
    public static class HtmlViewHelper
    {
        /// <summary>Creates the default UI element generators.</summary>
        /// <param name="view">The view.</param>
        /// <returns>The generators.</returns>
        public static Dictionary<string, IControlGenerator> GetDefaultGenerators(FrameworkElement view)
        {
            var list = new Dictionary<string, IControlGenerator>();
            list.Add("p", new ParagraphGenerator());
            list.Add("h1", new ParagraphGenerator { FontSize = 1.6 });
            list.Add("h2", new ParagraphGenerator { FontSize = 1.4 });
            list.Add("h3", new ParagraphGenerator { FontSize = 1.2 });

            list.Add("html", new HtmlGenerator());
            list.Add("strong", new StrongGenerator());
            list.Add("b", list["strong"]);
            list.Add("text", new TextGenerator());
            list.Add("em", new EmGenerator());
            list.Add("i", list["em"]);
            list.Add("a", new LinkGenerator());
            list.Add("img", new ImageGenerator());
            list.Add("ul", new UlGenerator());
            list.Add("script", new EmptyGenerator());

            return list;
        }

        internal async static void Generate(this IHtmlView htmlView)
        {
            var html = htmlView.Html;
            var itemsControl = (ItemsControl)htmlView;

            if (string.IsNullOrEmpty(html))
                itemsControl.Items.Clear();

            htmlView.SizeDependentControls.Clear();

            var scrollableHtmlView = htmlView as ScrollableHtmlView;
            if (scrollableHtmlView != null)
                scrollableHtmlView.UpdateHeader();

            if (string.IsNullOrEmpty(html))
            {
                if (scrollableHtmlView != null)
                    scrollableHtmlView.UpdateFooter();

                if (htmlView is ScrollableHtmlView)
                    ((ScrollableHtmlView)htmlView).OnHtmlLoaded();
                else
                    ((HtmlView)htmlView).OnHtmlLoaded();

                return;
            }

            HtmlNode node = null;
#if WP7
            await Task.Factory.StartNew(() =>
#else
            await Task.Run(() =>
#endif
            {
                try
                {
                    var parser = new HtmlParser();
                    node = parser.Parse(html);
                }
                catch { }
            });

            if (html == htmlView.Html)
            {
                if (node != null)
                {
                    try
                    {
                        itemsControl.Items.Clear();

                        if (scrollableHtmlView != null)
                            scrollableHtmlView.UpdateHeader();

                        foreach (var control in node.GetControls(htmlView))
                            itemsControl.Items.Add(control);

                        if (scrollableHtmlView != null)
                            scrollableHtmlView.UpdateFooter();
                    }
                    catch { }
                }

                if (htmlView is ScrollableHtmlView)
                    ((ScrollableHtmlView)htmlView).OnHtmlLoaded();
                else
                    ((HtmlView)htmlView).OnHtmlLoaded();
            }
        }
    }
}