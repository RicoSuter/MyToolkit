//-----------------------------------------------------------------------
// <copyright file="ProjectDependencyResolver.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
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
            return VisualStudioProject.FromFilePath(projectPath).ProjectReferences.Select(p => p.Path);
        }

        /// <summary>Sorts the given projects in their required build order. </summary>
        /// <param name="projectPaths">The project files. </param>
        /// <returns>The project file paths in the correct build order. </returns>
        public static IEnumerable<string> GetBuildOrder(IEnumerable<string> projectPaths)
        {
            return projectPaths.Select(VisualStudioProject.FromFilePath).SortByBuildOrder().Select(p => p.Path);
        }

        /// <summary>Checks whether the two project file paths are the same files. </summary>
        /// <param name="projectPath1">The first project file path. </param>
        /// <param name="projectPath2">The second project file path. </param>
        /// <returns>True when the paths are the same files. </returns>
        public static bool IsSameProject(string projectPath1, string projectPath2)
        {
            return Path.GetFullPath(projectPath1).ToLower() == Path.GetFullPath(projectPath2).ToLower();
        }
    }
}
