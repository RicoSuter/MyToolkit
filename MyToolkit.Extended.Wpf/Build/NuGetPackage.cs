//-----------------------------------------------------------------------
// <copyright file="NuGetPackage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Build
{
    /// <summary>Describes an installed NuGet package. </summary>
    public class NuGetPackage
    {
        /// <summary>Gets the name of the NuGet package. </summary>
        public string Name { get; internal set; }

        /// <summary>Gets the version of the installed NuGet package. </summary>
        public string Version { get; internal set; }
    }
}