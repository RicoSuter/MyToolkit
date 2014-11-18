//-----------------------------------------------------------------------
// <copyright file="NuGetPackageNotFoundException.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
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