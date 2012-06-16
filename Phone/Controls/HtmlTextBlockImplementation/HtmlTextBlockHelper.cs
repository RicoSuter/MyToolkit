using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.Controls.HtmlTextBlockImplementation
{
	internal static class HtmlTextBlockHelper
	{
		internal static void Generate(this IHtmlTextBlock me)
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

				Deployment.Current.Dispatcher.BeginInvoke(me.CallLoadedEvent);
				return;
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

								if (tb != null)
									tb.UpdateHeader();

								foreach (var c in node.GetControls(me))
									itemsCtrl.Items.Add(c);

								if (tb != null)
									tb.UpdateFooter();
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