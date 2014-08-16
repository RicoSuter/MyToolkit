//-----------------------------------------------------------------------
// <copyright file="VisualTreeHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !WPF
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace MyToolkit.Utilities
{
	public class DependencyObjectHelper
	{
		public static T FindVisualChild<T>(DependencyObject obj)
			where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
					return (T)child;
				else
				{
					var childOfChild = FindVisualChild<T>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

	}
}
