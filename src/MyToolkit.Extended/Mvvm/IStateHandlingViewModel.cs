//-----------------------------------------------------------------------
// <copyright file="IStateHandlingViewModel.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using MyToolkit.Paging;

namespace MyToolkit.Mvvm
{
    /// <summary>Interface of a view model which can save and load its state. </summary>
    public interface IStateHandlingViewModel
    {
        /// <summary>Used to load the saved state when the page has been reactivated. </summary>
        /// <param name="pageState">The saved page state. </param>
        void OnLoadState(MtLoadStateEventArgs pageState);

        /// <summary>Used to save the state when the page gets suspended. </summary>
        /// <param name="pageState">The dictionary to save the page state into. </param>
        void OnSaveState(MtSaveStateEventArgs pageState);
    }
}

#endif