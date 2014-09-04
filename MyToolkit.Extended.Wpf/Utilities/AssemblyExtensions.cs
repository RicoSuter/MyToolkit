//-----------------------------------------------------------------------
// <copyright file="AssemblyExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for <see cref="Assembly"/> objects. </summary>
    public static class AssemblyExtensions
    {
        /// <summary>Gets the build time of the assembly. </summary>
        /// <param name="assembly">The assembly. </param>
        /// <returns>The build time. </returns>
        public static DateTime GetBuildTime(this Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);
        }

        /// <summary>Gets the version and the build time of the assembly (format: '0.0.0.0 (BUILDTIME)'). </summary>
        /// <param name="assembly">The assembly. </param>
        /// <returns>The version and build time. </returns>
        public static string GetVersionWithBuildTime(this Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return version + " (" + GetBuildTime(assembly) + ")";
        }
    }
}
