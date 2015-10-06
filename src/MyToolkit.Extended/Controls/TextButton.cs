//-----------------------------------------------------------------------
// <copyright file="TextButton.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public class TextButton : Button
	{
		public TextButton()
		{
			DefaultStyleKey = typeof(TextButton);
		}

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (String), typeof (TextButton), new PropertyMetadata(default(String)));

		public String Header
		{
			get { return (String) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
	}
}

#endif