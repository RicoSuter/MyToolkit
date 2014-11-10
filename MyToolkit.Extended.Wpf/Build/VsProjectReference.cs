//-----------------------------------------------------------------------
// <copyright file="VsProject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Build
{
    public class VsProjectReference : VsObject
    {
        public VsProjectReference(string path) : base(path)
        {
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
    }
}