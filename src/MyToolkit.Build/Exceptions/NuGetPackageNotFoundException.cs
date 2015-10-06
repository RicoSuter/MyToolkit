//-----------------------------------------------------------------------
// <copyright file="NuGetPackageNotFoundException.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Build.Exceptions
{
    public class NuGetPackageNotFoundException : Exception
    {
        public NuGetPackageNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}