//-----------------------------------------------------------------------
// <copyright file="VisualStudioProject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio project. </summary>
    public class VisualStudioProject
    {
        private List<VisualStudioProject> _projectReferences;
        private List<string> _assemblyReferences;
        private List<NuGetPackage> _nuGetReferences;

        private const string XmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        /// <summary>Gets the name of the project. </summary>
        public string Name { get; internal set; }

        /// <summary>Gets the path of the project file. </summary>
        public string Path { get; internal set; }

        /// <summary>Gets the list of referenced projects. </summary>
        public List<VisualStudioProject> ProjectReferences
        {
            get
            {
                if (_projectReferences == null)
                    LoadProjectReferences();
                return _projectReferences;
            }
        }

        /// <summary>Gets the list of referenced assemblies (DLLs). </summary>
        public List<string> AssemblyReferences
        {
            get
            {
                if (_assemblyReferences == null)
                    LoadAssemblyReferences();
                return _assemblyReferences;
            }
        }

        /// <summary>Gets the list of installed NuGet packages. </summary>
        public List<NuGetPackage> NuGetReferences
        {
            get
            {
                if (_nuGetReferences == null)
                    LoadNuGetReferences();
                return _nuGetReferences;
            }
        }

        /// <summary>Loads a project from a given file path. </summary>
        /// <param name="filePath">The project file path. </param>
        /// <returns>The project. </returns>
        public static VisualStudioProject FromFilePath(string filePath)
        {
            var document = XDocument.Load(filePath);
            var project = new VisualStudioProject();
            project.Path = System.IO.Path.GetFullPath(filePath);
            project.Name = document.Descendants(XName.Get("AssemblyName", XmlNamespace)).First().Value;
            project.LoadReferences();
            return project;
        }

        /// <summary>Recursively loads all Visual Studio projects from the given directory. </summary>
        /// <param name="path">The directory path. </param>
        /// <returns>The projects. </returns>
        public static List<VisualStudioProject> LoadAllFromDirectory(string path)
        {
            var projects = new List<VisualStudioProject>();
            foreach (var file in Directory.GetFiles(path))
            {
                var extension = System.IO.Path.GetExtension(file);
                if (extension != null && extension.ToLower() == ".csproj")
                    projects.Add(FromFilePath(file));
            }

            foreach (var directoy in Directory.GetDirectories(path))
                projects.AddRange(LoadAllFromDirectory(directoy));

            return projects;
        }

        /// <summary>Loads the project's referenced assemblies, projects and NuGet packages. </summary>
        public void LoadReferences()
        {
            LoadProjectReferences();
            LoadAssemblyReferences();
            LoadNuGetReferences();
        }
        
        /// <summary>Checks whether this project references the given project. </summary>
        /// <param name="project">The project. </param>
        /// <returns>True when the given project is referenced. </returns>
        public bool IsReferencingProject(VisualStudioProject project)
        {
            return ProjectReferences.Any(p => ProjectDependencyResolver.IsSameProject(p.Path, project.Path));
        }

        /// <summary>Checks whether the project is referencing any of the given projects. </summary>
        /// <param name="projects">The projects to check. </param>
        /// <returns>True when this project references any of the given projects. </returns>
        public bool IsReferencingAnyProjects(IEnumerable<VisualStudioProject> projects)
        {
            return projects.Any(IsReferencingProject);
        }

        private void LoadProjectReferences()
        {
            var document = XDocument.Load(Path);
            var references = new List<VisualStudioProject>();
            foreach (var element in document.Descendants(XName.Get("ProjectReference", XmlNamespace)))
            {
                references.Add(new VisualStudioProject
                {
                    Name = element.Descendants(XName.Get("Name", XmlNamespace)).First().Value,
                    Path = System.IO.Path.GetFullPath(System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(Path), element.Attribute("Include").Value))
                });
            }
            _projectReferences = references;
        }

        private void LoadAssemblyReferences()
        {
            var document = XDocument.Load(Path);
            var references = new List<string>();
            foreach (var element in document.Descendants(XName.Get("Reference", XmlNamespace)))
            {
                var include = element.Attribute("Include").Value;
                references.Add(include);
            }
            _assemblyReferences = references; 
        }

        private void LoadNuGetReferences()
        {
            var configurationPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "packages.config");
            var references = new List<NuGetPackage>();
            if (File.Exists(configurationPath))
            {
                var document = XDocument.Load(configurationPath);
                foreach (var element in document.Descendants("package"))
                {
                    references.Add(new NuGetPackage
                    {
                        Name = element.Attribute("id").Value,
                        Version = element.Attribute("version").Value
                    });
                }
            }
            _nuGetReferences = references; 
        }
    }
}