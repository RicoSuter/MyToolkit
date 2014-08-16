using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;

#if WINRT
using System.Reflection;
using Windows.UI.Xaml.Data;
using System.ComponentModel;
#else
using System.ComponentModel;
#endif


namespace MyToolkit.MVVM
{
	public static class PropertyChangedEventArgsExtensions
	{
		public static bool IsProperty<TU>(this PropertyChangedEventArgs args, string propertyName)
		{
#if DEBUG
#if WINRT
			if (typeof(TU).GetTypeInfo().DeclaredProperties.Any(p => p.Name == propertyName))
				throw new ArgumentException("propertyName");
#else
			if (typeof(TU).GetProperty(propertyName) == null)
				throw new ArgumentException("propertyName");
#endif
#endif

			return args.PropertyName == propertyName;
		}

		public static bool IsProperty<TU>(this PropertyChangedEventArgs args, Expression<Func<TU, object>> expression)
		{
			var memexp = expression.Body is UnaryExpression ? 
				(MemberExpression)((UnaryExpression)expression.Body).Operand : ((MemberExpression)expression.Body);
			return args.PropertyName == memexp.Member.Name;
		}
	}
}