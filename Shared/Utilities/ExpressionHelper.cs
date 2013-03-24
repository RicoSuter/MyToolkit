using System;
using System.Linq.Expressions;

namespace MyToolkit.Utilities
{
	public static class ExpressionHelper
	{
		public static string GetName<TClass, TProp>(Expression<Func<TClass, TProp>> expression)
		{
			return ((MemberExpression)expression.Body).Member.Name;
		}
	}
}