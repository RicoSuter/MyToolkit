//-----------------------------------------------------------------------
// <copyright file="ImportAttribute.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Composition
{
    /// <summary>Marks a property to import another object. </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ImportAttribute : Attribute
    {
        /// <summary>Gets or sets the interface type to import. </summary>
        public Type Type;

        /// <summary>Gets or sets the name of the imported part. </summary>
        public string Name;

        /// <summary>Initializes a new instance of the <see cref="ImportAttribute"/> class. </summary>
        /// <param name="type">The interface type to import. </param>
        public ImportAttribute(Type type)
        {
            Type = type;
        }
    }
}