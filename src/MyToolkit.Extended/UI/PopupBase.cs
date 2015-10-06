//-----------------------------------------------------------------------
// <copyright file="PopupBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MyToolkit.UI
{
	public class PopupBase : UserControl
	{
		public void Close()
		{
			((Popup)Parent).IsOpen = false; 
		}
	}
}

#endif