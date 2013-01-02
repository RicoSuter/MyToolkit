using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;

#if WINRT
using MyToolkit.Utilities;
using Windows.UI.Xaml.Data; 
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#else
using System.ComponentModel;
#endif

namespace MyToolkit.MVVM
{
	[DataContract]
	public class NotifyPropertyChanged<T> : NotifyPropertyChanged
	{
		public bool SetProperty(Expression<Func<T, object>> expression, ref T oldValue, T newValue)
		{
			return SetProperty(GetName(expression), ref oldValue, newValue);
		}

		public void RaisePropertyChanged(Expression<Func<T, object>> expression)
		{
			RaisePropertyChanged(GetName(expression)); 
		}

		public void SetDependency(Expression<Func<T, object>> propertyName, Expression<Func<T, object>> dependentPropertyName)
		{
			SetDependency(GetName(propertyName), GetName(dependentPropertyName));
		}
	}

	[DataContract]
    public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private List<KeyValuePair<string, string>> dependencies;

		public bool SetProperty<T>(Expression<Func<T, object>> expression, ref T oldValue, T newValue)
		{
			return SetProperty(GetName(expression), ref oldValue, newValue);
		}

		public bool SetProperty<T>(String propertyName, ref T oldValue, T newValue)
		{
			if (Equals(oldValue, newValue))
				return false;

			oldValue = newValue;
			RaisePropertyChanged(propertyName);
			return true; 			
		}

		public void SetDependency(string propertyName, string dependentPropertyName)
		{
			if (dependencies == null)
				dependencies = new List<KeyValuePair<string, string>>();

			if (dependencies.Any(d => d.Key == propertyName && d.Value == dependentPropertyName))
				return;

			dependencies.Add(new KeyValuePair<string, string>(propertyName, dependentPropertyName));
		}

		public void SetDependency<T>(Expression<Func<T, object>> propertyName, Expression<Func<T, object>> dependentPropertyName)
		{
			SetDependency(GetName(propertyName), GetName(dependentPropertyName));
		}

		public void RaisePropertyChanged<T>(Expression<Func<T, object>> expression)
		{
			RaisePropertyChanged(GetName(expression));
		}

		protected static string GetName<T>(Expression<Func<T, object>> expression)
		{
			if (expression.Body is UnaryExpression)
				return (((MemberExpression) ((UnaryExpression) expression.Body).Operand).Member).Name;
			return ((MemberExpression) expression.Body).Member.Name;
		}

#if WINRT
		public bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] String propertyName = null)
		{
			return SetProperty(propertyName, ref oldValue, newValue); 
		} 

		public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
#else
		public void RaisePropertyChanged(string propertyName = null)
#endif
		{
			#if DEBUG
				#if WINRT
					if (GetType().GetTypeInfo().GetProperty(propertyName) == null)
						throw new ArgumentException("propertyName");
				#else
					if (GetType().GetProperty(propertyName) == null)
						throw new ArgumentException("propertyName");
				#endif
            #endif

			var copy = PropertyChanged; // avoid concurrent changes
			if (copy != null)
				copy(this, new PropertyChangedEventArgs(propertyName));

			if (dependencies != null)
			{
				foreach (var d in dependencies.Where(d => d.Key == propertyName))
					RaisePropertyChanged(d.Value);
			}
		}
	}
}
