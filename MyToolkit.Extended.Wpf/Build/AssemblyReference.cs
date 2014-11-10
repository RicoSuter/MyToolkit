//-----------------------------------------------------------------------
// <copyright file="AssemblyReference.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;

namespace MyToolkit.Build
{
    /// <summary>Describes a referenced assembly. </summary>
    public class AssemblyReference : VsReferenceBase
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyReference"/> class. </summary>
        /// <param name="rawName">The raw name. </param>
        internal AssemblyReference(string rawName)
        {
            RawName = rawName;
            Version = "Any";

            var array = rawName.Split(',');
            Name = array[0];

            foreach (var tuple in array.Skip(1)
                .Select(n => n.Trim().Split('='))
                .Select(n => new Tuple<string, string>(n[0], n[1])))
            {
                switch (tuple.Item1)
                {
                    case "Version":
                        Version = tuple.Item2;
                        break;
                }
            }
        }

        /// <summary>Gets the full name. </summary>
        public string RawName { get; private set; }
    }
}