using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace MyToolkit.MVVM
{
	public static class PropertyChangedEventArgsExtensions
	{
		public static bool IsProperty<TU>(this PropertyChangedEventArgs args, string propertyName)
		{
			#if DEBUG
			if (typeof(TU).GetProperty(propertyName) == null)
				throw new ArgumentException("propertyName");
			#endif
			
			return args.PropertyName == propertyName;
		}

		private static IDictionary<object, string> expressionDictionary; 
		public static bool IsProperty<TU>(this PropertyChangedEventArgs args, Expression<Func<TU, object>> expression)
		{
			if (expressionDictionary == null)
				expressionDictionary = new Dictionary<object, string>(); 
			
			if (!expressionDictionary.ContainsKey(expression))
			{
				var memexp = expression.Body is UnaryExpression ?
					(MemberExpression)((UnaryExpression)expression.Body).Operand :
					((MemberExpression)expression.Body);
				expressionDictionary[expression] = memexp.Member.Name;
			}
			
			return args.PropertyName == expressionDictionary[expression];
		}
	}

	[DataContract]
	public class NotifyPropertyChanged<TU> : NotifyPropertyChanged
	{
		private readonly IDictionary<object, string> expressionDictionary =  
            new Dictionary<object, string>(); 
	
		public bool SetProperty<T>(Expression<Func<TU, T>> expression, ref T oldValue, T newValue)
		{
			if (!expressionDictionary.ContainsKey(expression))
				expressionDictionary[expression] = ((MemberExpression) expression.Body).Member.Name;
			return SetProperty(expressionDictionary[expression], ref oldValue, newValue);
		}

		public void RaisePropertyChanged<T>(Expression<Func<TU, T>> expression)
		{
			if (!expressionDictionary.ContainsKey(expression))
				expressionDictionary[expression] = ((MemberExpression) expression.Body).Member.Name;
			RaisePropertyChanged(expressionDictionary[expression]);
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
			if (GetType().GetProperty(propertyName) == null)
				throw new ArgumentException("propertyName");
			#endif

			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
