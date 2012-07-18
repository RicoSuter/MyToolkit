using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Controls
{
	public class PageFrame : ContentControl, INavigate
	{
		public PageFrame()
		{
			PageStack = new Stack<object>();
		}

		public bool CanGoBack { get { return PageStack.Count > 1; } }

		public Stack<object> PageStack { get; private set; }

		public event NavigatedEventHandler Navigated;

		public bool GoBack()
		{
			if (CallPrepareMethod(GoBackEx))
				return true; 

			GoBackEx();
			return false; 
		}

		private void GoBackEx()
		{
			PageStack.Pop();

			var page = PageStack.Peek();
            NavigationEventArgs args = null; // new NavigationEventArgs(page, null, page.GetType(), null, NavigationMode.Back); TOOD find solution

			if (Navigated != null)
				Navigated(this, args);

			if (page is Page)
			{
				var method = typeof(Page).GetTypeInfo().GetDeclaredMethod("OnNavigatedTo");
				method.Invoke(page, new object[] { args });
			}

			Content = page;
		}

		public bool Navigate(Type type)
		{
			return Navigate(type, null);
		}

		public bool Navigate(Type type, object parameter)
		{
			if (CallPrepareMethod(() => NavigateEx(type, parameter)))
				return true;
			NavigateEx(type, parameter);
			return false;
		}

		private Type nextType;
		private object nextParameter; 
		private void NavigateEx(Type type, object parameter)
		{
			if (Content == null)
			{
				if (nextType != null)
					throw new Exception();

				nextType = type;
				nextParameter = parameter;
				return; 
			}

			var page = (Control)Activator.CreateInstance(type);
            NavigationEventArgs args = null; // new NavigationEventArgs(page, parameter, type, null, NavigationMode.New); // TODO find solution
		
			PageStack.Push(page);
			Content = page;

			if (Navigated != null)
				Navigated(this, args);
			OnNavigated(this, args);

			if (page is Page)
			{
				var method = typeof(Page).GetTypeInfo().GetDeclaredMethod("OnNavigatedTo");
				method.Invoke(page, new object[] { args });
			}
		}

		protected virtual void OnPageCreated(object sender, object page) { }

		protected virtual void OnNavigated(object sender, NavigationEventArgs e)
		{
			var page = e.Content;
			InjectPageFrame(page);

			if (e.NavigationMode == NavigationMode.New)
				OnPageCreated(sender, page);
		}

		private void InjectPageFrame(object page)
		{
			var property = page.GetType().GetTypeInfo().GetDeclaredProperty("PageFrame");
			if (property != null)
				property.SetValue(page, this);
		}

		private bool CallPrepareMethod(Action handler)
		{
			var page = Content;
			if (page == null)
				return false;

			var method = page.GetType().GetTypeInfo().GetDeclaredMethod("OnPrepareNavigatingFrom");
			if (method != null && method.GetParameters().Count() == 1)
			{
				if (page is UIElement)
					((UIElement)page).IsHitTestVisible = false; // disable interactions while animating

				var called = false; 
				var finishedAction = new Action(() =>
				{
					Window.Current.Dispatcher.RunAsync(
						Windows.UI.Core.CoreDispatcherPriority.Normal, 
						delegate 
						{
							if (!called)
							{
								if (page is UIElement)
									((UIElement)page).IsHitTestVisible = true;
								
								handler();
								called = true;
							}
						});
				});

				method.Invoke(page, new object[] { finishedAction });
				return true;
			}
			return false; 
		}
	}
}
