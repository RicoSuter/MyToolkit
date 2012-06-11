using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.Controls.HtmlTextBlockSource
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
			{
				itemsCtrl.Items.Clear();
				itemsCtrl.Items.Add(me.HeaderItem);
			}

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
								itemsCtrl.Items.Clear();
								if (me.HeaderItem != null)
									itemsCtrl.Items.Add(me.HeaderItem);

								foreach (var c in node.GetControls((IHtmlSettings)me))
									itemsCtrl.Items.Add(c);
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