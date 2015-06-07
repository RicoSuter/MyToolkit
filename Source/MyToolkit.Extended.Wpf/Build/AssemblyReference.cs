//-----------------------------------------------------------------------
// <copyright file="AssemblyReference.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace MyToolkit.Build
{
    /// <summary>Describes a referenced assembly. </summary>
    public class AssemblyReference : VsReferenceBase
    {
        private readonly string _name;
        private readonly string _version;

        /// <summary>Initializes a new instance of the <see cref="AssemblyReference"/> class. </summary>
        /// <param name="projectItem">The raw name. </param>
        internal AssemblyReference(ProjectItem projectItem)
        {
            ProjectItem = projectItem;

            var array = ProjectItem.EvaluatedInclude.Split(',');

            _name = array[0];
            _version = "Any";

            foreach (var tuple in array.Skip(1)
                .Select(n => n.Trim().Split('='))
                .Select(n => new Tuple<string, string>(n[0], n[1])))
            {
                switch (tuple.Item1)
                {
                    case "Version":
                        _version = tuple.Item2;
                        break;
                }
            }
        }

        /// <summary>Gets the full name. </summary>
        public ProjectItem ProjectItem { get; private set; }

        /// <summary>Gets the name of the NuGet package. </summary>
        public override string Name
        {
            get { return _name; }
        }

        /// <summary>Gets the version of the installed NuGet package. </summary>
        public override string Version
        {
            get { return _version; }
        }
    }
}