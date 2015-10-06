//-----------------------------------------------------------------------
// <copyright file="Keyboard.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WP8 && !WP7 && !WPF

using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace MyToolkit.Input
{
	public static class Keyboard
	{
		public static bool IsControlKeyDown
		{
			get { return IsKeyDown(VirtualKey.Control); }
		}

		public static bool IsShiftKeyDown
		{
			get { return IsKeyDown(VirtualKey.Shift); }
		}

		public static bool IsAltKeyDown
		{
			get { return IsKeyDown(VirtualKey.LeftMenu); }
		}

		public static bool IsKeyDown(VirtualKey key)
		{
			return (Window.Current.CoreWindow.GetKeyState(key) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
		}
	}
}

#endif