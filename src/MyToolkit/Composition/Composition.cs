//-----------------------------------------------------------------------
// <copyright file="Composition.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyToolkit.Composition
{
    /// <summary>Provides access to a singleton <see cref="ICompositionContext"/> object. </summary>
    public static class Composition
    {
        private static ICompositionContext _default;

        /// <summary>Gets or sets the default context. </summary>
        public static ICompositionContext Default
        {
            get
            {
                if (_default == null)
                {
                    lock (typeof(Composition))
                    {
                        if (_default == null)
                            _default = new CompositionContext();
                    }
                }
                return _default;
            }
            set { _default = value; }
        }

        [Obsolete("Use method on Default property instead. 6/4/2014")]
        public static int AddPartsFromAssembly(Assembly assembly)
        {
            return Default.AddPartsFromAssembly(assembly);
        }

        [Obsolete("Use method on Default property instead. 6/4/2014")]
        public static void RemovePart<T>(string name = null)
        {
            Default.RemovePart<T>(name);
        }

        [Obsolete("Use method on Default property instead. 6/4/2014")]
        public static T GetPart<T>(string name = null)
        {
            return Default.GetPart<T>(name);
        }

        [Obsolete("Use method on Default property instead. 6/4/2014")]
        public static IList<T> GetParts<T>()
        {
            return Default.GetParts<T>();
        }

        [Obsolete("Use method on Default property instead. 6/4/2014")]
        public static void SatisfyImports<T>(this T obj)
        {
            Default.SatisfyImports(obj);
        }

        [Obsolete("Use method on Default property instead. 6/4/2014")]
        public static void BuildCache<T>()
        {
            Default.BuildCache<T>();
        }

        [Obsolete("Use method on Default property instead. 6/4/2014")]
        public static void Clear()
        {
            Default.Clear();
        }
    }
}
