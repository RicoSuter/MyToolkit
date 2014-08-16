//-----------------------------------------------------------------------
// <copyright file="FileOpenEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Utilities
{
    /// <summary>The file open event arguments. </summary>
    public class FileOpenEventArgs : EventArgs
    {
        /// <summary>Gets the file name of the file to open. </summary>
        public string FileName { get; set; }
    }
}