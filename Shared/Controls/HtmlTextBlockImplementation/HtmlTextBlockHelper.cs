using System;
using System.Threading;

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
#if WINRT
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

				me.CallLoadedEvent();
				return;
			}

			HtmlNode node = null;
			await ThreadPool.RunAsync(o =>
			{
				try
				{
					var parser = new HtmlParser();
					node = parser.Parse(html);
				} catch { }
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
					} catch { }
				}
				me.CallLoadedEvent();
			}
		}
#else
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
#endif
	}
}