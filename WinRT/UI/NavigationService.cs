using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using MyToolkit.UI;
using MyToolkit.Controls;

namespace MyToolkit.UI
{
	public class NavigationService
	{
		public ExtendedFrame Frame { get; private set; }
		public object CurrentPage { get { return Frame.Content; } }

		public bool CanGoBack
		{
			get { return Frame.CanGoBack; }
		}

		public bool GoBack()
		{
			if (CallPrepareMethod(() => { Frame.GoBack(); }))
				return true; 

			Frame.GoBack();
			return false; 
		}

		public NavigationService(ExtendedFrame frame)
		{
			Frame = frame;
			Frame.Navigated += OnNavigated;
		}

		private void OnNavigated(object sender, NavigationEventArgs e)
		{
			var page = e.Content;
			InjectNavigationService(page);

			if (e.NavigationMode == NavigationMode.New)
				OnPageCreated(sender, page);
		}

		protected virtual void OnPageCreated(object sender, object page) { }

		public bool Navigate(Type type, object parameter = null)
		{
			if (CallPrepareMethod(() => { NavigateInternal(type, parameter); }))
				return true;

			NavigateInternal(type, parameter);
			return false;
		}

		private bool CallPrepareMethod(Action handler)
		{
			var page = Frame.Content;
			if (page == null)
				return false;

			var method = page.GetType().GetTypeInfo().GetDeclaredMethod("OnPrepareNavigatingFrom");
			if (method != null && method.GetParameters().Count() == 1)
			{
				var called = false; 
				var finishedAction = new Action(() =>
				{
					Window.Current.Dispatcher.Invoke(
						Windows.UI.Core.CoreDispatcherPriority.Normal,
						(x, y) => 
						{
							if (!called)
							{
								handler();
								called = true;
							}
						}, 
						page, null);
				});

				method.Invoke(page, new object[] { finishedAction });
				return true;
			}
			return false; 
		}

		private void NavigateInternal(Type type, object parameter)
		{
			Frame.Navigate(type, parameter);
		}

		private void InjectNavigationService(object page)
		{
			var property = page.GetType().GetTypeInfo().GetDeclaredProperty("NavigationService");
			if (property != null)
				property.SetValue(page, this);
		}
	}
}
