using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyToolkit.Utilities
{
	public static class ReflectionExtensions
	{
#if WINRT
		public static IEnumerable<PropertyInfo> GetProperties(this Type type)
		{
			return type.GetTypeInfo().DeclaredProperties;
		}

		public static PropertyInfo GetProperty(this Type type, string name)
		{
			return type.GetTypeInfo().GetDeclaredProperty(name);
		}

		public static MethodInfo GetMethod(this Type type, string name)
		{
			return type.GetTypeInfo().GetDeclaredMethod(name);
		}

		public static Type GetBaseType(this Type type)
		{
			return type.GetTypeInfo().BaseType;
		}

		public static IList<Type> GetGenericArguments(this Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}

		public static IEnumerable<Type> GetInterfaces(this Type type)
		{
			return type.GetTypeInfo().ImplementedInterfaces;
		}
#else 
#if !WP8
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}
#endif
#endif
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

#if WINRT
		public static void Clone(this object source, object target)
		{
			var targetType = target.GetType().GetTypeInfo();
			foreach (var p in source.GetType().GetTypeInfo().DeclaredProperties)
			{
				var tp = targetType.GetDeclaredProperty(p.Name);
				if (tp != null && p.CanWrite)
				{
					var value = p.GetValue(source, null);
					tp.SetValue(target, value, null);
				}
			}
		}

		public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo type)
		{
			var list = type.DeclaredProperties.ToList();

			var subtype = type.BaseType;
			if (subtype != null)
				list.AddRange(subtype.GetTypeInfo().GetAllProperties());

			foreach (var i in type.ImplementedInterfaces)
				list.AddRange(i.GetTypeInfo().GetAllProperties());

			return list.ToArray();
		}

		public static PropertyInfo GetProperty(this TypeInfo type, string name)
		{
			var property = type.GetDeclaredProperty(name);
			if (property != null)
				return property;

			foreach (var i in type.ImplementedInterfaces)
			{
				property = i.GetTypeInfo().GetProperty(name);
				if (property != null)
					return property;
			}

			var subtype = type.BaseType;
			if (subtype != null)
			{
				property = subtype.GetTypeInfo().GetProperty(name);
				if (property != null)
					return property;
			}
			return null; 
		}
#else
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
#endif

	}
}
