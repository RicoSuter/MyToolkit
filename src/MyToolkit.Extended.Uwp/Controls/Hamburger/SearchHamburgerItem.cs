//-----------------------------------------------------------------------
// <copyright file="SearchHamburgerItem.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.UI;

namespace MyToolkit.Controls
{
    /// <summary>A hamburger item which shows a search box.</summary>
    public class SearchHamburgerItem : PageHamburgerItem
    {
        private readonly AutoSuggestBox _searchBox;

        /// <summary>Initializes a new instance of the <see cref="SearchHamburgerItem"/> class.</summary>
        public SearchHamburgerItem()
        {
            _searchBox = new AutoSuggestBox();
            _searchBox.QueryIcon = new SymbolIcon { Symbol = Symbol.Find };
            _searchBox.Loaded += delegate { _searchBox.PlaceholderText = PlaceholderText; };
            _searchBox.QuerySubmitted += OnQuerySubmitted;
            _searchBox.GotFocus += OnGotFocus;

            Content = _searchBox;
            Icon = new SymbolIcon(Symbol.Find);

            CanBeSelected = false;
            ShowContentIcon = false;
            PlaceholderText = string.Empty;

            Click += (sender, args) =>
            {
                args.Hamburger.IsPaneOpen = true;
                args.Hamburger.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _searchBox.Focus(FocusState.Programmatic);
                });
            };
        }

        /// <summary>Gets or sets the text that is displayed in the control until the value is changed by a user action or some other operation.</summary>
        public string PlaceholderText { get; set; }

        /// <summary>Occurs when the user submits a search query.</summary>
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted
        {
            add { _searchBox.QuerySubmitted += value; }
            remove { _searchBox.QuerySubmitted -= value; }
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (CanBeSelected)
            {
                var hamburger = _searchBox.GetVisualParentOfType<Hamburger>();
                hamburger.SelectedItem = this;
            }
        }

        private void OnQuerySubmitted(AutoSuggestBox box, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _searchBox.Text = "";

            var hamburger = box.GetVisualParentOfType<Hamburger>();
            hamburger.IsPaneOpen = false;
        }
    }
}