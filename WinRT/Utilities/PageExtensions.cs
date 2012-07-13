using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Utilities
{
	public static class PageExtensions
	{
		/// <summary>
		/// Call this method in OnNavigatedTo as the event will be automatically deregistred when the page has been unloaded
		/// </summary>
		public static void RegisterBackKey(this Page page)
		{
			var del = new TypedEventHandler<CoreDispatcher, AcceleratorKeyEventArgs>(
				delegate(CoreDispatcher sender, AcceleratorKeyEventArgs args)
				{
					if (!args.Handled && args.VirtualKey == VirtualKey.Back && args.EventType == CoreAcceleratorKeyEventType.KeyUp)
					{
						var element = FocusManager.GetFocusedElement();
						if (element is TextBox || element is PasswordBox || element is WebView)
							return; 

						if (page.Frame.CanGoBack)
							page.Frame.GoBack();
					}
				});

			page.Dispatcher.AcceleratorKeyActivated += del;

			SingleEvent.Register(page, (p, h) => p.Unloaded += h, (p, h) => p.Unloaded -= h, (o, a) =>
			{
				page.Dispatcher.AcceleratorKeyActivated -= del;
			});
		}

		/// <summary>
		/// Call this method in OnNavigatedTo as the event will be automatically deregistred when the page has been unloaded
		/// </summary>
		/// <param name="page"></param>
		/// <param name="handler"></param>
		public static void RegisterAcceleratorKeyActivated(this Page page, TypedEventHandler<CoreDispatcher, AcceleratorKeyEventArgs> handler)
		{
			page.Dispatcher.AcceleratorKeyActivated += handler;
			SingleEvent.Register(page, (p, h) => p.Unloaded += h, (p, h) => p.Unloaded -= h, (o, a) =>
			{
				page.Dispatcher.AcceleratorKeyActivated -= handler;
			});
		}
	}
}
