using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace MyToolkit.UI
{
	public static class VisibilityManager
	{
		public static readonly DependencyProperty MinPageWidthProperty =
			DependencyProperty.RegisterAttached("MinPageWidth", typeof(double), 
				typeof(VisibilityManager), new PropertyMetadata(default(double), 
					OnMinPageWidthChanged));

		public static void SetMinPageWidth(UIElement element, double value)
		{
			element.SetValue(MinPageWidthProperty, value);
		}

		public static double GetMinPageWidth(UIElement element)
		{
			return (double) element.GetValue(MinPageWidthProperty);
		}

		private static List<FrameworkElement> elements;  
		private static void OnMinPageWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var element = obj as FrameworkElement;
			if (element != null)
			{
				Initialize();
				
				element.Loaded += delegate(object s, RoutedEventArgs e)
				{
					var sender = (FrameworkElement)s;
					var value = GetMinPageWidth(sender);
					if (value > 0)
					{
						Initialize();
						if (!elements.Contains(sender))
						{
							elements.Add(sender);
							UpdateElement(sender, value);
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

				UpdateElement(element, (double)args.NewValue);

				if (!elements.Contains(element))
					elements.Add(element);
			}
		}

		private static void UpdateElement(FrameworkElement ctrl, double value)
		{
			ctrl.Visibility = Window.Current.Bounds.Width < value ? 
				Visibility.Collapsed : Visibility.Visible;
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
			{
				e.Visibility = args.Size.Width < GetMinPageWidth(e) ? 
					Visibility.Collapsed : Visibility.Visible;
			}
		}

		private static void CleanUp()
		{
			elements = null;
			Window.Current.SizeChanged -= OnSizeChanged;
		}
	}
}
