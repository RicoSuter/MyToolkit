using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Utilities
{
	public static class PropertyPathHelper
	{
		private static readonly DependencyProperty DummyProperty =
			DependencyProperty.RegisterAttached(
			"Dummy", typeof(Object),
			typeof(DependencyObject),
			new PropertyMetadata(null));

		public static Object Evaluate(Object container, PropertyPath propertyPath)
		{
			Binding binding = new Binding() { Source = container, Path = propertyPath };
			DependencyObject dummyDO = new MyDependencyObject();
			BindingOperations.SetBinding(dummyDO, DummyProperty, binding);
			return dummyDO.GetValue(DummyProperty);
		}

		public static Object Evaluate(Object container, String propertyPath)
		{
			return Evaluate(container, new PropertyPath(propertyPath));
		}
	}
}
