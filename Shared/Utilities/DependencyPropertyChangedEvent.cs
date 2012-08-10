using System;

#if METRO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#else
using System.Windows;
using System.Windows.Data;
#endif 

namespace MyToolkit.Utilities
{
	public static class DependencyPropertyChangedEvent
	{
		public class Helper
		{
			private readonly Action<object, object> changed;
			private readonly FrameworkElement obj;

			public Helper(FrameworkElement obj, Action<object, object> changed, object currentValue)
			{
				this.obj = obj; 
				this.changed = changed;
				this.currentValue = currentValue;
			}

			private object currentValue;
			public object Property
			{
				get { return currentValue; }
				set
				{
					changed(obj, value);
					currentValue = value; 
				}
			}
		}

		public static void Register(FrameworkElement obj, DependencyProperty property, Action<object, object> changed)
		{
			var helper = new Helper(obj, changed, obj.GetValue(property));
			var binding = new Binding();
			binding.Path = new PropertyPath("Property");
			binding.Source = helper;
			binding.Mode = BindingMode.TwoWay;
			obj.SetBinding(property, binding);
		}
	}
}