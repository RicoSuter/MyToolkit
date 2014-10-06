//-----------------------------------------------------------------------
// <copyright file="VsProject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyToolkit.Utilities;

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio project. </summary>
    public class VsProject : VsObject
    {
        private List<VsProject> _projectReferences;
        private List<AssemblyReference> _assemblyReferences;
        private List<NuGetPackage> _nuGetReferences;

        private const string XmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        /// <summary>Gets the list of referenced projects. </summary>
        public List<VsProject> ProjectReferences
        {
            get
            {
                if (_projectReferences == null)
                    LoadProjectReferences();
                return _projectReferences;
            }
        }

        /// <summary>Gets the list of referenced assemblies (DLLs). </summary>
        public List<AssemblyReference> AssemblyReferences
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

        /// <summary>Checks whether the two project file paths are the same files. </summary>
        /// <param name="projectPath1">The first project file path. </param>
        /// <param name="projectPath2">The second project file path. </param>
        /// <returns>True when the paths are the same files. </returns>
        public static bool AreSameProjectFiles(string projectPath1, string projectPath2)
        {
            return System.IO.Path.GetFullPath(projectPath1).ToLower() == System.IO.Path.GetFullPath(projectPath2).ToLower();
        }

        /// <summary>Loads a project from a given file path. </summary>
        /// <param name="filePath">The project file path. </param>
        /// <returns>The project. </returns>
        public static VsProject FromFilePath(string filePath)
        {
            var document = XDocument.Load(filePath);
            var project = new VsProject();

            project.Path = System.IO.Path.GetFullPath(filePath);
            project.Name = document.Descendants(XName.Get("AssemblyName", XmlNamespace)).First().Value;
            project.Namespace = document.Descendants(XName.Get("RootNamespace", XmlNamespace)).First().Value;

            var frameworkVersionTag = document.Descendants(XName.Get("TargetFrameworkVersion", XmlNamespace)).FirstOrDefault();
            project.FrameworkVersion = frameworkVersionTag != null ? frameworkVersionTag.Value : String.Empty;

            project.LoadReferences();

            return project;
        }

        /// <summary>Recursively loads all Visual Studio projects from the given directory. </summary>
        /// <param name="path">The directory path. </param>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions (projects with exceptions are not returned). </param>
        /// <returns>The projects. </returns>
        public static Task<List<VsProject>> LoadAllFromDirectoryAsync(string path, bool ignoreExceptions)
        {
            return LoadAllFromDirectoryAsync(path, ignoreExceptions, ".csproj", FromFilePath);
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
        public bool IsReferencingProject(VsProject project)
        {
            return ProjectReferences.Any(p => p.HasSameProjectFile(project));
        }

        /// <summary>Checks whether the project is referencing any of the given projects. </summary>
        /// <param name="projects">The projects to check. </param>
        /// <returns>True when this project references any of the given projects. </returns>
        public bool IsReferencingAnyProjects(IEnumerable<VsProject> projects)
        {
            return projects.Any(IsReferencingProject);
        }

        /// <summary>Checks whether both projects are loaded from the same file. </summary>
        /// <param name="filePath">The project path. </param>
        /// <returns>true when both projects are loaded from the same file. </returns>
        public bool CheckProjectPath(string filePath)
        {
            return AreSameProjectFiles(Path, filePath);
        }

        /// <summary>Checks whether both projects are loaded from the same file. </summary>
        /// <param name="project">The other project. </param>
        /// <returns>true when both projects are loaded from the same file. </returns>
        public bool HasSameProjectFile(VsProject project)
        {
            return AreSameProjectFiles(Path, project.Path);
        }

        private void LoadProjectReferences()
        {
            var document = XDocument.Load(Path);
            var references = new List<VsProject>();
            foreach (var element in document.Descendants(XName.Get("ProjectReference", XmlNamespace)))
            {
                references.Add(new VsProject
                {
                    Name = element.Descendants(XName.Get("Name", XmlNamespace)).First().Value,
                    Path = System.IO.Path.GetFullPath(System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(Path), element.Attribute("Include").Value))
                });
            }
            _projectReferences = references.OrderBy(r => r.Name).ToList(); 
        }

        private void LoadAssemblyReferences()
        {
            var document = XDocument.Load(Path);
            var references = new List<AssemblyReference>();
            foreach (var element in document.Descendants(XName.Get("Reference", XmlNamespace)))
            {
                var include = element.Attribute("Include").Value;
                references.Add(new AssemblyReference(include));
            }
            _assemblyReferences = references.OrderByThenBy(r => r.Name, r => VersionUtilities.FromString(r.Version)).ToList(); 
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
            _nuGetReferences = references.OrderBy(r => r.Name).ToList(); 
        }
    }
}