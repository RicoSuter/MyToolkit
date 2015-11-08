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
        /// <summary>Initializes a new instance of the <see cref="SearchHamburgerItem"/> class.</summary>
        public SearchHamburgerItem()
        {
            AutoSuggestBox = new AutoSuggestBox();
            AutoSuggestBox.QueryIcon = new SymbolIcon { Symbol = Symbol.Find };
            AutoSuggestBox.AutoMaximizeSuggestionArea = false;
            AutoSuggestBox.Loaded += delegate { AutoSuggestBox.PlaceholderText = PlaceholderText; };
            AutoSuggestBox.QuerySubmitted += OnQuerySubmitted;
            AutoSuggestBox.GotFocus += OnGotFocus;

            Content = AutoSuggestBox;
            Icon = new SymbolIcon(Symbol.Find);

            CanBeSelected = false;
            ShowContentIcon = false;
            PlaceholderText = string.Empty;
            
            Click += (sender, args) =>
            {
                args.Hamburger.IsPaneOpen = true;
                args.Hamburger.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    AutoSuggestBox.Focus(FocusState.Programmatic);
                });
            };
        }

        /// <summary>Gets the search box.</summary>
        public AutoSuggestBox AutoSuggestBox { get; private set; }

        /// <summary>Gets or sets the text that is displayed in the control until the value is changed by a user action or some other operation.</summary>
        public string PlaceholderText { get; set; }

        /// <summary>Occurs when the user submits a search query.</summary>
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted
        {
            add { AutoSuggestBox.QuerySubmitted += value; }
            remove { AutoSuggestBox.QuerySubmitted -= value; }
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (CanBeSelected)
            {
                var hamburger = AutoSuggestBox.GetVisualParentOfType<Hamburger>();
                hamburger.SelectedItem = this;
            }
        }

        private void OnQuerySubmitted(AutoSuggestBox box, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            AutoSuggestBox.Text = "";

            var hamburger = box.GetVisualParentOfType<Hamburger>();
            hamburger.IsPaneOpen = false;
        }
    }
}