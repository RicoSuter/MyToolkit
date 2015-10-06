//-----------------------------------------------------------------------
// <copyright file="ImportManyAttribute.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Composition
{
    /// <summary>Marks a collection property to import objects. </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ImportManyAttribute : Attribute
    {
        /// <summary>Gets or sets the interface type to import. </summary>
        public Type Type;

        /// <summary>Initializes a new instance of the <see cref="ImportManyAttribute"/> class. </summary>
        /// <param name="type">The interface type to import. </param>
        public ImportManyAttribute(Type type)
        {
            Type = type;
        }
    }
}