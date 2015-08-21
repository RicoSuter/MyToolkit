//-----------------------------------------------------------------------
// <copyright file="SearchHamburgerItem.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
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
        private readonly SearchBox _searchBox;

        /// <summary>Initializes a new instance of the <see cref="SearchHamburgerItem"/> class.</summary>
        public SearchHamburgerItem()
        {
            _searchBox = new SearchBox();
            _searchBox.Loaded += delegate { _searchBox.PlaceholderText = PlaceholderText; };
            _searchBox.QuerySubmitted += OnQuerySubmitted;
            _searchBox.GotFocus += OnGotFocus;

            Content = _searchBox;
            Icon = new SymbolIcon(Symbol.Find);

            CanBeSelected = false;
            ShowContentIcon = false;

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
        public event TypedEventHandler<SearchBox, SearchBoxQuerySubmittedEventArgs> QuerySubmitted
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

        private void OnQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            _searchBox.QueryText = "";

            var hamburger = sender.GetVisualParentOfType<Hamburger>();
            hamburger.IsPaneOpen = false;
        }
    }
}