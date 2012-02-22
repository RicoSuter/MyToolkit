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
	}
}
