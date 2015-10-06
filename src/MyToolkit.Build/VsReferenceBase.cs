//-----------------------------------------------------------------------
// <copyright file="AssemblyReference.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Diagnostics;

namespace MyToolkit.Build
{
    /// <summary>Describes a reference. </summary>
    [DebuggerDisplay("{Name} - {Version}")]
    public abstract class VsReferenceBase
    {
        /// <summary>Gets the name of the NuGet package. </summary>
        public abstract string Name { get; }

        /// <summary>Gets the version of the installed NuGet package. </summary>
        public abstract string Version { get; }
    }
}