//-----------------------------------------------------------------------
// <copyright file="IPageAnmiation.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyToolkit.Paging.Animations
{
    /// <summary>Initializes a new instance of the <see cref="IPageAnimation"/> class. </summary>
    public interface IPageAnimation
    {
        /// <summary>Gets the insertion mode for the next page.</summary>
        PageInsertionMode PageInsertionMode { get; }

        /// <summary>Animates for navigating forward from a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        Task AnimateForwardNavigatingFromAsync(FrameworkElement previousPage, FrameworkElement nextPage);

        /// <summary>Animates for navigating forward to a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        Task AnimateForwardNavigatedToAsync(FrameworkElement previousPage, FrameworkElement nextPage);

        /// <summary>Animates for navigating backward from a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        Task AnimateBackwardNavigatingFromAsync(FrameworkElement previousPage, FrameworkElement nextPage);

        /// <summary>Animates for navigating backward to a page. </summary>
        /// <param name="previousPage">The previous page. </param>
        /// <param name="nextPage">The next page. </param>
        /// <returns>The task. </returns>
        Task AnimateBackwardNavigatedToAsync(FrameworkElement previousPage, FrameworkElement nextPage);
    }
}

#endif