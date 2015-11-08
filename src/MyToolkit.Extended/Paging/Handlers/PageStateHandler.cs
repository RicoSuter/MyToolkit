//-----------------------------------------------------------------------
// <copyright file="PageStateHandler.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Paging.Handlers
{
    internal class PageStateHandler
    {
        private readonly MtPage _page;
        private bool _stateLoaded = false; 

        public string PageKey { get; internal set; }

        internal event EventHandler<MtLoadStateEventArgs> LoadState;
        internal event EventHandler<MtSaveStateEventArgs> SaveState;

        public PageStateHandler(MtPage page, string pageKey)
        {
            _page = page;
            PageKey = pageKey;
        }

        public void OnNavigatedTo(MtNavigationEventArgs e)
        {
            if (!_stateLoaded)
            {
                var frameState = MtSuspensionManager.SessionStateForFrame(_page.Frame);

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

                _stateLoaded = true; 
            }
        }

        public void OnNavigatedFrom(MtNavigationEventArgs e)
        {
            if (_page.Frame.DisableForwardStack && e.NavigationMode == NavigationMode.Back)
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

#endif