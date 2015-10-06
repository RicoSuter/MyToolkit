//-----------------------------------------------------------------------
// <copyright file="VsProject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using Microsoft.Build.Evaluation;

namespace MyToolkit.Build
{
    /// <summary>An project reference. </summary>
    public class VsProjectReference : VsObject
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref="VsProjectReference"/> class. </summary>
        /// <param name="path">The path to the referenced project. </param>
        /// <param name="name">The reference name. </param>
        private VsProjectReference(string path, string name) : base(path)
        {
            _name = name; 
        }

        /// <summary>Loads a <see cref="VsProjectReference"/> from a <see cref="ProjectItem"/>. </summary>
        /// <param name="project">The parent project. </param>
        /// <param name="projectItem">The <see cref="ProjectItem"/>. </param>
        /// <returns>The <see cref="VsProjectReference"/>. </returns>
        public static VsProjectReference Load(VsProject project, ProjectItem projectItem)
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(project.Path), projectItem.EvaluatedInclude);
            path = System.IO.Path.GetFullPath(path);

            var name = projectItem.Metadata.Single(m => m.Name == "Name").EvaluatedValue;

            return new VsProjectReference(path, name);
        }

        /// <summary>Checks whether both projects are loaded from the same file. </summary>
        /// <param name="project">The other project. </param>
        /// <returns>true when both projects are loaded from the same file. </returns>
        public bool IsSameProject(VsProject project)
        {
            return Id == project.Id;
        }

        /// <summary>Checks whether both projects are loaded from the same file. </summary>
        /// <param name="projectReference">The other project reference. </param>
        /// <returns>true when both projects are loaded from the same file. </returns>
        public bool IsSameProject(VsProjectReference projectReference)
        {
            return Id == projectReference.Id;
        }

        /// <summary>Gets the name of the project. </summary>
        public override string Name
        {
            get { return _name; }
        }
    }
}