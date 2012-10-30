using System;
using System.Collections.Generic;
using System.Linq;
using MyToolkit.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Paging
{
	public delegate void MyNavigatedEventHandler(object sender, NavigationEventArgs e);
	public class Frame : ContentControl, INavigate
	{
		public event MyNavigatedEventHandler Navigated;
		public IEnumerable<PageDescription> Pages { get { return pages; } }

		private int currentIndex = -1; 
		private List<PageDescription> pages = new List<PageDescription>();

		private PageDescription Current { get { return pages.Count > 0 ? pages[currentIndex] : null; } }

		public Frame()
		{
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			VerticalContentAlignment = VerticalAlignment.Stretch;
		}

		public bool CanGoForward { get { return currentIndex < pages.Count - 1; } }

		/// <returns>Returns true if navigating forward, false if cancelled</returns>
		public bool GoForward()
		{
			if (CallOnNavigatingFrom(Current, NavigationMode.Back))
				return false; 
			if (!CallPrepareMethod(() => GoForwardOrBack(NavigationMode.Forward)))
				GoForwardOrBack(NavigationMode.Forward);
			return true; 
		}

		public bool CanGoBack { get { return currentIndex > 0; } }

		/// <returns>Returns true if navigating back, false if cancelled</returns>
		public bool GoBack()
		{
			if (CallOnNavigatingFrom(Current, NavigationMode.Back))
				return false; 
			if (!CallPrepareMethod(() => GoForwardOrBack(NavigationMode.Back)))
				GoForwardOrBack(NavigationMode.Back);
			return true; 
		}

		private void GoForwardOrBack(NavigationMode mode)
		{
			if (mode == NavigationMode.Forward ? CanGoForward : CanGoBack)
			{
				var oldPage = Current;
				currentIndex += mode == NavigationMode.Forward ? 1 : -1;
				var newPage = Current;

				Content = newPage.GetPage(this);
				CallOnNavigatedFrom(oldPage, mode);
				CallOnNavigatedTo(newPage, mode);
			}
			else
				throw new Exception("cannot go forward or back");
		}

		public bool Navigate(Type type)
		{
			return Navigate(type, null);
		}

		public bool Navigate(Type type, object parameter)
		{
			if (CallOnNavigatingFrom(Current, NavigationMode.Back))
				return false;
			if (!CallPrepareMethod(() => NavigateInternal(type, parameter)))
				NavigateInternal(type, parameter);
			return true;
		}

		private void RemoveAllAfterCurrent()
		{
			for (var i = pages.Count - 1; i > currentIndex; i--)
			{
				var page = pages[i];
				pages.Remove(page);
			}
		}

		private void NavigateInternal(Type type, object parameter)
		{
			var oldPage = Current;
			RemoveAllAfterCurrent(); 

			var newPage = new PageDescription(type, parameter);
			pages.Add(newPage);
			currentIndex++; 

			Content = newPage.GetPage(this);
			if (oldPage != null)
				CallOnNavigatedFrom(oldPage, NavigationMode.Forward);
			CallOnNavigatedTo(newPage, NavigationMode.New);
		}

		private void CallOnNavigatedFrom(PageDescription description, NavigationMode mode)
		{
			var page = description.GetPage(this);
			var args = new NavigationEventArgs();
			args.Content = page;
			args.SourcePageType = description.Type;
			args.Parameter = description.Parameter;
			args.NavigationMode = mode;
			page.InternalOnNavigatedFrom(args);
		}

		private bool CallOnNavigatingFrom(PageDescription description, NavigationMode mode)
		{
			var page = description.GetPage(this);
			var args = new NavigatingCancelEventArgs();
			args.Content = page;
			args.SourcePageType = description.Type;
			args.NavigationMode = mode;
			page.InternalOnNavigatingFrom(args);
			return args.Cancel; 
		}

		private void CallOnNavigatedTo(PageDescription description, NavigationMode mode)
		{
			var page = description.GetPage(this);
			var args = new NavigationEventArgs();
			args.Content = page;
			args.SourcePageType = description.Type;
			args.Parameter = description.Parameter;
			args.NavigationMode = mode;
			page.InternalOnNavigatedTo(args);

			if (Navigated != null)
				Navigated(this, args);
		}

		protected virtual void OnPageCreated(object sender, object page) { }

		protected virtual void OnNavigated(object sender, NavigationEventArgs e)
		{
			var page = e.Content;
			if (e.NavigationMode == NavigationMode.New)
				OnPageCreated(sender, page);
		}

		private bool CallPrepareMethod(Action completed)
		{
			var description = Current;
			if (description != null)
			{
				var page = description.GetPage(this);
				
				var called = false;
				var continuationAction = new Action(() => 
					Window.Current.Dispatcher.RunAsync(
						Windows.UI.Core.CoreDispatcherPriority.Normal,
						delegate
						{
							if (!called)
							{
								page.IsPreparingNavigatingFrom = false;
								page.IsHitTestVisible = true;
								completed();
								called = true;
							}
						}));

				var result = page.OnPrepareNavigatingFrom(continuationAction);
				if (result)
				{
					// disable interactions while animating
					page.IsHitTestVisible = false; 
					page.IsPreparingNavigatingFrom = true; 
				}
				return result;
			}
			return false; 
		}

		public void SetNavigationState(string s)
		{
			var tuple = DataContractSerialization.Deserialize<Tuple<int, List<PageDescription>>>(s);
			pages = new List<PageDescription>(tuple.Item2);
			currentIndex = tuple.Item1;

			Content = Current.GetPage(this);
			CallOnNavigatedTo(Current, NavigationMode.Refresh);
		}

		public string GetNavigationState()
		{
			var oldPage = Current;
			CallOnNavigatingFrom(oldPage, NavigationMode.Forward);
			CallOnNavigatedFrom(oldPage, NavigationMode.Forward);

			var output = DataContractSerialization.Serialize(
				new Tuple<int, List<PageDescription>>(currentIndex, pages), true);
			return output; 
		}

		public int BackStackDepth
		{
			get { return currentIndex + 1; }
		}
	}
}
