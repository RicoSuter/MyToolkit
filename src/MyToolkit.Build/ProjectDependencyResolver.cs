//-----------------------------------------------------------------------
// <copyright file="ProjectDependencyResolver.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace MyToolkit.Build
{
    /// <summary>Provides methods to work with Visual Studio project files. </summary>
    public static class ProjectDependencyResolver
    {
        /// <summary>Gets all referenced project files of a given Visual Studio project file. </summary>
        /// <param name="projectPath">The project file path. </param>
        /// <param name="projectCollection">The project collection.</param>
        /// <returns>The referenced project files. </returns>
        public static IEnumerable<string> GetProjectReferences(string projectPath, ProjectCollection projectCollection)
        {
            return VsProject.Load(projectPath, projectCollection)
                .ProjectReferences.Select(p => p.Path);
        }

        /// <summary>Sorts the given projects in their required build order. </summary>
        /// <param name="projectPaths">The project files. </param>
        /// <param name="projectCollection">The project collection.</param>
        /// <returns>The project file paths in the correct build order. </returns>
        public static IEnumerable<string> GetBuildOrder(IEnumerable<string> projectPaths, ProjectCollection projectCollection)
        {
            return projectPaths.Select(p => VsProject.Load(p, projectCollection))
                .SortByBuildOrder().Select(p => p.Path);
        }
    }
}
