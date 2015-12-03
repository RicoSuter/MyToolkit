#if WINRT || WINDOWS_UAP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Paging;
using MyToolkit.Serialization;

namespace MyToolkit.Extended.Paging.Handlers
{
    internal class PageStackManager
    {
        private int _currentIndex = -1;
        private List<MtPageDescription> _pages = new List<MtPageDescription>();

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

        public bool AutomaticBackButtonHandling { get; set; }

        public bool IsFirstPage
        {
            get { return CurrentIndex == 0; }
        }

        public MtPageDescription PreviousPage
        {
            get { return CurrentIndex > 0 ? _pages[CurrentIndex - 1] : null; }
        }

        public MtPageDescription CurrentPage
        {
            get { return _pages.Count > 0 ? _pages[CurrentIndex] : null; }
        }

        public MtPageDescription NextPage
        {
            get { return CurrentIndex < _pages.Count - 1 ? _pages[CurrentIndex + 1] : null; }
        }

        public bool CanGoForward
        {
            get { return CurrentIndex < _pages.Count - 1; }
        }

        public bool CanGoBack
        {
            get { return CurrentIndex > 0; }
        }

        public IReadOnlyList<MtPageDescription> Pages
        {
            get { return _pages; }
        }

        public int BackStackDepth
        {
            get { return CurrentIndex + 1; } // TODO: Do not add 1 => breaking change!!!
        }

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

        public MtPageDescription GetPageAt(int index)
        {
            return _pages[index];
        }

        public int GetPageIndex(MtPageDescription pageDescription)
        {
            return _pages.IndexOf(pageDescription);
        }

        /// <exception cref="ArgumentException">The current page cannot be removed from the stack. </exception>
        public bool RemovePageFromStack(MtPageDescription pageDescription)
        {
            var index = GetPageIndex(pageDescription);
            if (index >= 0)
            {
                RemovePageFromStackAt(index);
                return true;
            }
            return false;
        }

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

        public async Task<bool> MoveToTop(MtPageDescription page, Func<MtPageDescription, Task<bool>> action)
        {
            if (CurrentPage == page)
                return true;

            var index = _pages.IndexOf(page);
            if (index != -1)
            {
                _pages.RemoveAt(index);
                _currentIndex--;

                if (await action(page))
                    return true;

                _pages.Insert(index, page);
            }
            return false;
        }

        public void SetNavigationState(string data)
        {
            var frameDescription = DataContractSerialization.Deserialize<MtFrameDescription>(data, MtSuspensionManager.KnownTypes.ToArray());

            _pages = frameDescription.PageStack;
            CurrentIndex = frameDescription.CurrentPageIndex;
        }

        public string GetNavigationState(MtFrame frame)
        {
            // remove pages which do not support tombstoning
            var pagesToSerialize = _pages;
            var currentIndexToSerialize = CurrentIndex;
            var firstPageToRemove = _pages.FirstOrDefault(p =>
            {
                var page = p.GetPage(frame);
                return !page.IsSuspendable;
            });

            if (firstPageToRemove != null)
            {
                var index = pagesToSerialize.IndexOf(firstPageToRemove);
                pagesToSerialize = _pages.Take(index).ToList();
                currentIndexToSerialize = index - 1;
            }

            var output = DataContractSerialization.Serialize(
                new MtFrameDescription
                {
                    CurrentPageIndex = currentIndexToSerialize,
                    PageStack = pagesToSerialize
                },
                true, MtSuspensionManager.KnownTypes.ToArray());

            return output;
        }

        public void ClearBackStack()
        {
            for (var i = _currentIndex - 1; i >= 0; i--)
                RemovePageFromStackAt(i);
        }

        public void ClearForwardStack()
        {
            for (var i = _pages.Count - 1; i > CurrentIndex; i--)
                RemovePageFromStackAt(i);
        }
        
        public void ChangeCurrentPage(MtPageDescription newPage, int nextPageIndex)
        {
            if (_pages.Count <= nextPageIndex)
                _pages.Add(newPage);

            CurrentIndex = nextPageIndex;
        }

        public bool CanGoBackTo(int newPageIndex)
        {
            if (newPageIndex == CurrentIndex)
                return false;

            if (newPageIndex < 0 || newPageIndex > CurrentIndex)
                return false;

            return true; 
        }
    }
}

#endif