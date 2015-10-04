#if WINRT

using System;
using System.Windows.Input;
using MyToolkit.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
    public enum ImageButtonState
    {
        Normal,
        Over,
        Pressed
    }

    public class ImageButton : Control
    {
        public ImageButtonState State { get; private set; }

        public ImageButton()
        {
            DefaultStyleKey = typeof(ImageButton);

            State = ImageButtonState.Normal;

            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerExited += OnPointerExited;
            PointerEntered += OnPointerEntered;

            //Tapped += OnTapped; 
        }

        private ContentPresenter content;
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            content = (ContentPresenter)GetTemplateChild("content");
            content.Content = Content;
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            SetStandardContent();
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            if (args.Pointer.IsInContact)
                SetPressedContent();
            else
                SetOverContent();
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (State == ImageButtonState.Pressed)
                OnTapped(sender, null);
            SetStandardContent();
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            SetPressedContent();
        }


        private void SetStandardContent()
        {
            State = ImageButtonState.Normal;
            content.Content = Content;
        }

        private void SetPressedContent()
        {
            State = ImageButtonState.Pressed;
            if (PressedContent != null)
                content.Content = PressedContent;
        }

        private void SetOverContent()
        {
            State = ImageButtonState.Over;
            if (OverContent != null)
                content.Content = OverContent;
        }


        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var copy = Click;
            if (copy != null)
                copy(this, new RoutedEventArgs());

            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }

        public event RoutedEventHandler Click;


        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ImageButton), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ImageButton), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }


        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(FrameworkElement), typeof(ImageButton), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement Content
        {
            get { return (FrameworkElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }


        public static readonly DependencyProperty PressedContentProperty =
            DependencyProperty.Register("PressedContent", typeof(FrameworkElement), typeof(ImageButton), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement PressedContent
        {
            get { return (FrameworkElement)GetValue(PressedContentProperty); }
            set { SetValue(PressedContentProperty, value); }
        }


        public static readonly DependencyProperty OverContentProperty =
            DependencyProperty.Register("OverContent", typeof(FrameworkElement), typeof(ImageButton), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement OverContent
        {
            get { return (FrameworkElement)GetValue(OverContentProperty); }
            set { SetValue(OverContentProperty, value); }
        }


        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ImageButton), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}

#endif