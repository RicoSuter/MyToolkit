//-----------------------------------------------------------------------
// <copyright file="BuildOrderException.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Build
{
    public class BuildOrderException : Exception
    {
        public BuildOrderException(string message) : base(message) { }
    }
}