//-----------------------------------------------------------------------
// <copyright file="BuildOrderException.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Build.Exceptions
{
    public class BuildOrderException : Exception
    {
        public BuildOrderException(string message) : base(message) { }
    }
}