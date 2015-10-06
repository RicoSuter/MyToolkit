//-----------------------------------------------------------------------
// <copyright file="MtFrame.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MyToolkit.Command;
using MyToolkit.Paging.Animations;
using MyToolkit.Paging.Handlers;
using MyToolkit.Serialization;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Environment;

namespace MyToolkit.Paging
{
    public delegate void NavigatedEventHandler(object sender, MtNavigationEventArgs e);
    public delegate void NavigatingEventHandler(object sender, MtNavigatingCancelEventArgs e);

    /// <summary>Navigation container for pages. </summary>
    public class MtFrame : Control, INavigate
    {
        private int _currentIndex = -1;
        private List<MtPageDescription> _pages = new List<MtPageDescription>();

        /// <summary>Initializes a new instance of the <see cref="MtFrame"/> class. </summary>
        public MtFrame()
        {
#if WINDOWS_UAP
            AutomaticBackButtonHandling = true;
#endif

            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            Loaded += delegate { Window.Current.VisibilityChanged += OnVisibilityChanged; };
            Unloaded += delegate { Window.Current.VisibilityChanged -= OnVisibilityChanged; };

            GoBackCommand = new AsyncRelayCommand(GoBackAsync, () => CanGoBack);

            DefaultStyleKey = typeof(MtFrame);

            if (Device.HasHardwareBackKey)
            {
                DisableForwardStack = true;
                PageAnimation = new TurnstilePageAnimation();
            }
            else
                DisableForwardStack = false;
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(MtFrame), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the content of the <see cref="MtFrame"/>. </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentTransitionsProperty = DependencyProperty.Register(
            "ContentTransitions", typeof(TransitionCollection), typeof(MtFrame), new PropertyMetadata(default(TransitionCollection)));

        /// <summary>Gets or sets the content transitions of the <see cref="MtFrame"/>. </summary>
        public TransitionCollection ContentTransitions
        {
            get { return (TransitionCollection)GetValue(ContentTransitionsProperty); }
            set { SetValue(ContentTransitionsProperty, value); }
        }

        /// <summary>Gets the current <see cref="MtFrame"/>. </summary>
        public static MtFrame Current
        {
            get { return Window.Current.Content as MtFrame; }
        }

        /// <summary>Gets the current page index. </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            private set
            {
                if (_currentIndex != value)
                {
                    _currentIndex = value;

#if WINDOWS_UAP
                    if (AutomaticBackButtonHandling)
                    {
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                            CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
                    }
#endif
                }
            }
        }

#if WINDOWS_UAP

        /// <summary>Gets or sets a value indicating whether the back button is automatically shown and hidden by the frame (default: true).</summary>
        public bool AutomaticBackButtonHandling { get; set; }

#endif

        /// <summary>Gets a value indicating whether the first/root page is visible. </summary>
        public bool IsFirstPage
        {
            get { return CurrentIndex == 0; }
        }

        /// <summary>Occurs when the frame navigated to another page. </summary>
        public event NavigatedEventHandler Navigated;

        /// <summary>Occurs when the frame navigates to another page. </summary>
        public event NavigatingEventHandler Navigating;

        /// <summary>Gets a command to navigate to the previous page. </summary>
        public ICommand GoBackCommand { get; private set; }

        /// <summary>Gets or sets a value indicating whether the forward stack is disabled 
        /// (default: disabled on Windows Phone, enabled on Windows). </summary>
        public bool DisableForwardStack { get; set; }

        /// <summary>Gets or sets a value indicating whether the cache is fully 
        /// deactivated (should be used only for testing). Default: false. </summary>
        public bool DisableCache { get; set; }

        /// <summary>Gets the page before the current page in the page stack or null if not available. </summary>
        public MtPageDescription PreviousPage
        {
            get { return CurrentIndex > 0 ? _pages[CurrentIndex - 1] : null; }
        }

        /// <summary>Gets the current page. </summary>
        public MtPageDescription CurrentPage
        {
            get { return _pages.Count > 0 ? _pages[CurrentIndex] : null; }
        }

        /// <summary>Gets the page after the current page in the page stack or null if not available. </summary>
        public MtPageDescription NextPage
        {
            get { return CurrentIndex < _pages.Count - 1 ? _pages[CurrentIndex + 1] : null; }
        }

        /// <summary>Gets the current page animation. 
        /// Only available when ContentTransitions is null.
        /// May be overridden by the current page's PageAnimation property. </summary>
        public IPageAnimation PageAnimation { get; set; }

        private IPageAnimation ActualPageAnimation
        {
            get
            {
                if (ContentTransitions != null)
                    return null;

                var currentPage = CurrentPage;
                return currentPage != null && currentPage.Page != null && currentPage.Page.PageAnimation != null ?
                    CurrentPage.Page.PageAnimation : PageAnimation;
            }
        }

        /// <summary>Gets or sets a value indicating whether to show the animation when launching, leaving or switching to the app. Default: false. </summary>
        public bool ShowNavigationOnAppInAndOut { get; set; }

        /// <summary>Gets the underlying WinRT frame object. </summary>
        public Frame InternalFrame { get; private set; }

        /// <summary>Gets a value indicating whether it is possible to navigate forward. </summary>
        public bool CanGoForward { get { return CurrentIndex < _pages.Count - 1; } }

        /// <summary>Tries to navigate forward to the next page. </summary>
        /// <remarks>After the task has completed the <see cref="Frame"/>'s current page has changed. </remarks>
        /// <returns>Returns true if navigating forward, false if cancelled</returns>
        public async Task<bool> GoForwardAsync()
        {
            try
            {
                IsNavigating = true;

                if (await RaisePageOnNavigatingFromAsync(CurrentPage, null, NavigationMode.Forward))
                    return false;

                await GoForwardOrBackAsync(NavigationMode.Forward);
                return true;
            }
            finally
            {
                IsNavigating = false; 
            }
        }

        /// <summary>Gets a value indicating whether it is possible to navigate back. </summary>
        public bool CanGoBack { get { return CurrentIndex > 0; } }

        /// <summary>Gets a list of the pages in the page stack. </summary>
        public IReadOnlyList<MtPageDescription> Pages { get { return _pages; } }

        /// <summary>Gets the number of pages in the page back stack. </summary>
        public int BackStackDepth
        {
            get { return CurrentIndex + 1; }
        }

        private Grid ContentGrid
        {
            get
            {
                if (Content == null)
                    Content = new Grid();
                return (Grid)Content;
            }
        }

        /// <summary>Gets the first page of the specified type in the page back stack or null if no page of the type is available. </summary>
        /// <param name="pageType">The page type. </param>
        /// <returns>The page or null if not found. </returns>
        public MtPageDescription GetNearestPageOfTypeInBackStack(Type pageType)
        {
            var index = CurrentIndex;
            while (index >= 0)
            {
                if (_pages[index].Type == pageType)
                    return _pages[index];
                index--;
            }
            return null;
        }

        /// <summary>Navigates back to the given page. </summary>
        /// <param name="pageDescription">The page to navigate to. </param>
        /// <returns>True if the navigation could be performed. </returns>
        public async Task<bool> GoBackToAsync(MtPageDescription pageDescription)
        {
            var index = _pages.IndexOf(pageDescription);
            return await GoBackToAsync(index);
        }

        /// <summary>Navigates back to the first page in the page stack. </summary>
        /// <returns>True if the navigation could be performed. </returns>
        public Task<bool> GoHomeAsync()
        {
            return GoBackToAsync(0);
        }

        /// <summary>Navigates back to the given index. </summary>
        /// <param name="newPageIndex">The page index. </param>
        /// <returns>True if the navigation could be performed. </returns>
        public async Task<bool> GoBackToAsync(int newPageIndex)
        {
            if (newPageIndex == CurrentIndex)
                return false;

            if (newPageIndex < 0 || newPageIndex > CurrentIndex)
                return false;

            try
            {
                IsNavigating = true;

                if (await RaisePageOnNavigatingFromAsync(CurrentPage, _pages[newPageIndex], NavigationMode.Back))
                    return false;

                var currentPage = CurrentPage;
                var nextPage = _pages[newPageIndex];

                await NavigateWithAnimationsAndCallbacksAsync(NavigationMode.Back, currentPage, nextPage, newPageIndex);

                if (DisableForwardStack)
                    RemoveForwardStack();

                return true;
            }
            finally
            {
                IsNavigating = false;
            }
        }

        /// <summary>Removes a page from the page stack. </summary>
        /// <param name="pageDescription">The page to remove. </param>
        /// <returns><c>true</c> if the page has been found and was removed; otherwise, <c>false</c>. </returns>
        /// <exception cref="ArgumentException">The current page cannot be removed from the stack. </exception>
        public bool RemovePageFromStack(MtPageDescription pageDescription)
        {
            var index = _pages.IndexOf(pageDescription);
            if (index >= 0)
            {
                RemovePageFromStackAt(index);
                return true;
            }
            return false;
        }

        /// <summary>Removes a page from the page stack. </summary>
        /// <param name="pageIndex">The index of the page page to remove. </param>
        /// <returns><c>true</c> if the page has been found and was removed; otherwise, <c>false</c>. </returns>
        /// <exception cref="ArgumentException">The current page cannot be removed from the stack. </exception>
        public bool RemovePageFromStackAt(int pageIndex)
        {
            if (pageIndex == CurrentIndex)
                throw new ArgumentException("The current page cannot be removed from the stack. ");

            _pages.RemoveAt(pageIndex);
            if (pageIndex < CurrentIndex)
                CurrentIndex--;

            return true;
        }

        /// <summary>Gets a value indicating whether the frame is currently navigating to another page. </summary>
        public bool IsNavigating { get; private set; }

        /// <summary>Tries to navigate back to the previous page. </summary>
        /// <remarks>After the task has completed the <see cref="Frame"/>'s current page has changed. </remarks>
        /// <returns>Returns true if navigating back, false if cancelled or CanGoBack is false. </returns>
        public async Task<bool> GoBackAsync()
        {
            try
            {
                IsNavigating = true;

                if (await RaisePageOnNavigatingFromAsync(CurrentPage, PreviousPage, NavigationMode.Back))
                    return false;

                await GoForwardOrBackAsync(NavigationMode.Back);
                return true;
            }
            finally
            {
                IsNavigating = false;
            }
        }

        /// <summary>Initializes the frame and navigates to the given first page. </summary>
        /// <param name="homePageType">The type of the home page. </param>
        /// <param name="parameter">The parameter for the page. </param>
        public void Initialize(Type homePageType, object parameter = null)
        {
            NavigateAsync(homePageType, parameter);
        }

        /// <summary>Navigates forward to a new instance of the given page type. </summary>
        /// <param name="pageType">The page type. </param>
        /// <returns>Returns true if the navigation process has not been cancelled. </returns>
        public Task<bool> NavigateAsync(Type pageType)
        {
            return NavigateAsync(pageType, null);
        }

        /// <summary>Navigates to the page of the given type. </summary>
        /// <param name="sourcePageType">The page type. </param>
        public bool Navigate(Type sourcePageType)
        {
            NavigateAsync(sourcePageType);
            return true;
        }

        /// <summary>Navigates forward to a new instance of the given page type. </summary>
        /// <param name="pageType">The page type. </param>
        /// <param name="parameter">The page parameter. </param>
        /// <returns>Returns true if the navigation process has not been cancelled. </returns>
        public async Task<bool> NavigateAsync(Type pageType, object parameter)
        {
            var newPage = new MtPageDescription(pageType, parameter);
            return await NavigateAsync(newPage);
        }

        /// <summary>Navigates to the given page and removes the page from the previous position in the page stack.</summary>
        /// <param name="page">The page.</param>
        /// <returns>True if page is now on top of the stack, false when navigation from the current page failed.</returns>
        public async Task<bool> MoveToTopAndNavigateAsync(MtPageDescription page)
        {
            if (CurrentPage == page)
                return true;

            var index = _pages.IndexOf(page);
            if (index != -1)
            {
                _pages.RemoveAt(index);
                _currentIndex--;

                if (await NavigateAsync(page))
                    return true;

                _pages.Insert(index, page);
            }
            return false;
        }

        /// <summary>Clears all pages from the page back stack.</summary>
        public void ClearBackStack()
        {
            var pages = _pages.ToArray();
            for (var i = 0; i < _currentIndex; i++)
            {
                var page = pages[i];
                _pages.Remove(page);
            }
            _currentIndex = 0;
        }

        /// <summary>Used set the serialized the current page stack (used in the SuspensionManager). </summary>
        /// <param name="data">The data. </param>
        public void SetNavigationState(string data)
        {
            var frameDescription = DataContractSerialization.Deserialize<MtFrameDescription>(data, MtSuspensionManager.KnownTypes.ToArray());

            _pages = frameDescription.PageStack;
            CurrentIndex = frameDescription.CurrentPageIndex;

            if (CurrentIndex != -1)
            {
                ContentGrid.Children.Add(CurrentPage.GetPage(this).InternalPage);
                RaisePageOnNavigatedTo(CurrentPage, NavigationMode.Back);
            }
        }

        /// <summary>Used to serialize the current page stack (used in the SuspensionManager). </summary>
        /// <returns>The data. </returns>
        public string GetNavigationState()
        {
            RaisePageOnNavigatedFrom(CurrentPage, NavigationMode.Forward);

            // remove pages which do not support tombstoning
            var pagesToSerialize = _pages;
            var currentIndexToSerialize = CurrentIndex;
            var firstPageToRemove = _pages.FirstOrDefault(p =>
            {
                var page = p.GetPage(this);
                return !page.IsSuspendable;
            });

            if (firstPageToRemove != null)
            {
                var index = pagesToSerialize.IndexOf(firstPageToRemove);
                pagesToSerialize = _pages.Take(index).ToList();
                currentIndexToSerialize = index - 1;
            }

            var output = DataContractSerialization.Serialize(
                new MtFrameDescription { CurrentPageIndex = currentIndexToSerialize, PageStack = pagesToSerialize },
                true, MtSuspensionManager.KnownTypes.ToArray());

            return output;
        }

        /// <summary>Called when a new page has been created. </summary>
        /// <param name="sender">The frame. </param>
        /// <param name="page">The created page. </param>
        protected virtual void OnPageCreated(object sender, object page)
        {
            // Must be empty. 
        }

        /// <summary>Called when navigated to another page. </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="args">The args. </param>
        protected virtual void OnNavigated(object sender, MtNavigationEventArgs args)
        {
            // Must be empty. 
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InternalFrame = (Frame)GetTemplateChild("Frame");
        }

        private async Task<bool> NavigateAsync(MtPageDescription newPage)
        {
            try
            {
                IsNavigating = true;

                var currentPage = CurrentPage;
                if (currentPage != null)
                {
                    if (await RaisePageOnNavigatingFromAsync(CurrentPage, newPage, NavigationMode.New))
                        return false;
                }

                RemoveForwardStack();
                await NavigateWithAnimationsAndCallbacksAsync(NavigationMode.New, currentPage, newPage, CurrentIndex + 1);

                return true;
            }
            finally
            {
                IsNavigating = false;
            }
        }

        /// <exception cref="InvalidOperationException">The frame cannot go forward or back</exception>
        private async Task GoForwardOrBackAsync(NavigationMode navigationMode)
        {
            if (navigationMode == NavigationMode.Forward ? CanGoForward : CanGoBack)
            {
                var currentPage = CurrentPage;
                var nextPageIndex = CurrentIndex + (navigationMode == NavigationMode.Forward ? 1 : -1);
                var nextPage = _pages[nextPageIndex];

                await NavigateWithAnimationsAndCallbacksAsync(navigationMode, currentPage, nextPage, nextPageIndex);

                if (navigationMode == NavigationMode.Back && DisableForwardStack)
                    RemoveForwardStack();
            }
            else
                throw new InvalidOperationException("The frame cannot go forward or back");
        }

        private async Task NavigateWithAnimationsAndCallbacksAsync(NavigationMode navigationMode,
            MtPageDescription currentPage, MtPageDescription nextPage, int nextPageIndex)
        {
            var pageAnimation = ActualPageAnimation;
            var insertionMode = pageAnimation != null ? pageAnimation.PageInsertionMode : PageInsertionMode.Sequential;

            ContentGrid.IsHitTestVisible = false;

            AddNewPageToGridIfNotSequential(insertionMode, nextPage);

            await AnimateNavigatedFromIfCurrentPageNotNull(pageAnimation, navigationMode, insertionMode, currentPage, nextPage);

            SwitchPagesIfSequential(insertionMode, currentPage, nextPage);
            ChangeCurrentPageAndRaiseNavigationEvents(navigationMode, currentPage, nextPage, nextPageIndex);

            await AnimateNavigatedToAndRemoveCurrentPageAsync(pageAnimation, navigationMode, insertionMode, currentPage, nextPage);

            ContentGrid.IsHitTestVisible = true;
            
            ReleasePageIfNecessary(currentPage);
        }

        private void ReleasePageIfNecessary(MtPageDescription page)
        {
            if (page != null && (page.Page.NavigationCacheMode == NavigationCacheMode.Disabled || DisableCache))
                page.ReleasePage();
        }

        private void AddNewPageToGridIfNotSequential(PageInsertionMode insertionMode, MtPageDescription newPage)
        {
            if (insertionMode == PageInsertionMode.ConcurrentAbove)
                ContentGrid.Children.Add(newPage.GetPage(this).InternalPage);
            else if (insertionMode == PageInsertionMode.ConcurrentBelow)
                ContentGrid.Children.Insert(0, newPage.GetPage(this).InternalPage);
        }

        private async Task AnimateNavigatedFromIfCurrentPageNotNull(IPageAnimation pageAnimation, NavigationMode navigationMode,
            PageInsertionMode insertionMode, MtPageDescription currentPage, MtPageDescription newPage)
        {
            if (currentPage != null)
            {
                if (insertionMode != PageInsertionMode.Sequential)
                {
                    await AnimateNavigatingFromAsync(pageAnimation, navigationMode,
                        currentPage.GetPage(this).ActualAnimationContext,
                        insertionMode != PageInsertionMode.Sequential ? newPage.GetPage(this).ActualAnimationContext : null);
                }
                else
                {
                    await AnimateNavigatingFromAsync(pageAnimation, navigationMode, currentPage.GetPage(this).ActualAnimationContext, null);
                }
            }
        }

        private void ChangeCurrentPageAndRaiseNavigationEvents(NavigationMode navigationMode, MtPageDescription currentPage,
            MtPageDescription newPage, int nextPageIndex)
        {
            if (currentPage != null)
                RaisePageOnNavigatedFrom(currentPage, navigationMode);

            if (navigationMode == NavigationMode.New)
                _pages.Add(newPage);
            CurrentIndex = nextPageIndex;

            RaisePageOnNavigatedTo(newPage, navigationMode);
            ((CommandBase)GoBackCommand).RaiseCanExecuteChanged();
        }

        private void SwitchPagesIfSequential(PageInsertionMode insertionMode, MtPageDescription currentPage, MtPageDescription newPage)
        {
            if (insertionMode == PageInsertionMode.Sequential)
            {
                if (currentPage != null)
                    ContentGrid.Children.Remove(currentPage.GetPage(this).InternalPage);
                ContentGrid.Children.Add(newPage.GetPage(this).InternalPage);
            }
        }

        private async Task AnimateNavigatedToAndRemoveCurrentPageAsync(IPageAnimation pageAnimation, NavigationMode navigationMode, PageInsertionMode insertionMode, MtPageDescription currentPage, MtPageDescription newPage)
        {
            if (currentPage != null)
            {
                if (insertionMode != PageInsertionMode.Sequential)
                {
                    await AnimateNavigatedToAsync(pageAnimation, navigationMode,
                        currentPage.GetPage(this).ActualAnimationContext,
                        newPage.GetPage(this).ActualAnimationContext);

                    ContentGrid.Children.Remove(currentPage.GetPage(this).InternalPage);
                }
                else
                {
                    await AnimateNavigatedToAsync(pageAnimation, navigationMode, null,
                        newPage.GetPage(this).ActualAnimationContext);
                }
            }
        }

        private async Task AnimateNavigatingFromAsync(IPageAnimation pageAnimation, NavigationMode navigationMode, FrameworkElement previousPage, FrameworkElement nextPage)
        {
            if (IsFirstPage && ShowNavigationOnAppInAndOut && navigationMode == NavigationMode.Back)
                return;

            if (pageAnimation != null)
            {
                if (navigationMode == NavigationMode.Back)
                    await pageAnimation.AnimateBackwardNavigatingFromAsync(previousPage, nextPage);
                else if (navigationMode != NavigationMode.Refresh)
                    await pageAnimation.AnimateForwardNavigatingFromAsync(previousPage, nextPage);
            }
        }

        private async Task AnimateNavigatedToAsync(IPageAnimation pageAnimation, NavigationMode navigationMode, FrameworkElement previousPage, FrameworkElement nextPage)
        {
            if (IsFirstPage && ShowNavigationOnAppInAndOut && (navigationMode == NavigationMode.New || navigationMode == NavigationMode.Forward))
            {
                if (nextPage != null)
                    nextPage.Opacity = 1;
                return;
            }

            if (pageAnimation != null)
            {
                if (navigationMode == NavigationMode.Back)
                    await pageAnimation.AnimateBackwardNavigatedToAsync(previousPage, nextPage);
                else if (navigationMode != NavigationMode.Refresh)
                    await pageAnimation.AnimateForwardNavigatedToAsync(previousPage, nextPage);
            }

            if (nextPage != null)
                nextPage.Opacity = 1;
        }

        private void OnVisibilityChanged(object sender, VisibilityChangedEventArgs args)
        {
            if (CurrentPage != null)
                CurrentPage.GetPage(this).OnVisibilityChanged(args);
        }

        private void RemoveForwardStack()
        {
            for (var i = _pages.Count - 1; i > CurrentIndex; i--)
            {
                var page = _pages[i];
                page.ReleasePage();

                _pages.Remove(page);
            }
        }

        private void RaisePageOnNavigatedFrom(MtPageDescription description, NavigationMode mode)
        {
            var page = description.GetPage(this);

            var args = new MtNavigationEventArgs();
            args.Content = page;
            args.SourcePageType = description.Type;
            args.Parameter = description.Parameter;
            args.NavigationMode = mode;

            page.OnNavigatedFromCore(args);
        }

        private async Task<bool> RaisePageOnNavigatingFromAsync(MtPageDescription currentPage, MtPageDescription targetPage, NavigationMode mode)
        {
            var page = currentPage.GetPage(this);

            var args = new MtNavigatingCancelEventArgs();
            args.Content = page;
            args.SourcePageType = currentPage.Type;
            args.NavigationMode = mode;
            args.Parameter = currentPage.Parameter;

            await page.OnNavigatingFromCoreAsync(args);

            if (!args.Cancel && targetPage != null)
            {
                var args2 = new MtNavigatingCancelEventArgs();
                args2.SourcePageType = targetPage.Type;
                args2.NavigationMode = mode;
                args2.Parameter = targetPage.Parameter;

                var copy = Navigating;
                if (copy != null)
                {
                    copy(this, args2);
                    args.Cancel = args2.Cancel;
                }
            }

            return args.Cancel;
        }

        private void RaisePageOnNavigatedTo(MtPageDescription description, NavigationMode mode)
        {
            var page = description.GetPage(this);

            var args = new MtNavigationEventArgs();
            args.Content = page;
            args.SourcePageType = description.Type;
            args.Parameter = description.Parameter;
            args.NavigationMode = mode;
            page.OnNavigatedToCore(args);

            var copy = Navigated;
            if (copy != null)
                copy(this, args);

            OnNavigated(this, args);

            if (args.NavigationMode == NavigationMode.New)
                OnPageCreated(this, page);
        }
    }
}

#endif
