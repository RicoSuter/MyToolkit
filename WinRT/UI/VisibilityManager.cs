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
			return (double) element.GetValue(MinPageWidthProperty);
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
			return (double) element.GetValue(MaxPageWidthProperty);
		}

		#endregion 

		private static List<FrameworkElement> elements;  
		private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var element = obj as FrameworkElement;
			if (element != null)
			{
				Initialize();
				
				element.Loaded += delegate(object s, RoutedEventArgs e)
				{
					var sender = (FrameworkElement)s;
					if (GetMinPageWidth(sender) > 0 || GetMaxPageWidth(sender) > 0)
					{
						Initialize();
						if (!elements.Contains(sender))
						{
							elements.Add(sender);
							UpdateElement(sender, Window.Current.Bounds.Width);
						}
					}
				};

				element.Unloaded += delegate(object s, RoutedEventArgs e)
				{
					var sender = (FrameworkElement)s;
					if (elements.Contains(sender))
						elements.Remove(sender);
					if (elements.Count == 0)
						CleanUp();
				};

				UpdateElement(element, Window.Current.Bounds.Width);

				if (!elements.Contains(element))
					elements.Add(element);
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

		private static void Initialize()
		{
			if (elements == null)
			{
				elements = new List<FrameworkElement>();
				Window.Current.SizeChanged += OnSizeChanged;
			}
		}

		private static void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
		{
			foreach (var e in elements)
				UpdateElement(e, args.Size.Width);
		}

		private static void CleanUp()
		{
			elements = null;
			Window.Current.SizeChanged -= OnSizeChanged;
		}
	}
}
