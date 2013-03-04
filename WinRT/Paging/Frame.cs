using System;
using System.Collections.Generic;
using System.Linq;
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

		private int currentIndex = -1; 
		private List<PageDescription> pages = new List<PageDescription>();

		public PageDescription CurrentPageDescription { get { return pages.Count > 0 ? pages[currentIndex] : null; } }
		public Page CurrentPage { get { return CurrentPageDescription != null ? CurrentPageDescription.GetPage(this) : null; } }
		
		public Type PreviousPageType {get { return currentIndex > 0 ? pages[currentIndex - 1].Type : null; }}

		public Frame()
		{
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			VerticalContentAlignment = VerticalAlignment.Stretch;

			Loaded += delegate { Window.Current.VisibilityChanged += OnVisibilityChanged; };
			Unloaded += delegate { Window.Current.VisibilityChanged -= OnVisibilityChanged; };
		}

		private void OnVisibilityChanged(object sender, VisibilityChangedEventArgs args)
		{
			if (CurrentPageDescription != null)
				CurrentPageDescription.GetPage(this).OnVisibilityChanged(args);
		}

		public bool CanGoForward { get { return currentIndex < pages.Count - 1; } }

		/// <returns>Returns true if navigating forward, false if cancelled</returns>
		public async Task<bool> GoForwardAsync()
		{
			if (await CallOnNavigatingFrom(CurrentPageDescription, NavigationMode.Forward))
				return false;

			GoForwardOrBack(NavigationMode.Forward);
			return true; 
		}

		public bool CanGoBack { get { return currentIndex > 0; } }

		public IReadOnlyList<PageDescription> Pages { get { return pages; } }

		public PageDescription GetNearestPageOfTypeInBackStack(Type pageType)
		{
			var index = currentIndex;
			while (index >= 0)
			{
				if (pages[index].Type == pageType)
					return pages[index];
				index--;
			}
			return null; 
		}

		public async Task<bool> GoBackToAsync(PageDescription page)
		{
			var index = pages.IndexOf(page);
			return await GoBackToAsync(index);
		}

		public async Task<bool> GoBackToAsync(int newIndex)
		{
			if (newIndex == currentIndex)
				return true;

			if (newIndex < 0 || newIndex > currentIndex)
				return false; 
				
			while (currentIndex != newIndex)
			{
				if (!await GoBackAsync())
					return false;
			}

			return true; 
		}

		public async Task<bool> GoHomeAsync()
		{
			while (currentIndex != 0)
			{
				if (!await GoBackAsync())
					return false; 
			}
			return true; 
		}

		/// <returns>Returns true if navigating back, false if cancelled</returns>
		public async Task<bool> GoBackAsync()
		{
			if (await CallOnNavigatingFrom(CurrentPageDescription, NavigationMode.Back))
				return false;

			GoForwardOrBack(NavigationMode.Back);
			return true; 
		}

		private void GoForwardOrBack(NavigationMode mode)
		{
			if (mode == NavigationMode.Forward ? CanGoForward : CanGoBack)
			{
				var oldPage = CurrentPageDescription;
				currentIndex += mode == NavigationMode.Forward ? 1 : -1;
				var newPage = CurrentPageDescription;

				Content = newPage.GetPage(this);
				CallOnNavigatedFrom(oldPage, mode);
				CallOnNavigatedTo(newPage, mode);
			}
			else
				throw new Exception("cannot go forward or back");
		}

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
			if (CurrentPageDescription != null && await CallOnNavigatingFrom(CurrentPageDescription, NavigationMode.Forward))
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
			var oldPage = CurrentPageDescription;
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
			var tuple = DataContractSerialization.Deserialize<Tuple<int, List<PageDescription>>>(s, SuspensionManager.KnownTypes.ToArray());
			pages = new List<PageDescription>(tuple.Item2);
			currentIndex = tuple.Item1;

			if (currentIndex != -1)
			{
				Content = CurrentPageDescription.GetPage(this);
				CallOnNavigatedTo(CurrentPageDescription, NavigationMode.Refresh);
			}
		}

		public string GetNavigationState()
		{
			CallOnNavigatingFrom(CurrentPageDescription, NavigationMode.Forward);
			CallOnNavigatedFrom(CurrentPageDescription, NavigationMode.Forward);

			// remove pages which do not support tombstoning
			var pagesToSerialize = pages;
			var currentIndexToSerialize = currentIndex; 
			var firstPageToRemove = pages.FirstOrDefault(p =>
			{
				var page = p.GetPage(this);
				return !(page is ExtendedPage) || !((ExtendedPage) page).IsSuspendable;
			});

			if (firstPageToRemove != null)
			{
				var index = pagesToSerialize.IndexOf(firstPageToRemove);
				pagesToSerialize = pages.Take(index).ToList();
				currentIndexToSerialize = index - 1; 
			}

			var output = DataContractSerialization.Serialize(
				new Tuple<int, List<PageDescription>>(currentIndexToSerialize, pagesToSerialize), true, SuspensionManager.KnownTypes.ToArray());
			return output; 
		}

		public int BackStackDepth
		{
			get { return currentIndex + 1; }
		}
	}
}
