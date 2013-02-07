using System;
using System.Collections.Generic;
using MyToolkit.Input;
using MyToolkit.UI;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Paging
{
	public class ExtendedPage : Page
	{
		public bool IsSuspendable { get; set; }
		public bool UseBackKeyToGoBack { get; set; }
		public bool UseBackKeyToGoBackInWebView { get; set; }

		public ExtendedPage()
		{
			UseBackKeyToGoBack = true;
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
				return;

			Loaded += (sender, args) =>
			{
				if (ActualHeight == Window.Current.Bounds.Height &&
					ActualWidth == Window.Current.Bounds.Width)
				{
					Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
					Window.Current.CoreWindow.PointerPressed += OnPointerPressed;
				}
			};

			Unloaded += (sender, args) =>
			{
				Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= OnAcceleratorKeyActivated;
				Window.Current.CoreWindow.PointerPressed -= OnPointerPressed;
			};
		}

		#region Navigation

		private String pageKey;
		protected internal override void InternalOnNavigatedTo(NavigationEventArgs e)
		{
			base.InternalOnNavigatedTo(e);
			if (pageKey != null) // new instance
				return;

			var frameState = SuspensionManager.SessionStateForFrame(Frame);
			pageKey = "Page" + Frame.BackStackDepth;

			if (e.NavigationMode == NavigationMode.New)
			{
				var nextPageKey = pageKey;
				var nextPageIndex = Frame.BackStackDepth;
				while (frameState.Remove(nextPageKey))
				{
					nextPageIndex++;
					nextPageKey = "Page" + nextPageIndex;
				}
				LoadState(e.Parameter, null);
			}
			else
				LoadState(e.Parameter, (Dictionary<String, Object>)frameState[pageKey]);
		}

		protected internal override void InternalOnNavigatedFrom(NavigationEventArgs e)
		{
			base.InternalOnNavigatedFrom(e);

			var frameState = SuspensionManager.SessionStateForFrame(Frame);
			var pageState = new Dictionary<String, Object>();
			SaveState(pageState);
			frameState[pageKey] = pageState;
		}

		#endregion

		#region Suspending

		protected virtual void LoadState(Object parameter, Dictionary<String, Object> pageState)
		{

		}

		protected virtual void SaveState(Dictionary<String, Object> pageState)
		{

		}

		#endregion

		#region Keyboard/Pointer input

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
					(int)virtualKey == 166 || (int)virtualKey == 167);
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
	}
}