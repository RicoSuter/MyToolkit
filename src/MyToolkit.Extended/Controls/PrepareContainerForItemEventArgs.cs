//-----------------------------------------------------------------------
// <copyright file="PrepareContainerForItemEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Controls
{
	public class PrepareContainerForItemEventArgs : EventArgs
	{
		public PrepareContainerForItemEventArgs(DependencyObject element, object item)
		{
			Element = element;
			Item = item;
		}

		public DependencyObject Element { get; private set; }

		public object Item { get; private set; }
	}
}