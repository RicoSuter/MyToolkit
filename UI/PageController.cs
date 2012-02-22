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
	public class PageController
	{
		public Stack<UserControl> PageStack { get; private set; }
		public bool CanGoBack { get { return PageStack.Count > 1; } }

		public PageController()
		{
			PageStack = new Stack<UserControl>();
		}

		public bool Navigate(UserControl page)
		{
			// TODO for better performance => create page (GetPage) after CallOnNavigatedFrom() call (performance). Problem: page in lambda not availalbe then

			var lastPage = CurrentPage; 
			PageStack.Push(page);

			if (lastPage != null)
				return CallOnNavigatedFrom(lastPage, page);
			else
			{
				SetPage(page);
				CallOnNavigatedTo(page);
				return false; 
			}
		}

		protected virtual void SetPage(UserControl page)
		{
			Window.Current.Content = page;
		}

		private void CallOnNavigatedTo(UserControl page)
		{
			var method = page.GetType().GetTypeInfo().GetDeclaredMethod("OnNavigatedTo");
			if (method != null)
				method.Invoke(page, null);
		}

		private bool CallOnNavigatedFrom(UserControl page, UserControl newPage)
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
							delegate
							{
								SetPage(newPage);
								CallOnNavigatedTo(newPage);
							}, page, null);
					});
					method.Invoke(page, new object[] { finishedAction });
					return true; 
				}
			}

			SetPage(newPage);
			CallOnNavigatedTo(newPage);
			return false;
		}

		public UserControl CurrentPage
		{
			get { return PageStack.Count > 0 ? PageStack.Peek() : null; }
		}

		public void GoBack()
		{
			var lastPage = CurrentPage;
			PageStack.Pop();
			CallOnNavigatedFrom(lastPage, CurrentPage);
		}
	}
}
