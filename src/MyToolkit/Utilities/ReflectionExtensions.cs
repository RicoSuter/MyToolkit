//-----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyToolkit.Utilities
{
    /// <summary>Provides additional reflection methods. </summary>
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

        /// <summary>Checks whether the given type inherits from a type with the given class name.</summary>
        /// <param name="type">The type.</param>
        /// <param name="typeName">THe class name of the type (not the full class name).</param>
        /// <returns>True when inheriting from the type name.</returns>
        public static bool InheritsFromTypeName(this Type type, string typeName)
        {
            var baseType = ReflectionUtilities.GetBaseType(type);
            while (baseType != null)
            {
                if (baseType.Name == typeName)
                    return true;

                baseType = ReflectionUtilities.GetBaseType(baseType);
            }
            return false;
        }


        /// <summary>Instantiates an object of a generic type. </summary>
        /// <param name="type">The type. </param>
        /// <param name="innerType">The first generic type. </param>
        /// <param name="args">The constructor parameters. </param>
        /// <returns>The instantiated object. </returns>
        public static object CreateGenericObject(this Type type, Type innerType, params object[] args)
        {
            var specificType = type.MakeGenericType(new[] { innerType });
            return Activator.CreateInstance(specificType, args);
        }

        /// <summary>Merges a given source object into a target object (no deep copy!). </summary>
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

#if !LEGACY

        public static IEnumerable<PropertyInfo> GetInheritedProperties(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var list = typeInfo.DeclaredProperties.ToList();

            var subtype = typeInfo.BaseType;
            if (subtype != null)
                list.AddRange(subtype.GetRuntimeProperties());

            foreach (var i in typeInfo.ImplementedInterfaces)
                list.AddRange(i.GetRuntimeProperties());

            return list.ToArray();
        }

        public static PropertyInfo GetInheritedProperty(this Type type, string name)
        {
            var typeInfo = type.GetTypeInfo();

            var property = typeInfo.GetDeclaredProperty(name);
            if (property != null)
                return property;

            foreach (var i in typeInfo.ImplementedInterfaces)
            {
                property = i.GetRuntimeProperty(name);
                if (property != null)
                    return property;
            }

            var subtype = typeInfo.BaseType;
            if (subtype != null)
            {
                property = subtype.GetRuntimeProperty(name);
                if (property != null)
                    return property;
            }

            return null;
        }

        public static void Clone(this object source, object target)
        {
            var targetType = target.GetType();
            foreach (var p in source.GetType().GetInheritedProperties())
            {
                var tp = targetType.GetInheritedProperty(p.Name);
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
