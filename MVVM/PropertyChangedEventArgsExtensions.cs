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
}