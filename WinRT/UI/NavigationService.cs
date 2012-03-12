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

namespace MyToolkit.Metro.UI
{
	public class NavigationService
	{
		public Frame Frame { get; private set; }

		public object CurrentPage { get { return Frame.Content; } }
		public int StackSize { get; private set; }

		public bool GoBack()
		{
			StackSize--;

			if (CallPrepareMethod(delegate { Frame.GoBack(); }))
				return true; 

			Frame.GoBack();
			return false; 
		}

		//public bool CanGoBack { get { return Frame.CanGoBack; } }
		public bool CanGoBack { get { return StackSize > 1; } }

		public NavigationService(Frame frame)
		{
			Frame = frame;
			Frame.Navigated += OnNavigated;
		}

		private void OnNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			// TODO create new instance if new page of same type
			//if (e.NavigationMode == NavigationMode.New)
			//	Frame.Content = Activator.CreateInstance(e.SourcePageType);

			var page = Frame.Content; //e.Content;
			InjectNavigationService(page);

			if (e.NavigationMode == NavigationMode.New)
				OnPageCreated(sender, page);
		}

		protected virtual void OnPageCreated(object sender, object page) { }

		public bool Navigate(Type type, object parameter = null)
		{
			if (CallPrepareMethod(delegate { NavigateInternal(type, parameter); }))
				return true;

			NavigateInternal(type, parameter);
			return false;
		}

		private bool CallPrepareMethod(InvokedHandler handler)
		{
			var page = Frame.Content;
			if (page == null)
				return false; 

			var method = page.GetType().GetTypeInfo().GetDeclaredMethod("OnPrepareNavigatingFrom");
			if (method != null)
			{
				if (method.GetParameters().Count() == 1)
				{
					var finishedAction = new Action(() =>
					{
						Window.Current.Dispatcher.Invoke(
							Windows.UI.Core.CoreDispatcherPriority.Normal,
							handler, page, null);
					});

					method.Invoke(page, new object[] { finishedAction });
					return true; 
				}
			}
			return false; 
		}

		private void NavigateInternal(Type type, object parameter)
		{
			StackSize++;
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
