//-----------------------------------------------------------------------
// <copyright file="VisibilityManager.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace MyToolkit.UI
{
	public static class VisibilityManager
	{
#region Dependency properties

		public static readonly DependencyProperty MinPageWidthProperty =
			DependencyProperty.RegisterAttached("MinPageWidth", typeof(double),
				typeof(VisibilityManager), new PropertyMetadata(default(double),
					OnValueChanged));

		public static void SetMinPageWidth(UIElement element, double value)
		{
			element.SetValue(MinPageWidthProperty, value);
		}

		public static double GetMinPageWidth(UIElement element)
		{
			return (double)element.GetValue(MinPageWidthProperty);
		}

		public static readonly DependencyProperty MaxPageWidthProperty =
			DependencyProperty.RegisterAttached("MaxPageWidth", typeof(double),
				typeof(VisibilityManager), new PropertyMetadata(default(double),
					OnValueChanged));

		public static void SetMaxPageWidth(UIElement element, double value)
		{
			element.SetValue(MaxPageWidthProperty, value);
		}

		public static double GetMaxPageWidth(UIElement element)
		{
			return (double)element.GetValue(MaxPageWidthProperty);
		}

#endregion

		private static List<FrameworkElement> _elements;
		public static List<FrameworkElement> Elements
		{
			get
			{
				if (_elements == null)
				{
					Window.Current.SizeChanged += OnSizeChanged;
					_elements = new List<FrameworkElement>();
				}
				return _elements;
			}
		}

		private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var element = obj as FrameworkElement;
			if (element != null)
			{
				element.Loaded += delegate(object s, RoutedEventArgs e)
				{
					var sender = (FrameworkElement)s;
					if (GetMinPageWidth(sender) > 0 || GetMaxPageWidth(sender) > 0)
					{
						if (!Elements.Contains(sender))
						{
							Elements.Add(sender);
							UpdateElement(sender, Window.Current.Bounds.Width);
						}
					}
				};

				element.Unloaded += delegate(object s, RoutedEventArgs e)
				{
					var sender = (FrameworkElement)s;
					if (Elements.Contains(sender))
						Elements.Remove(sender);

					if (Elements.Count == 0)
					{
						_elements = null;
						Window.Current.SizeChanged -= OnSizeChanged;
					}
				};

				UpdateElement(element, Window.Current.Bounds.Width);
			}
		}

		private static void UpdateElement(FrameworkElement element, double windowWidth)
		{
			var minWidth = GetMinPageWidth(element);
			var maxWidth = GetMaxPageWidth(element);

			if (minWidth > 0)
			{
				if (minWidth > 0 && maxWidth > 0)
					element.Visibility = windowWidth > minWidth && Window.Current.Bounds.Width <= maxWidth ?
						Visibility.Visible : Visibility.Collapsed;
				else
					element.Visibility = windowWidth > minWidth ? Visibility.Visible : Visibility.Collapsed;
			}
			else if (maxWidth > 0)
				element.Visibility = windowWidth <= maxWidth ? Visibility.Visible : Visibility.Collapsed;
		}

		private static void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
		{
			foreach (var e in Elements)
				UpdateElement(e, args.Size.Width);
		}
	}
}

#endif