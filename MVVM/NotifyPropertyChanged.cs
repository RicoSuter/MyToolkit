using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;

#if METRO
using Windows.UI.Xaml.Data; 
using System.Reflection;
#else
using System.ComponentModel;
#endif

namespace MyToolkit.MVVM
{
	[DataContract]
	public class NotifyPropertyChanged<TU> : NotifyPropertyChanged
	{
		public bool SetProperty<T>(Expression<Func<TU, T>> expression, ref T oldValue, T newValue)
		{
			return SetProperty(((MemberExpression)expression.Body).Member.Name, ref oldValue, newValue);
		}

		public void RaisePropertyChanged<T>(Expression<Func<TU, T>> expression)
		{
			RaisePropertyChanged(((MemberExpression)expression.Body).Member.Name); 
		}
	}

	[DataContract]
	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public bool SetProperty<T>(String propertyName, ref T oldValue, T newValue)
		{
			if (newValue != null && newValue.Equals(oldValue)) 
				return false;

			oldValue = newValue;
			RaisePropertyChanged(propertyName);
			return true; 
		}

		public void RaisePropertyChanged(string propertyName)
		{
			#if DEBUG
				#if METRO
					// TODO: search in subclasses
					//if (GetType().GetTypeInfo().GetDeclaredProperty(propertyName) == null)
					//	throw new ArgumentException("propertyName");
				#else
					if (GetType().GetProperty(propertyName) == null)
						throw new ArgumentException("propertyName");
				#endif
            #endif

			var copy = PropertyChanged; // avoid concurrent changes
			if (copy != null)
				copy(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
