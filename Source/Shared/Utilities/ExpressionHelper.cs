using System;
using System.Linq.Expressions;

namespace MyToolkit.Utilities
{
	public static class ExpressionHelper
	{
		public static string GetName<TClass, TProp>(Expression<Func<TClass, TProp>> expression)
		{
			if (expression.Body is UnaryExpression)
				return ((MemberExpression)(((UnaryExpression)expression.Body).Operand)).Member.Name;
			return ((MemberExpression)expression.Body).Member.Name;
		}
	}
}