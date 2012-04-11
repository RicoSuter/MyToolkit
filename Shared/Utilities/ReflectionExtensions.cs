using System;
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
			System.Type specificType = type.MakeGenericType(new System.Type[] { innerType });
			return Activator.CreateInstance(specificType, args);
		}
	}
}
