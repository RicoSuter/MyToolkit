//-----------------------------------------------------------------------
// <copyright file="ProjectDependencyResolver.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace MyToolkit.Build
{
    /// <summary>Provides methods to work with Visual Studio project files. </summary>
    public static class ProjectDependencyResolver
    {
        /// <summary>Gets all referenced project files of a given Visual Studio project file. </summary>
        /// <param name="projectPath">The project file path. </param>
        /// <returns>The referenced project files. </returns>
        public static IEnumerable<string> GetProjectReferences(string projectPath)
        {
            return VsProject.Load(projectPath).ProjectReferences.Select(p => p.Path);
        }

        /// <summary>Sorts the given projects in their required build order. </summary>
        /// <param name="projectPaths">The project files. </param>
        /// <returns>The project file paths in the correct build order. </returns>
        public static IEnumerable<string> GetBuildOrder(IEnumerable<string> projectPaths)
        {
            return projectPaths.Select(VsProject.Load).SortByBuildOrder().Select(p => p.Path);
        }
    }
}
