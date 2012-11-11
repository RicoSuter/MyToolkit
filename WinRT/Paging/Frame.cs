using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Paging
{
	public delegate void NavigatedEventHandler(object sender, NavigationEventArgs e);
	public class Frame : ContentControl//, INavigate
	{
		public event NavigatedEventHandler Navigated;
		public IEnumerable<PageDescription> Pages { get { return pages; } }

		private int currentIndex = -1; 
		private List<PageDescription> pages = new List<PageDescription>();

		private PageDescription Current { get { return pages.Count > 0 ? pages[currentIndex] : null; } }

		public Frame()
		{
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			VerticalContentAlignment = VerticalAlignment.Stretch;

			Loaded += delegate { Window.Current.VisibilityChanged += OnVisibilityChanged; };
			Unloaded += delegate { Window.Current.VisibilityChanged -= OnVisibilityChanged; };
		}

		private void OnVisibilityChanged(object sender, VisibilityChangedEventArgs args)
		{
			if (Current != null)
				Current.GetPage(this).OnVisibilityChanged(args);
		}

		public bool CanGoForward { get { return currentIndex < pages.Count - 1; } }

		/// <returns>Returns true if navigating forward, false if cancelled</returns>
		public async Task<bool> GoForwardAsync()
		{
			if (await CallOnNavigatingFrom(Current, NavigationMode.Forward))
				return false;

			GoForwardOrBack(NavigationMode.Forward);
			return true; 
		}

		public bool CanGoBack { get { return currentIndex > 0; } }

		/// <returns>Returns true if navigating back, false if cancelled</returns>
		public async Task<bool> GoBackAsync()
		{
			if (await CallOnNavigatingFrom(Current, NavigationMode.Back))
				return false;

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

		///// <returns>Return value is always true. Use NavigateAsync instead</returns>
		//public bool Navigate(Type sourcePageType)
		//{
		//	NavigateAsync(sourcePageType);
		//	return true; 
		//}

		public bool Initialize(Type sourcePageType, object parameter = null)
		{
			NavigateInternal(sourcePageType, parameter);
			return true; 
		}

		/// <returns>Returns true if the navigation process has not been cancelled</returns>
		public Task<bool> NavigateAsync(Type type)
		{
			return NavigateAsync(type, null);
		}

		public async Task<bool> NavigateAsync(Type type, object parameter)
		{
			if (Current != null && await CallOnNavigatingFrom(Current, NavigationMode.Forward))
				return false;
			
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

		private async Task<bool> CallOnNavigatingFrom(PageDescription description, NavigationMode mode)
		{
			var page = description.GetPage(this);
			var args = new NavigatingCancelEventArgs();
			args.Content = page;
			args.SourcePageType = description.Type;
			args.NavigationMode = mode;
			await page.InternalOnNavigatingFrom(args);
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
			CallOnNavigatingFrom(Current, NavigationMode.Forward);
			CallOnNavigatedFrom(Current, NavigationMode.Forward);

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
