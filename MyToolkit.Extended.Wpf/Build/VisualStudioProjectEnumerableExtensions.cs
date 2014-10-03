//-----------------------------------------------------------------------
// <copyright file="VisualStudioProjectEnumerableExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace MyToolkit.Build
{
    /// <summary>Provides extension methods for enumerables of <see cref="VisualStudioProject"/>. </summary>
    public static class VisualStudioProjectEnumerableExtensions
    {
        /// <summary>Sorts the given enumeration of <see cref="VisualStudioProject"/> by their required build order. </summary>
        /// <param name="projects">The projects to sort. </param>
        /// <returns>The projects in the correct build order. </returns>
        /// <exception cref="BuildOrderException">Thrown when the projects have cyclic dependencies. </exception>
        public static IEnumerable<VisualStudioProject> SortByBuildOrder(this IEnumerable<VisualStudioProject> projects)
        {
            var targetProjects = new List<VisualStudioProject>();
            var sourceProjects = projects.ToList();
            while (true)
            {
                var projectWithNoDependencies = sourceProjects.FirstOrDefault(p => !p.IsReferencingAnyProjects(sourceProjects));
                if (projectWithNoDependencies != null)
                {
                    foreach (var project in sourceProjects)
                    {
                        var item = project.ProjectReferences.SingleOrDefault(v => v.HasSameProjectFile(projectWithNoDependencies));
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