using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Input;
using MyToolkit.UI;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Paging
{
	[Windows.Foundation.Metadata.WebHostHidden]
	public class LayoutAwarePage : SuspendablePage
    {
        private List<Control> layoutAwareControls;

		public bool UseBackKeyToGoBack { get; set; }
		public bool UseBackKeyToGoBackInWebView { get; set; }

		public LayoutAwarePage()
		{
			UseBackKeyToGoBack = true; 
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) 
				return;

            Loaded += (sender, e) =>
            {
                StartLayoutUpdates(InternalPage, e);

                if (ActualHeight == Window.Current.Bounds.Height &&
                    ActualWidth == Window.Current.Bounds.Width)
                {
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed += OnPointerPressed;
                }
            };

            Unloaded += (sender, e) =>
            {
                StopLayoutUpdates(InternalPage, e);
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= OnAcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -= OnPointerPressed;
            };
        }

        #region Navigation support

		protected virtual async Task<bool> GoHomeAsync(object sender, RoutedEventArgs e)
        {
            if (Frame != null)
            {
				while (Frame.CanGoBack)
				{
					if (!await Frame.GoBackAsync())
						return false;
				}
				return true;
			}
			return false;
		}

        protected virtual void GoBack(object sender, RoutedEventArgs e)
        {
			if (Frame != null && Frame.CanGoBack) 
				Frame.GoBackAsync();
        }

        protected virtual void GoForward(object sender, RoutedEventArgs e)
        {
			if (Frame != null && Frame.CanGoForward)
				Frame.GoForwardAsync();
        }

		protected virtual void OnKeyActivated(AcceleratorKeyEventArgs args)
		{

		}

		protected virtual void OnKeyUp(AcceleratorKeyEventArgs args)
		{
			
		}

        private void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
	        OnKeyActivated(args);
			if (args.KeyStatus.IsKeyReleased)
				OnKeyUp(args);

			if (args.Handled)
				return;
			
			var virtualKey = args.VirtualKey;
			if (args.KeyStatus.IsKeyReleased)
			{
				var isLeftOrRightKey = (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right || 
					(int) virtualKey == 166 || (int) virtualKey == 167);
				var isBackKey = virtualKey == VirtualKey.Back;

				if (isLeftOrRightKey || isBackKey)
				{
					if (PopupHelper.IsPopupVisible)
						return; 

					var element = FocusManager.GetFocusedElement();
					if (element is FrameworkElement && PopupHelper.IsInPopup((FrameworkElement)element))
						return;

					if (isBackKey)
					{
						if (UseBackKeyToGoBack)
						{
							if (!(element is TextBox) && !(element is PasswordBox) && 
								(UseBackKeyToGoBackInWebView || !(element is WebView)) && Frame.CanGoBack)
							{
								args.Handled = true;
								GoBack(this, new RoutedEventArgs());
							}
						}
					}
					else
					{
						var altKey = Keyboard.IsAltKeyDown;
						var controlKey = Keyboard.IsControlKeyDown;
						var shiftKey = Keyboard.IsShiftKeyDown;
						
						var noModifiers = !altKey && !controlKey && !shiftKey;
						var onlyAlt = altKey && !controlKey && !shiftKey;

						if (((int)virtualKey == 166 && noModifiers) || (virtualKey == VirtualKey.Left && onlyAlt))
						{
							args.Handled = true;
							GoBack(this, new RoutedEventArgs());
						}
						else if (((int)virtualKey == 167 && noModifiers) || (virtualKey == VirtualKey.Right && onlyAlt))
						{
							args.Handled = true;
							GoForward(this, new RoutedEventArgs());
						}
					}
				}
			}
        }

        private void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            var properties = args.CurrentPoint.Properties;
			if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed || properties.IsMiddleButtonPressed) 
				return;

            var backPressed = properties.IsXButton1Pressed;
            var forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                args.Handled = true;
                if (backPressed) 
					GoBack(this, new RoutedEventArgs());
                if (forwardPressed) 
					GoForward(this, new RoutedEventArgs());
            }
        }

        #endregion

        #region Visual state switching

        /// <summary>
        /// Invoked as an event handler, typically on the <see cref="FrameworkElement.Loaded"/>
        /// event of a <see cref="Control"/> within the page, to indicate that the sender should
        /// start receiving visual state management changes that correspond to application view
        /// state changes.
        /// </summary>
        /// <param name="sender">Instance of <see cref="Control"/> that supports visual state
        /// management corresponding to view states.</param>
        /// <param name="e">Event data that describes how the request was made.</param>
        /// <remarks>The current view state will immediately be used to set the corresponding
        /// visual state when layout updates are requested.  A corresponding
        /// <see cref="FrameworkElement.Unloaded"/> event handler connected to
        /// <see cref="StopLayoutUpdates"/> is strongly encouraged.  Instances of
        /// <see cref="LayoutAwarePage"/> automatically invoke these handlers in their Loaded and
        /// Unloaded events.</remarks>
        /// <seealso cref="DetermineVisualState"/>
        /// <seealso cref="InvalidateVisualState"/>
        public void StartLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            if (this.layoutAwareControls == null)
            {
                // Start listening to view state changes when there are controls interested in updates
                Window.Current.SizeChanged += this.WindowSizeChanged;
                this.layoutAwareControls = new List<Control>();
            }
            this.layoutAwareControls.Add(control);

            // Set the initial visual state of the control
            VisualStateManager.GoToState(control, DetermineVisualState(ApplicationView.Value), false);
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.InvalidateVisualState();
        }

        /// <summary>
        /// Invoked as an event handler, typically on the <see cref="FrameworkElement.Unloaded"/>
        /// event of a <see cref="Control"/>, to indicate that the sender should start receiving
        /// visual state management changes that correspond to application view state changes.
        /// </summary>
        /// <param name="sender">Instance of <see cref="Control"/> that supports visual state
        /// management corresponding to view states.</param>
        /// <param name="e">Event data that describes how the request was made.</param>
        /// <remarks>The current view state will immediately be used to set the corresponding
        /// visual state when layout updates are requested.</remarks>
        /// <seealso cref="StartLayoutUpdates"/>
        public void StopLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null || this.layoutAwareControls == null) return;
            this.layoutAwareControls.Remove(control);
            if (this.layoutAwareControls.Count == 0)
            {
                // Stop listening to view state changes when no controls are interested in updates
                this.layoutAwareControls = null;
                Window.Current.SizeChanged -= this.WindowSizeChanged;
            }
        }

        /// <summary>
        /// Translates <see cref="ApplicationViewState"/> values into strings for visual state
        /// management within the page.  The default implementation uses the names of enum values.
        /// Subclasses may override this method to control the mapping scheme used.
        /// </summary>
        /// <param name="viewState">View state for which a visual state is desired.</param>
        /// <returns>Visual state name used to drive the
        /// <see cref="VisualStateManager"/></returns>
        /// <seealso cref="InvalidateVisualState"/>
        protected virtual string DetermineVisualState(ApplicationViewState viewState)
        {
            return viewState.ToString();
        }

        /// <summary>
        /// Updates all controls that are listening for visual state changes with the correct
        /// visual state.
        /// </summary>
        /// <remarks>
        /// Typically used in conjunction with overriding <see cref="DetermineVisualState"/> to
        /// signal that a different value may be returned even though the view state has not
        /// changed.
        /// </remarks>
        public void InvalidateVisualState()
        {
            if (this.layoutAwareControls != null)
            {
                string visualState = DetermineVisualState(ApplicationView.Value);
                foreach (var layoutAwareControl in this.layoutAwareControls)
                {
                    VisualStateManager.GoToState(layoutAwareControl, visualState, false);
                }
            }
        }

        #endregion
    }
}
