//-----------------------------------------------------------------------
// <copyright file="DependencyObjectExtensions.cs" company="MyToolkit">
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
	/// <summary>Provides extension methods for <see cref="DependencyObject"/> objects. </summary>
	public static class DependencyObjectExtensions
	{
        /// <summary>Traverses the visual child and returns the first child of the given generic type. </summary>
        /// <typeparam name="T">The child type to find. </typeparam>
        /// <param name="obj">The parent object. </param>
        /// <returns>The child object. </returns>
		public static T FindVisualChild<T>(this DependencyObject obj)
			where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				var child = VisualTreeHelper.GetChild(obj, i);
				if (child is T)
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
