using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MyToolkit.Controls.HtmlTextBlockSource;

namespace MyToolkit.Controls
{
	internal static class HtmlTextBlockHelper
	{
		internal static void UpdateHeaderControl(this ItemsControl me, UIElement newElem, UIElement oldElem)
		{
			if (oldElem == null && newElem != null)
				me.Items.Insert(0, newElem);
			else if (oldElem != null && newElem != null)
			{
				me.Items.Remove(0);
				me.Items.Insert(0, newElem);
			}
			else if (oldElem != null && newElem == null)
				me.Items.Remove(0);
		}

		internal static void Generate(this IHtmlTextBlock me, string html)
		{
			var itemsCtrl = (ItemsControl)me;
			if (me.HeaderItem != null)
				itemsCtrl.ItemsSource = new List<UIElement> { me.HeaderItem };

			ThreadPool.QueueUserWorkItem(o =>
			{
			    HtmlNode node = null;
			    try
			    {
			        var parser = new HtmlParser();
			        node = parser.Parse(html);
			    }
			    catch { }

			    Deployment.Current.Dispatcher.BeginInvoke(() =>
			    {
			        if (html == me.Html) // prevent from setting wrong control if html changed fast
			        {
			            if (node != null)
			            {
			                try
			                {
			                    var items = node.GetControls((IHtmlSettings)me).ToList();
			                    if (me.HeaderItem != null)
			                        items.Insert(0, me.HeaderItem);
			                    itemsCtrl.ItemsSource = items;
			                }
			                catch { }
			            }

			            Deployment.Current.Dispatcher.BeginInvoke(me.CallLoadedEvent);
			        }
			    });
			});
		}
	}
}