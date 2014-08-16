//-----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyToolkit.Utilities
{
    // TODO: Move to MyToolkit.IO?

    /// <summary>
    /// Provides additional reflection methods. 
    /// </summary>
	public static class ReflectionExtensions
	{
        /// <summary>Gets the name of the type (without namespace or assembly version). </summary>
        /// <param name="type">The type. </param>
        /// <returns>The name of the type. </returns>
		public static string GetName(this Type type)
		{
			var fullName = type.FullName;
			var index = fullName.LastIndexOf('.');
			if (index != -1)
				return fullName.Substring(index + 1);
			return fullName;
		}

        /// <summary>Instantiates an object of a generic type. </summary>
        /// <param name="type">The type. </param>
        /// <param name="innerType">The first generic type. </param>
        /// <param name="args">The constructor parameters. </param>
        /// <returns>The instantiated object. </returns>
		public static object CreateGenericObject(this Type type, Type innerType, params object[] args)
		{
			var specificType = type.MakeGenericType(new [] { innerType });
			return Activator.CreateInstance(specificType, args);
		}

        /// <summary>
        /// Merges a given source object into a target object (no deep copy!). 
        /// </summary>
        /// <param name="source">The source object. </param>
        /// <param name="target">The target object. </param>
		public static void Merge<T>(this T source, T target)
		{
			var targetType = target.GetType();
#if !LEGACY
			foreach (var p in source.GetType().GetRuntimeProperties())
			{
				var tp = targetType.GetRuntimeProperty(p.Name); 
				if (tp != null && p.CanWrite)
				{
					var value = p.GetValue(source, null);
					tp.SetValue(target, value, null);
				}
			}
#else
            foreach (var p in source.GetType().GetProperties())
            {
                var tp = targetType.GetProperty(p.Name);
                if (tp != null && p.CanWrite)
                {
                    var value = p.GetValue(source, null);
                    tp.SetValue(target, value, null);
                }
            }
#endif
		}
	}
}
