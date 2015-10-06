//-----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;

namespace MyToolkit.Build
{
    internal static class ReflectionExtensions
    {
        public static object GetPropertyValue(this object obj, string name)
        {
            return obj.GetType()
                .GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(obj, null);
        }

        public static void SetPropertyValue(this object obj, string name, object value)
        {
            obj.GetType()
                .GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(obj, value, null);
        }

        public static object InvokeMethod(this object obj, string name, params object[] args)
        {
            return obj.GetType()
                .GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(obj, args);
        }
    }

}
