//-----------------------------------------------------------------------
// <copyright file="VsProjectEnumerableExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MyToolkit.Build.Exceptions;

namespace MyToolkit.Build
{
    /// <summary>Provides extension methods for enumerables of <see cref="VsProject"/>. </summary>
    public static class VsProjectEnumerableExtensions
    {
        /// <summary>Sorts the given enumeration of <see cref="VsProject"/> by their required build order. </summary>
        /// <param name="projects">The projects to sort. </param>
        /// <returns>The projects in the correct build order. </returns>
        /// <exception cref="BuildOrderException">Thrown when the projects have cyclic dependencies. </exception>
        public static IEnumerable<VsProject> SortByBuildOrder(this IEnumerable<VsProject> projects)
        {
            var targetProjects = new List<VsProject>();
            var sourceProjects = projects.ToList();
            while (true)
            {
                var projectWithNoDependencies = sourceProjects.FirstOrDefault(p => !p.IsReferencingAnyProjects(sourceProjects));
                if (projectWithNoDependencies != null)
                {
                    foreach (var project in sourceProjects)
                    {
                        var item = project.ProjectReferences.SingleOrDefault(v => v.IsSameProject(projectWithNoDependencies));
                        if (item != null)
                            project.ProjectReferences.Remove(item);
                    }

                    targetProjects.Add(projectWithNoDependencies);
                    sourceProjects.Remove(projectWithNoDependencies);
                }
                else if (sourceProjects.Any())
                    throw new BuildOrderException("No build order could be built because the projects have cyclic references. ");
                else
                    return targetProjects;
            }
        }
    }
}