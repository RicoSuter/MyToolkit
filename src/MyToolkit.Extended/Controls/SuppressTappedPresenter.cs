//-----------------------------------------------------------------------
// <copyright file="SuppressTappedPresenter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
    /// <summary>Control which suppresses all tapped events. </summary>
    public sealed class SuppressTappedPresenter : ContentPresenter
    {
        /// <summary>Initializes a new instance of the <see cref="SuppressTappedPresenter"/> class. </summary>
        public SuppressTappedPresenter()
        {
            Tapped += delegate(object sender, TappedRoutedEventArgs args) { args.Handled = true; };
        }
    }
}

#endif