using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
    public class SearchHamburgerItem : HamburgerItem
    {
        private readonly TextBox _textBox;

        public event EventHandler<HamburgerItemSearchEventArgs> Search;

        public SearchHamburgerItem()
        {
            _textBox = new TextBox(); 
            _textBox.KeyUp += OnTextBoxKeyUp;
            _textBox.GotFocus += OnTextBoxGotFocus;

            Content = _textBox;
            Icon = '\uE11A'.ToString();
            ShowIconWhenPaneIsOpen = false;
            Selected += async (sender, args) =>
            {
                args.Hamburger.IsPaneOpen = true;
                await args.Hamburger.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ((TextBox)Content).Focus(FocusState.Programmatic);
                });
            };
        }

        private void OnTextBoxGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO: Not working
            IsSelected = true;
        }

        private void OnTextBoxKeyUp(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key == VirtualKey.Enter)
            {
                var copy = Search;
                if (copy != null)
                    copy(this, new HamburgerItemSearchEventArgs());

                _textBox.Text = "";
                // TODO: Close pane...
            }
        }
    }
}