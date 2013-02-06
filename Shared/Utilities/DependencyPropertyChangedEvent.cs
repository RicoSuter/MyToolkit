using System;

#if WINRT
using System.Collections.Generic;
using System.Linq;
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
			public readonly FrameworkElement obj;
			public readonly DependencyProperty property;
			public Action<object, object> changed;

			public Helper(FrameworkElement obj, DependencyProperty property, Action<object, object> changed, object currentValue)
			{
				this.obj = obj;
				this.property = property; 
				this.changed = changed;
				this.currentValue = currentValue;
			}

			private object currentValue;
			public object Property
			{
				get { return currentValue; }
				set
				{
					if (changed != null)
						changed(obj, value);
					currentValue = value; 
				}
			}
		}

		private static List<Helper> helpers;
		public static void Register(FrameworkElement obj, DependencyProperty property, Action<object, object> changed)
		{
			if (helpers == null)
				helpers = new List<Helper>();

			var helper = new Helper(obj, property, changed, obj.GetValue(property));
			var binding = new Binding();
			binding.Path = new PropertyPath("Property");
			binding.Source = helper;
			binding.Mode = BindingMode.TwoWay;
			obj.SetBinding(property, binding);
			
			helpers.Add(helper);
		}

		public static void Unregister(FrameworkElement obj, DependencyProperty property, Action<object, object> changed)
		{
			var helper = helpers.Single(h => h.obj == obj && h.property == property && h.changed == changed);
			helper.changed = null; 
		}
	}
}