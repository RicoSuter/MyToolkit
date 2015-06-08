//-----------------------------------------------------------------------
// <copyright file="HtmlTextBlockHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.System.Threading;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation
{
    internal static class HtmlTextBlockHelper
    {
        async internal static void Generate(this IHtmlTextBlock me)
        {
            var html = me.Html;
            var itemsCtrl = (ItemsControl)me;

            if (string.IsNullOrEmpty(html))
                itemsCtrl.Items.Clear();

            var tb = me as HtmlTextBlock;
            if (tb != null)
                tb.UpdateHeader();

            if (string.IsNullOrEmpty(html))
            {
                if (tb != null)
                    tb.UpdateFooter();

                if (me is HtmlTextBlock)
                    ((HtmlTextBlock)me).CallHtmlLoaded();
                else
                    ((FixedHtmlTextBlock)me).CallHtmlLoaded();
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

            if (html == me.Html)
            {
                if (node != null)
                {
                    try
                    {
                        itemsCtrl.Items.Clear();

                        if (tb != null)
                            tb.UpdateHeader();

                        foreach (var c in node.GetControls(me))
                            itemsCtrl.Items.Add(c);

                        if (tb != null)
                            tb.UpdateFooter();
                    }
                    catch { }
                }

                if (me is HtmlTextBlock)
                    ((HtmlTextBlock)me).CallHtmlLoaded();
                else
                    ((FixedHtmlTextBlock)me).CallHtmlLoaded();
            }
        }
    }
}