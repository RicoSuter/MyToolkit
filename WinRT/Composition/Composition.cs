using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyToolkit.Composition
{
	public static class Composition
	{
		private static CompositionContext currentContext; 
		public static CompositionContext CurrentContext
		{
			get
			{
				if (currentContext == null)
					 currentContext = new CompositionContext();
				return currentContext; 
			}
		}

		public static void Clear()
		{
			CurrentContext.Clear();
		}

		public static int AddPartsFromAssembly(Assembly assembly)
		{
			return CurrentContext.AddPartsFromAssembly(assembly);
		}

		public static bool AddPart<T>(T part)
		{
			return CurrentContext.AddPart(typeof(T), null, part, part.GetType());
		}

		public static bool AddPart<T>(string name, T part)
		{
			return CurrentContext.AddPart(typeof(T), name, part, part.GetType());
		}

		public static void RemovePart<T>(string name = null)
		{
			CurrentContext.RemovePart(typeof(T), name);
		}

		public static T GetPart<T>(string name = null)
		{
			return CurrentContext.GetPart<T>(name);
		}

		public static IList<T> GetParts<T>()
		{
			return CurrentContext.GetParts<T>();
		}

		public static void SatisfyImports(this object obj)
		{
			CurrentContext.SatisfyImports(obj);
		}

		public static void BuildCache(Type type)
		{
			CurrentContext.BuildCache(type);
		}
	}
}
