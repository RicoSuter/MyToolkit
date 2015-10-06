//-----------------------------------------------------------------------
// <copyright file="ExportAttribute.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Composition
{
    /// <summary>Marks a class as exported part. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExportAttribute : Attribute
    {
        /// <summary>Gets or sets the interface type to export the class for. </summary>
        public Type Type;

        /// <summary>Gets or sets the name of the exported part. </summary>
        public string Name;

        /// <summary>Initializes a new instance of the <see cref="ExportAttribute"/> class. </summary>
        /// <param name="type">The interface type to export the class for. </param>
        public ExportAttribute(Type type)
        {
            Type = type;
        }
    }
}