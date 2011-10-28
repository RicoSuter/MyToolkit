using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

#if METRO
using System.Reflection;
#endif

namespace MyToolkit.MVVM
{
	public static class PropertyChangedEventArgsExtensions
	{
		public static bool IsProperty<TU>(this PropertyChangedEventArgs args, string propertyName)
		{
			#if DEBUG
				#if METRO
					if (typeof(TU).GetTypeInfo().DeclaredProperties.Any(p => p.Name == propertyName))
						throw new ArgumentException("propertyName");
				#else
					if (typeof(TU).GetProperty(propertyName) == null)
						throw new ArgumentException("propertyName");
				#endif
            #endif

            return args.PropertyName == propertyName;
		}

		//private static IDictionary<object, string> expressionDictionary; 
		public static bool IsProperty<TU>(this PropertyChangedEventArgs args, Expression<Func<TU, object>> expression)
		{
			//if (expressionDictionary == null)
			//	expressionDictionary = new Dictionary<object, string>(); 
			
			//if (!expressionDictionary.ContainsKey(expression))
			//{
				var memexp = expression.Body is UnaryExpression ?
					(MemberExpression)((UnaryExpression)expression.Body).Operand :
					((MemberExpression)expression.Body);
				return args.PropertyName == memexp.Member.Name;
				//expressionDictionary[expression] = memexp.Member.Name;
			//}
			
			//return args.PropertyName == expressionDictionary[expression];
		}
	}

	[DataContract]
	public class NotifyPropertyChanged<TU> : NotifyPropertyChanged
	{
		//private IDictionary<object, string> expressionDictionary; 
	
		public bool SetProperty<T>(Expression<Func<TU, T>> expression, ref T oldValue, T newValue)
		{
			//if (expressionDictionary == null)
			//    expressionDictionary = new Dictionary<object, string>();
			//if (!expressionDictionary.ContainsKey(expression))
			//    expressionDictionary[expression] = ((MemberExpression) expression.Body).Member.Name;
			//return SetProperty(expressionDictionary[expression], ref oldValue, newValue);
			return SetProperty(((MemberExpression)expression.Body).Member.Name, ref oldValue, newValue);
		}

		public void RaisePropertyChanged<T>(Expression<Func<TU, T>> expression)
		{
			//if (expressionDictionary == null)
			//    expressionDictionary = new Dictionary<object, string>();
			//if (!expressionDictionary.ContainsKey(expression))
			//    expressionDictionary[expression] = ((MemberExpression) expression.Body).Member.Name;
			//RaisePropertyChanged(expressionDictionary[expression]);
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
					if (GetType().GetTypeInfo().DeclaredProperties.Any(p => p.Name == propertyName))
						throw new ArgumentException("propertyName");
				#else
					if (GetType().GetProperty(propertyName) == null)
						throw new ArgumentException("propertyName");
				#endif
            #endif

			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
