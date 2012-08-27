using System;
using System.Reflection;

namespace MyToolkit.Utilities
{
	public static class ReflectionExtensions
	{
		public static string GetName(this Type type)
		{
			var fullName = type.FullName;
			var index = fullName.LastIndexOf('.');
			if (index != -1)
				return fullName.Substring(index + 1);
			return fullName;
		}

		public static object CreateGenericObject(this Type type, Type innerType, params object[] args)
		{
			Type specificType = type.MakeGenericType(new [] { innerType });
			return Activator.CreateInstance(specificType, args);
		}

		public static void Clone(this object source, object target)
		{
			var targetType = target.GetType();
			foreach (var p in source.GetType().GetProperties())
			{
				var tp = targetType.GetProperty(p.Name); 
				if (tp != null && p.CanWrite)
				{
					var value = p.GetValue(source, null);
					tp.SetValue(target, value, null);
				}
			}
		}
	}
}
