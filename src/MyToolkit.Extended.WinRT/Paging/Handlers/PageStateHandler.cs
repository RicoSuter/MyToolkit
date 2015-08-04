using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Paging.Handlers
{
    public class PageStateHandler
    {
        private readonly MtPage _page;

        public string PageKey { get; private set; }

        internal event EventHandler<MtLoadStateEventArgs> LoadState;
        internal event EventHandler<MtSaveStateEventArgs> SaveState;

        public PageStateHandler(MtPage page)
        {
            _page = page; 
        }

        public void OnNavigatedTo(MtNavigationEventArgs e)
        {
            if (PageKey != null) // new instance
                return;

            var frameState = MtSuspensionManager.SessionStateForFrame(_page.Frame);
            PageKey = "Page" + _page.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                var nextPageKey = PageKey;
                var nextPageIndex = _page.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page" + nextPageIndex;
                }

                // Does not make sense when no page state is available...
                //var args = new MtLoadStateEventArgs(e.Parameter, null);

                //var copy = LoadState;
                //if (copy != null)
                //    copy(this, args);

                _page.LoadState(e.Parameter, null);
                //_page.OnLoadState(args);
            }
            else
            {
                var pageState = (Dictionary<String, Object>)frameState[PageKey];
                var args = new MtLoadStateEventArgs(e.Parameter, pageState);

                var copy = LoadState;
                if (copy != null)
                    copy(this, args);

                _page.LoadState(e.Parameter, pageState);
                _page.OnLoadState(args);
            }
        }

        public void OnNavigatedFrom(MtNavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            var frameState = MtSuspensionManager.SessionStateForFrame(_page.Frame);
            var pageState = new Dictionary<String, Object>();
            var args = new MtSaveStateEventArgs(pageState);

            var copy = SaveState;
            if (copy != null)
                copy(this, args);

            _page.SaveState(pageState);
            _page.OnSaveState(args);
            frameState[PageKey] = pageState;
        }
    }
}
