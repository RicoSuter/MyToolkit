using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MyToolkit.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Paging
{
	public delegate void MyNavigatedEventHandler(object sender, MyNavigationEventArgs e);
	public class MyFrame : ContentControl, INavigate
	{
		private Stack<PageDescription> pageStack;
		public IEnumerable<PageDescription> PageStack { get { return pageStack; } }
		
		public MyFrame()
		{
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			VerticalContentAlignment = VerticalAlignment.Stretch;
			pageStack = new Stack<PageDescription>();
		}

		public bool CanGoBack { get { return pageStack.Count > 1; } }
		public event MyNavigatedEventHandler Navigated;

		public bool GoBack()
		{
			if (CallPrepareMethod(GoBackEx))
				return true; 
			GoBackEx();
			return false; 
		}

		private void GoBackEx()
		{
			if (CanGoBack)
			{
				var oldPage = pageStack.Pop();
				var newPage = pageStack.Peek(); 
				
				CallOnNavigatingFrom(oldPage, NavigationMode.Back);
				Content = newPage.GetPage(this);
				CallOnNavigatedFrom(oldPage, NavigationMode.Back);
				CallOnNavigatedTo(newPage, NavigationMode.Back);
			}
			else
				throw new Exception("cannot go back");
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

		private void NavigateEx(Type type, object parameter)
		{
			var oldPage = pageStack.Count > 0 ? pageStack.Peek() : null;
			var newPage = new PageDescription(type, parameter);
			pageStack.Push(newPage);

			if (oldPage != null)
				CallOnNavigatingFrom(oldPage, NavigationMode.Forward);
			Content = newPage.GetPage(this);
			if (oldPage != null)
				CallOnNavigatedFrom(oldPage, NavigationMode.Forward);
			CallOnNavigatedTo(newPage, NavigationMode.New);
		}

		private void CallOnNavigatedFrom(PageDescription description, NavigationMode mode)
		{
			var page = description.GetPage(this);
			var args = new MyNavigationEventArgs();
			args.Content = page;
			args.Type = description.Type;
			args.Parameter = description.Parameter;
			args.NavigationMode = mode;
			page.OnNavigatedFrom(args);
		}

		private void CallOnNavigatingFrom(PageDescription description, NavigationMode mode)
		{
			var page = description.GetPage(this);
			var args = new MyNavigationEventArgs();
			args.Content = page;
			args.Type = description.Type;
			args.Parameter = description.Parameter;
			args.NavigationMode = mode;
			page.OnNavigatingFrom(args);
		}

		private void CallOnNavigatedTo(PageDescription description, NavigationMode mode)
		{
			var page = description.GetPage(this);
			var args = new MyNavigationEventArgs();
			args.Content = page;
			args.Type = description.Type;
			args.Parameter = description.Parameter;
			args.NavigationMode = mode;
			page.OnNavigatedTo(args);

			if (Navigated != null)
				Navigated(this, args);
		}

		protected virtual void OnPageCreated(object sender, object page) { }

		protected virtual void OnNavigated(object sender, MyNavigationEventArgs e)
		{
			var page = e.Content;
			if (e.NavigationMode == NavigationMode.New)
				OnPageCreated(sender, page);
		}

		private bool CallPrepareMethod(Action completed)
		{
			if (pageStack.Count > 0)
			{
				var description = pageStack.Peek();
				var page = description.GetPage(this);

				var called = false;
				var continuationAction = new Action(() => 
					Window.Current.Dispatcher.RunAsync(
						Windows.UI.Core.CoreDispatcherPriority.Normal,
						delegate
						{
							if (!called)
							{
								page.IsHitTestVisible = true;
								completed();
								called = true;
							}
						}));

				var result = page.OnPrepareNavigatingFrom(continuationAction);
				if (result)
					page.IsHitTestVisible = false; // disable interactions while animating
				return result;
			}
			return false; 
		}

		public void SetNavigationState(string s)
		{
			pageStack = new Stack<PageDescription>(DataContractSerialization.Deserialize<List<PageDescription>>(s));
			Content = pageStack.Peek().GetPage(this);
			CallOnNavigatedTo(pageStack.Peek(), NavigationMode.Refresh);
		}

		public object GetNavigationState()
		{
			var oldPage = pageStack.Peek();
			CallOnNavigatingFrom(oldPage, NavigationMode.Forward);
			CallOnNavigatedFrom(oldPage, NavigationMode.Forward);

			return DataContractSerialization.Serialize(pageStack.Reverse().ToList());
		}

		public int BackStackDepth
		{
			get { return pageStack.Count; }
		}
	}
}
