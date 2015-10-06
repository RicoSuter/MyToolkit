//-----------------------------------------------------------------------
// <copyright file="RolloverButton.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace MyToolkit.Controls
{
    [ContentProperty(Name = "Content")]
    public sealed class RolloverButton : Control
    {
        public RolloverButton()
        {
            this.DefaultStyleKey = typeof(RolloverButton);
        }

        public event RoutedEventHandler Click;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var root = (Border) GetTemplateChild("LayoutRoot");
            root.Tapped += OnTapped;

            root.PointerEntered += delegate { VisualStateManager.GoToState(this, "PointerOver", true); };
            root.PointerExited += delegate { VisualStateManager.GoToState(this, "Normal", true); };
            root.PointerPressed += delegate { VisualStateManager.GoToState(this, "Pressed", true); };
            root.PointerReleased += delegate { VisualStateManager.GoToState(this, "Normal", true); };
        }

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var copy = Click;
            if (copy != null)
                copy(sender, e);

            if (Command != null && Command.CanExecute(sender))
                Command.Execute(sender);
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(RolloverButton), null);

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(RolloverButton), null);
    }
}

#endif