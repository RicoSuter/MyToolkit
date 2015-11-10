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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Environment;
using MyToolkit.Extended.Paging.Handlers;

namespace MyToolkit.Paging
{
    public delegate void NavigatedEventHandler(object sender, MtNavigationEventArgs e);
    public delegate void NavigatingEventHandler(object sender, MtNavigatingCancelEventArgs e);

    /// <summary>Navigation container for pages. </summary>
    public class MtFrame : Control, INavigate
    {
        private readonly PageStackManager _pageStackManager = new PageStackManager();

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

        /// <summary>Gets or sets a value indicating whether to show the animation when launching, leaving or switching to the app. Default: false. </summary>
        public bool ShowNavigationOnAppInAndOut { get; set; }
        
        /// <summary>Gets or sets a value indicating whether the forward stack is disabled 
        /// (default: disabled on Windows Phone, enabled on Windows). </summary>
        public bool DisableForwardStack { get; set; }

        /// <summary>Gets or sets a value indicating whether the cache is fully 
        /// deactivated (should be used only for testing). Default: false. </summary>
        public bool DisableCache { get; set; }

        /// <summary>Gets the underlying WinRT frame object. </summary>
        public Frame InternalFrame { get; private set; }

        /// <summary>Gets the current <see cref="MtFrame"/>. </summary>
        public static MtFrame Current
        {
            get { return Window.Current.Content as MtFrame; }
        }

#if WINDOWS_UAP

        /// <summary>Gets or sets a value indicating whether the back button is automatically shown and hidden by the frame (default: true).</summary>
        public bool AutomaticBackButtonHandling
        {
            get { return _pageStackManager.AutomaticBackButtonHandling; }
            set { _pageStackManager.AutomaticBackButtonHandling = value; }
        }

#endif

        #region Dependency Properties

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(MtFrame), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the content of the <see cref="MtFrame"/>. </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
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

        #endregion

        /// <summary>Occurs when the frame navigated to another page. </summary>
        public event NavigatedEventHandler Navigated;

        /// <summary>Occurs when the frame navigates to another page. </summary>
        public event NavigatingEventHandler Navigating;

        /// <summary>Gets a command to navigate to the previous page. </summary>
        public ICommand GoBackCommand { get; private set; }
        
        /// <summary>Gets a value indicating whether the first/root page is visible. </summary>
        public bool IsFirstPage
        {
            get { return _pageStackManager.IsFirstPage; }
        }

        /// <summary>Gets the page before the current page in the page stack or null if not available. </summary>
        public MtPageDescription PreviousPage
        {
            get { return _pageStackManager.PreviousPage; }
        }

        /// <summary>Gets the current page. </summary>
        public MtPageDescription CurrentPage
        {
            get { return _pageStackManager.CurrentPage; }
        }

        /// <summary>Gets the page after the current page in the page stack or null if not available. </summary>
        public MtPageDescription NextPage
        {
            get { return _pageStackManager.NextPage; }
        }

        /// <summary>Gets a value indicating whether it is possible to navigate back. </summary>
        public bool CanGoBack
        {
            get { return _pageStackManager.CanGoBack; }
        }

        /// <summary>Gets a value indicating whether it is possible to navigate forward. </summary>
        public bool CanGoForward
        {
            get { return _pageStackManager.CanGoForward; }
        }

        /// <summary>Gets a list of the pages in the page stack. </summary>
        public IReadOnlyList<MtPageDescription> Pages
        {
            get { return _pageStackManager.Pages; }
        }

        /// <summary>Gets the number of pages in the page back stack. </summary>
        public int BackStackDepth
        {
            get { return _pageStackManager.BackStackDepth; }
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
                return currentPage != null &&
                    currentPage.Page != null &&
                    currentPage.Page.PageAnimation != null ? CurrentPage.Page.PageAnimation : PageAnimation;
            }
        }

        /// <summary>Tries to navigate forward to the next page. </summary>
        /// <remarks>After the task has completed the <see cref="Frame"/>'s current page has changed. </remarks>
        /// <returns>Returns true if navigating forward, false if cancelled</returns>
        public Task<bool> GoForwardAsync()
        {
            return RunNavigationWithCheckAsync(async () =>
            {
                if (await RaisePageOnNavigatingFromAsync(CurrentPage, null, NavigationMode.Forward))
                    return false;

                await GoForwardOrBackAsync(NavigationMode.Forward);
                return true;
            });
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
            return _pageStackManager.GetNearestPageOfTypeInBackStack(pageType);
        }

        /// <summary>Navigates back to the given page. </summary>
        /// <param name="pageDescription">The page to navigate to. </param>
        /// <returns>True if the navigation could be performed. </returns>
        public async Task<bool> GoBackToAsync(MtPageDescription pageDescription)
        {
            var index = _pageStackManager.GetPageIndex(pageDescription);
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
        public Task<bool> GoBackToAsync(int newPageIndex)
        {
            if (!_pageStackManager.CanGoBackTo(newPageIndex))
                return Task.FromResult(false);

            return RunNavigationWithCheckAsync(async () =>
            {
                var nextPage = _pageStackManager.GetPageAt(newPageIndex);
                var currentPage = CurrentPage;

                if (await RaisePageOnNavigatingFromAsync(CurrentPage, currentPage, NavigationMode.Back))
                    return false;

                await NavigateWithAnimationsAndCallbacksAsync(NavigationMode.Back, currentPage, nextPage, newPageIndex);

                if (DisableForwardStack)
                    _pageStackManager.ClearForwardStack();

                return true;
            });
        }

        /// <summary>Removes a page from the page stack. </summary>
        /// <param name="pageDescription">The page to remove. </param>
        /// <returns><c>true</c> if the page has been found and was removed; otherwise, <c>false</c>. </returns>
        /// <exception cref="ArgumentException">The current page cannot be removed from the stack. </exception>
        public bool RemovePageFromStack(MtPageDescription pageDescription)
        {
            return _pageStackManager.RemovePageFromStack(pageDescription);
        }

        /// <summary>Removes a page from the page stack. </summary>
        /// <param name="pageIndex">The index of the page page to remove. </param>
        /// <returns><c>true</c> if the page has been found and was removed; otherwise, <c>false</c>. </returns>
        /// <exception cref="ArgumentException">The current page cannot be removed from the stack. </exception>
        public bool RemovePageFromStackAt(int pageIndex)
        {
            return _pageStackManager.RemovePageFromStackAt(pageIndex);
        }

        /// <summary>Gets a value indicating whether the frame is currently navigating to another page. </summary>
        public bool IsNavigating { get; private set; }

        /// <summary>Tries to navigate back to the previous page. </summary>
        /// <remarks>After the task has completed the <see cref="Frame"/>'s current page has changed. </remarks>
        /// <returns>Returns true if navigating back, false if cancelled or CanGoBack is false. </returns>
        public Task<bool> GoBackAsync()
        {
            return RunNavigationWithCheckAsync(async () =>
            {
                if (await RaisePageOnNavigatingFromAsync(CurrentPage, PreviousPage, NavigationMode.Back))
                    return false;

                await GoForwardOrBackAsync(NavigationMode.Back);
                return true;
            });
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

        /// <summary>Navigates forward to a new instance of the given page type.</summary>
        /// <param name="pageType">The page type. </param>
        /// <param name="parameter">The page parameter. </param>
        /// <returns>Returns true if the navigation process has not been cancelled. </returns>
        public async Task<bool> NavigateAsync(Type pageType, object parameter)
        {
            var newPage = new MtPageDescription(pageType, parameter);
            return await NavigateAsync(newPage, NavigationMode.New);
        }

        /// <summary>Navigates forward to the existing page of the given page type or creates a new page instace.</summary>
        /// <remarks>If the page exists it is referenced multiple times in the page stack: 
        /// The <see cref="MtPageDescription"/> is contained multiple times in the page stack.</remarks>
        /// <param name="pageType">The page type. </param>
        /// <param name="pageParamter">The page parameter. </param>
        /// <returns>Returns true if the navigation process has not been cancelled. </returns>
        public async Task<bool> NavigateToExistingOrNewPageAsync(Type pageType, object pageParamter = null)
        {
            var existingPage = GetNearestPageOfTypeInBackStack(pageType);
            if (existingPage != null)
            {
                existingPage.Parameter = pageParamter;
                return await CopyToTopAndNavigateAsync(existingPage);
            }
            return await NavigateAsync(pageType, pageParamter);
        }

        /// <summary>Navigates to the given page and removes the page from the previous position in the page stack.</summary>
        /// <param name="page">The page.</param>
        /// <returns>True if page is now on top of the stack, false when navigation from the current page failed.</returns>
        public Task<bool> MoveToTopAndNavigateAsync(MtPageDescription page)
        {
            return _pageStackManager.MoveToTop(page, async p => await NavigateAsync(p, NavigationMode.Forward));
        }

        /// <summary>Navigates to the given page and copies the page.</summary>
        /// <param name="page">The page.</param>
        /// <returns>True if page is now on top of the stack, false when navigation from the current page failed.</returns>
        public async Task<bool> CopyToTopAndNavigateAsync(MtPageDescription page)
        {
            if (CurrentPage == page)
                return true;

            if (_pageStackManager.Pages.Contains(page))
            {
                if (await NavigateAsync(page, NavigationMode.Forward))
                    return true;
            }

            return false;
        }

        /// <summary>Clears all pages from the page back stack.</summary>
        public void ClearBackStack()
        {
            _pageStackManager.ClearBackStack();
        }

        /// <summary>Used set the serialized the current page stack (used in the SuspensionManager). </summary>
        /// <param name="data">The data. </param>
        public void SetNavigationState(string data)
        {
            _pageStackManager.SetNavigationState(data);
            if (_pageStackManager.CurrentIndex != -1)
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
            return _pageStackManager.GetNavigationState(this);
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
        
        private Task<bool> NavigateAsync(MtPageDescription newPage, NavigationMode navigationMode)
        {
            return RunNavigationWithCheckAsync(async () =>
            {
                var currentPage = CurrentPage;
                if (currentPage != null)
                {
                    if (await RaisePageOnNavigatingFromAsync(CurrentPage, newPage, navigationMode))
                        return false;
                }

                _pageStackManager.ClearForwardStack();

                await NavigateWithAnimationsAndCallbacksAsync(navigationMode, currentPage, newPage, _pageStackManager.CurrentIndex + 1);
                
                return true;
            });
        }

        private async Task<bool> RunNavigationWithCheckAsync(Func<Task<bool>> task)
        {
            if (IsNavigating)
                return false;

            try
            {
                IsNavigating = true;
                return await task();
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
                var nextPageIndex = _pageStackManager.CurrentIndex + (navigationMode == NavigationMode.Forward ? 1 : -1);
                var nextPage = _pageStackManager.Pages[nextPageIndex];

                await NavigateWithAnimationsAndCallbacksAsync(navigationMode, currentPage, nextPage, nextPageIndex);

                if (navigationMode == NavigationMode.Back && DisableForwardStack)
                    _pageStackManager.ClearForwardStack();
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
            var page = newPage.GetPage(this).InternalPage;

            if (!ContentGrid.Children.Contains(page))
            {
                if (insertionMode == PageInsertionMode.ConcurrentAbove)
                    ContentGrid.Children.Add(page);
                else if (insertionMode == PageInsertionMode.ConcurrentBelow)
                    ContentGrid.Children.Insert(0, page);
            }
        }

        private void ChangeCurrentPageAndRaiseNavigationEvents(NavigationMode navigationMode, MtPageDescription currentPage,
            MtPageDescription newPage, int nextPageIndex)
        {
            if (currentPage != null)
                RaisePageOnNavigatedFrom(currentPage, navigationMode);

            _pageStackManager.ChangeCurrentPage(newPage, nextPageIndex);

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
                ContentGrid.UpdateLayout();
            }
        }

        private void OnVisibilityChanged(object sender, VisibilityChangedEventArgs args)
        {
            if (CurrentPage != null)
                CurrentPage.GetPage(this).OnVisibilityChanged(args);
        }

        #region Page Animations

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

        #endregion

        #region Raise Events

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

        private async Task<bool> RaisePageOnNavigatingFromAsync(MtPageDescription currentPage, MtPageDescription nextPage, NavigationMode mode)
        {
            var page = currentPage.GetPage(this);

            var args = new MtNavigatingCancelEventArgs();
            args.Content = page;
            args.SourcePageType = currentPage.Type;
            args.NavigationMode = mode;
            args.Parameter = currentPage.Parameter;

            await page.OnNavigatingFromCoreAsync(args);

            if (!args.Cancel && nextPage != null)
            {
                var args2 = new MtNavigatingCancelEventArgs();
                args2.SourcePageType = nextPage.Type;
                args2.NavigationMode = mode;
                args2.Parameter = nextPage.Parameter;

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

        #endregion
    }
}

#endif