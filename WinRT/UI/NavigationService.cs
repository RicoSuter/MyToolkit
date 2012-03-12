using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Reflection;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Metro.UI
{
	public class NavigationService
	{
		public Frame Frame { get; private set; }

		public object CurrentPage { get { return Frame.Content; } }
		public int StackSize { get; private set; }

		public void GoBack()
		{
			StackSize--;
			Frame.GoBack();
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
			var page = e.Content;
			InjectNavigationService(page);

			if (PageCreated != null)
				PageCreated(sender, page);
			OnPageCreated(sender, page);

			CallOnNavigatedTo(page, e.Parameter);
		}

		public event Action<object, object> PageCreated;
		protected virtual void OnPageCreated(object sender, object page) { }

		public bool Navigate(Type type)
		{
			var lastPage = Frame.Content; 
			if (lastPage != null)
				return CallOnNavigatedFrom(lastPage, type);
			else
			{
				NavigateInternal(type);
				return false; 
			}
		}

		private void NavigateInternal(Type type)
		{
			StackSize++;
			Frame.Navigate(type);
		}

		private void InjectNavigationService(object page)
		{
			var property = page.GetType().GetTypeInfo().GetDeclaredProperty("NavigationService");
			if (property != null)
				property.SetValue(page, this);
		}

		private void CallOnNavigatedTo(object page, object parameter)
		{
			var method = page.GetType().GetTypeInfo().GetDeclaredMethod("OnNavigatedTo");
			if (method != null)
				method.Invoke(page, method.GetParameters().Length == 1 ? new object[] { parameter } : null);
		}

		private bool CallOnNavigatedFrom(object page, Type newPage)
		{
			var method = page.GetType().GetTypeInfo().GetDeclaredMethod("OnNavigatedFrom");
			if (method != null)
			{
				if (method.GetParameters().Count() == 1)
				{
					var finishedAction = new Action(() =>
					{
						Window.Current.Dispatcher.Invoke(
							Windows.UI.Core.CoreDispatcherPriority.Normal,
							delegate { NavigateInternal(newPage); }, page, null);
					});
					method.Invoke(page, new object[] { finishedAction });
					return true; 
				}
			}

			NavigateInternal(newPage);
			return false;
		}
	}
}
