//-----------------------------------------------------------------------
// <copyright file="MtFrame.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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

namespace MyToolkit.Paging
{
    public delegate void NavigatedEventHandler(object sender, MtNavigationEventArgs e);

    /// <summary>Navigation container for pages. </summary>
    public class MtFrame : Control, INavigate
    {
        private int _currentIndex = -1;
        private IPageAnimation _pageAnimation;
        private List<MtPageDescription> _pages = new List<MtPageDescription>();

        /// <summary>Gets the current <see cref="MtFrame"/>. </summary>
        public static MtFrame Current
        {
            get { return Window.Current.Content as MtFrame; }
        }

        /// <summary>Gets the current page index. </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
        }

        /// <summary>Gets a value indicating whether the first/root page is visible. </summary>
        public bool IsFirstPage
        {
            get { return _currentIndex == 0; }
        }

        /// <summary>Initializes a new instance of the <see cref="MtFrame"/> class. </summary>
        public MtFrame()
        {
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            Loaded += delegate { Window.Current.VisibilityChanged += OnVisibilityChanged; };
            Unloaded += delegate { Window.Current.VisibilityChanged -= OnVisibilityChanged; };

            GoBackCommand = new RelayCommand(() => GoBackAsync(), () => CanGoBack);

            DefaultStyleKey = typeof(MtFrame);

            if (NavigationKeyHandler.IsRunningOnPhone)
            {
                DisableForwardStack = true;
                _pageAnimation = new TurnstilePageAnimation();
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

        /// <summary>Occurs when navigating to a page. </summary>
        public event NavigatedEventHandler Navigated;

        /// <summary>Gets a command to navigate to the previous page. </summary>
        public ICommand GoBackCommand { get; private set; }

        /// <summary>Gets or sets a value indicating whether the forward stack is disabled 
        /// (default: disabled on Windows Phone, enabled on Windows). </summary>
        public bool DisableForwardStack { get; set; }

        /// <summary>Gets or sets a value indicating whether the cache is fully 
        /// deactivated (should be used only for testing). Default: false. </summary>
        public bool DisableCache { get; set; }

        /// <summary>Gets the page before the current page in the page stack or null if not available. </summary>
        public MtPageDescription PreviousPage { get { return _currentIndex > 0 ? _pages[_currentIndex - 1] : null; } }

        /// <summary>Gets the current page. </summary>
        public MtPageDescription CurrentPage { get { return _pages.Count > 0 ? _pages[_currentIndex] : null; } }

        /// <summary>Gets the page after the current page in the page stack or null if not available. </summary>
        public MtPageDescription NextPage { get { return _currentIndex < _pages.Count - 1 ? _pages[_currentIndex + 1] : null; } }

        /// <summary>Gets the current page animation. Only available when ContentTransitions is null. </summary>
        public IPageAnimation PageAnimation
        {
            get { return ContentTransitions == null ? _pageAnimation : null; }
            set { _pageAnimation = value; }
        }

        /// <summary>Gets or sets a value indicating whether to show the animation when launching, leaving or switching to the app. Default: false. </summary>
        public bool ShowNavigationOnAppInAndOut { get; set; }

        /// <summary>Gets the underlying WinRT frame object. </summary>
        public Frame InternalFrame { get; private set; }

        /// <summary>Gets a value indicating whether it is possible to navigate forward. </summary>
        public bool CanGoForward { get { return _currentIndex < _pages.Count - 1; } }

        /// <summary>Tries to navigate forward to the next page. </summary>
        /// <returns>Returns true if navigating forward, false if cancelled</returns>
        public async Task<bool> GoForwardAsync()
        {
            if (await RaisePageOnNavigatingFromAsync(CurrentPage, NavigationMode.Forward))
                return false;

            GoForwardOrBack(NavigationMode.Forward);
            return true;
        }

        /// <summary>Gets a value indicating whether it is possible to navigate back. </summary>
        public bool CanGoBack { get { return _currentIndex > 0; } }

        /// <summary>Gets a list of the pages in the page stack. </summary>
        public IReadOnlyList<MtPageDescription> Pages { get { return _pages; } }

        /// <summary>Gets the first page of the specified type in the page back stack or null if no page of the type is available. </summary>
        /// <param name="pageType">The page type. </param>
        /// <returns>The page or null if not found. </returns>
        public MtPageDescription GetNearestPageOfTypeInBackStack(Type pageType)
        {
            var index = _currentIndex;
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

        /// <summary>Navigates back to the given index. </summary>
        /// <param name="newIndex">The page index. </param>
        /// <returns>True if the navigation could be performed. </returns>
        public async Task<bool> GoBackToAsync(int newIndex)
        {
            if (newIndex == _currentIndex)
                return true;

            if (newIndex < 0 || newIndex > _currentIndex)
                return false;

            while (_currentIndex != newIndex)
            {
                if (!await GoBackAsync())
                    return false;
            }

            return true;
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
            if (pageIndex == _currentIndex)
                throw new ArgumentException("The current page cannot be removed from the stack. ");

            _pages.RemoveAt(pageIndex);
            if (pageIndex < _currentIndex)
                _currentIndex--;

            return true;
        }

        /// <summary>Navigates back to the first page in the page stack. </summary>
        /// <returns>True if the navigation could be performed. </returns>
        public async Task<bool> GoHomeAsync()
        {
            while (_currentIndex != 0)
            {
                if (!await GoBackAsync())
                    return false;
            }
            return true;
        }

        /// <summary>Gets a value indicating whether the frame is currently navigating to another page. </summary>
        public bool IsNavigating { get; private set; }

        /// <summary>Tries to navigate back to the previous page. </summary>
        /// <returns>Returns true if navigating back, false if cancelled or CanGoBack is false. </returns>
        public async Task<bool> GoBackAsync()
        {
            if (await RaisePageOnNavigatingFromAsync(CurrentPage, NavigationMode.Back))
                return false;

            GoForwardOrBack(NavigationMode.Back);
            return true;
        }

        /// <summary>Initializes the frame and navigates to the given first page. </summary>
        /// <param name="homePageType">The type of the home page. </param>
        /// <param name="parameter">The parameter for the page. </param>
        /// <returns>Always true. </returns>
        public bool Initialize(Type homePageType, object parameter = null)
        {
            NavigateAsync(homePageType, parameter);
            return true;
        }

        /// <summary>Navigates forward to a new instance of the given page type. </summary>
        /// <param name="pageType">The page type. </param>
        /// <returns>Returns true if the navigation process has not been cancelled. </returns>
        public Task<bool> NavigateAsync(Type pageType)
        {
            return NavigateAsync(pageType, null);
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

        private async void GoForwardOrBack(NavigationMode navigationMode)
        {
            if (navigationMode == NavigationMode.Forward ? CanGoForward : CanGoBack)
            {
                var currentPage = CurrentPage;
                _currentIndex += navigationMode == NavigationMode.Forward ? 1 : -1;
                var newPage = CurrentPage;

                if (navigationMode == NavigationMode.Back && DisableForwardStack)
                    RemoveForwardStack();

                await NavigateWithAnimationsAndCallbacksAsync(navigationMode, currentPage, newPage);
            }
            else
                throw new Exception("cannot go forward or back");
        }

        /// <summary>Navigates forward to a new instance of the given page type. </summary>
        /// <param name="pageType">The page type. </param>
        /// <param name="parameter">The page parameter. </param>
        /// <returns>Returns true if the navigation process has not been cancelled. </returns>
        public async Task<bool> NavigateAsync(Type pageType, object parameter)
        {
            var currentPage = CurrentPage;
            if (currentPage != null && await RaisePageOnNavigatingFromAsync(CurrentPage, NavigationMode.New))
                return false;

            RemoveForwardStack();

            var newPage = new MtPageDescription(pageType, parameter);
            await NavigateWithAnimationsAndCallbacksAsync(NavigationMode.New, currentPage, newPage);

            // Destroy current page if cache is disabled
            if (currentPage != null && (currentPage.Page.NavigationCacheMode == NavigationCacheMode.Disabled || DisableCache))
                currentPage.Page = null;

            return true;
        }

        private async Task NavigateWithAnimationsAndCallbacksAsync(NavigationMode navigationMode, MtPageDescription currentPage, MtPageDescription newPage)
        {
            ContentGrid.IsHitTestVisible = false; 

            // navigating from animation
            ContentGrid.Children.Insert(0, newPage.GetPage(this).InternalPage);
            if (currentPage != null)
            {
                await AnimateNavigatingFromAsync(navigationMode,
                    currentPage.GetPage(this).ActualAnimationContext,
                    newPage.GetPage(this).ActualAnimationContext);
            }

            // callbacks
            if (currentPage != null)
            {
                ContentGrid.Children.Move(1, 0);
                RaisePageOnNavigatedFrom(currentPage, navigationMode);
            }

            if (navigationMode == NavigationMode.New)
            {
                _pages.Add(newPage);
                _currentIndex++;
            }

            RaisePageOnNavigatedTo(newPage, navigationMode);
            ((RelayCommand)GoBackCommand).RaiseCanExecuteChanged();

            // navigated to animation
            if (currentPage != null)
            {
                await AnimateNavigatedToAsync(navigationMode,
                    currentPage.GetPage(this).ActualAnimationContext,
                    newPage.GetPage(this).ActualAnimationContext);
                ContentGrid.Children.Remove(currentPage.GetPage(this).InternalPage);
            }

            ContentGrid.IsHitTestVisible = true; 
        }

        private async Task AnimateNavigatingFromAsync(NavigationMode navigationMode, FrameworkElement previousPage, FrameworkElement nextPage)
        {
            if (IsFirstPage && ShowNavigationOnAppInAndOut && navigationMode == NavigationMode.Back)
                return;

            if (PageAnimation != null)
            {
                if (navigationMode == NavigationMode.Back)
                    await PageAnimation.NavigatingFromBackward(previousPage, nextPage);
                else if (navigationMode != NavigationMode.Refresh)
                    await PageAnimation.NavigatingFromForward(previousPage, nextPage);
            }
        }

        private async Task AnimateNavigatedToAsync(NavigationMode navigationMode, FrameworkElement previousPage, FrameworkElement nextPage)
        {
            if (IsFirstPage && ShowNavigationOnAppInAndOut &&
                (navigationMode == NavigationMode.New || navigationMode == NavigationMode.Forward))
            {
                nextPage.Opacity = 1;
                return;
            }

            if (PageAnimation != null)
            {
                if (navigationMode == NavigationMode.Back)
                    await PageAnimation.NavigatedToBackward(previousPage, nextPage);
                else if (navigationMode != NavigationMode.Refresh)
                    await PageAnimation.NavigatedToForward(previousPage, nextPage);
            }

            nextPage.Opacity = 1;
        }

        /// <summary>Navigates to the page of the given type. </summary>
        /// <param name="sourcePageType">The page type. </param>
        public bool Navigate(Type sourcePageType)
        {
            NavigateAsync(sourcePageType);
            return true;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InternalFrame = (Frame)GetTemplateChild("Frame");
        }

        private void OnVisibilityChanged(object sender, VisibilityChangedEventArgs args)
        {
            if (CurrentPage != null)
                CurrentPage.GetPage(this).OnVisibilityChanged(args);
        }

        private void RemoveForwardStack()
        {
            for (var i = _pages.Count - 1; i > _currentIndex; i--)
            {
                var page = _pages[i];
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

        private async Task<bool> RaisePageOnNavigatingFromAsync(MtPageDescription description, NavigationMode mode)
        {
            var page = description.GetPage(this);

            var args = new MtNavigatingCancelEventArgs();
            args.Content = page;
            args.SourcePageType = description.Type;
            args.NavigationMode = mode;

            IsNavigating = true;
            await page.OnNavigatingFromCoreAsync(args);
            IsNavigating = false;

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

        /// <summary>Used set the serialized the current page stack (used in the SuspensionManager). </summary>
        /// <param name="data">The data. </param>
        public void SetNavigationState(string data)
        {
            var frameDescription = DataContractSerialization.Deserialize<MtFrameDescription>(data, MtSuspensionManager.KnownTypes.ToArray());

            _pages = frameDescription.PageStack;
            _currentIndex = frameDescription.PageIndex;

            if (_currentIndex != -1)
            {
                Content = CurrentPage.GetPage(this).InternalPage;
                RaisePageOnNavigatedTo(CurrentPage, NavigationMode.Back);
            }
        }

        /// <summary>Used to serialize the current page stack (used in the SuspensionManager). </summary>
        /// <returns>The data. </returns>
        public string GetNavigationState()
        {
            //RaisePageOnNavigatingFromAsync(CurrentPage, NavigationMode.Forward);
            RaisePageOnNavigatedFrom(CurrentPage, NavigationMode.Forward);

            // remove pages which do not support tombstoning
            var pagesToSerialize = _pages;
            var currentIndexToSerialize = _currentIndex;
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
                new MtFrameDescription { PageIndex = currentIndexToSerialize, PageStack = pagesToSerialize },
                true, MtSuspensionManager.KnownTypes.ToArray());

            return output;
        }

        /// <summary>Gets the number of pages in the page back stack. </summary>
        public int BackStackDepth
        {
            get { return _currentIndex + 1; }
        }
    }
}
