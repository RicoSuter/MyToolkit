//-----------------------------------------------------------------------
// <copyright file="ProjectDependencyResolver.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MyToolkit.Utilities
{
    public static class ProjectDependencyResolver
    {
        public static IEnumerable<string> GetProjectReferences(string projectPath)
        {
            var document = XDocument.Load(projectPath);
            foreach (var element in document.Descendants(XName.Get("ProjectReference", "http://schemas.microsoft.com/developer/msbuild/2003")))
                yield return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath), element.Attribute("Include").Value));
        }

        public static IEnumerable<string> GetBuildOrder(IEnumerable<string> projectPaths)
        {
            var order = new List<string>();
            var projects = projectPaths.Select(p => new VisualStudioProject
            {
                Path = p, 
                ProjectReferences = GetProjectReferences(p).ToList()
            }).ToList();

            while (true)
            {
                var projectWithNoDependencies = projects.FirstOrDefault(p => !p.IsDependentOnAny(projects));
                if (projectWithNoDependencies != null)
                {
                    foreach (var project in projects)
                    {
                        var item = project.ProjectReferences.SingleOrDefault(v => IsSameProject(v, projectWithNoDependencies.Path));
                        if (!string.IsNullOrEmpty(item))
                            project.ProjectReferences.Remove(item);
                    }

                    order.Add(projectWithNoDependencies.Path);
                    projects.Remove(projectWithNoDependencies);
                }
                else if (projects.Any())
                    throw new Exception("No build order could be built because the projects have cyclic references. ");
                else
                    return order;
            }
        }

        public static bool IsSameProject(string projectPath1, string projectPath2)
        {
            return Path.GetFullPath(projectPath1).ToLower() == Path.GetFullPath(projectPath2).ToLower();
        }
    }

    public class VisualStudioProject
    {
        public string Path { get; set; }
        public List<string> ProjectReferences { get; set; }

        public bool IsReferencingProject(VisualStudioProject project)
        {
            return ProjectReferences.Any(p => ProjectDependencyResolver.IsSameProject(p, project.Path));
        }

        public bool IsDependentOnAny(List<VisualStudioProject> projects)
        {
            return projects.Any(IsReferencingProject);
        }
    }
}
