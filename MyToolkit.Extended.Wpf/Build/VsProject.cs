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
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using MyToolkit.Utilities;

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio project. </summary>
    public class VsProject : VsObject
    {
        private readonly static object _lock = new Object();
        private readonly static Dictionary<string, VsProject> _projects = new Dictionary<string, VsProject>();

        private List<VsProjectReference> _projectReferences;
        private List<AssemblyReference> _assemblyReferences;
        private List<NuGetPackageReference> _nuGetReferences;

        /// <exception cref="InvalidProjectFileException">The project file could not be found. </exception>
        private VsProject(string path)
            : base(path)
        {
            Project = new Project(path);
        }

        /// <summary>Loads a project from a given file path. </summary>
        /// <param name="filePath">The project file path. </param>
        /// <returns>The project. </returns>
        /// <exception cref="InvalidProjectFileException">The project file could not be found. </exception>
        public static VsProject Load(string filePath)
        {
            lock (_lock)
            {
                var id = GetIdFromPath(filePath);
                if (_projects.ContainsKey(id))
                    return _projects[id];

                var project = new VsProject(filePath);
                _projects[id] = project;
                return project;
            }
        }

        /// <summary>Gets the internal MSBuild project. </summary>
        public Project Project { get; private set; }

        /// <summary>Gets the list of referenced projects. </summary>
        public List<VsProjectReference> ProjectReferences
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
        public List<NuGetPackageReference> NuGetReferences
        {
            get
            {
                if (_nuGetReferences == null)
                    LoadNuGetReferences();
                return _nuGetReferences;
            }
        }

        /// <summary>Gets the name of the project. </summary>
        public override string Name
        {
            get { return Project.GetPropertyValue("AssemblyName"); }
        }

        /// <summary>Gets the root namespace. </summary>
        public string Namespace
        {
            get { return Project.GetPropertyValue("RootNamespace"); }
        }

        /// <summary>Gets or sets the target framework version. </summary>
        public string FrameworkVersion
        {
            get { return Project.GetPropertyValue("TargetFrameworkVersion"); }
        }

        /// <summary>Gets or sets the used tools version. </summary>
        public string ToolsVersion
        {
            get { return Project.ToolsVersion; }
        }

        /// <summary>Gets or sets the project's GUID. </summary>
        public string Guid
        {
            get { return Project.GetPropertyValue("ProjectGuid"); }
        }

        /// <summary>Checks whether the two project file paths are the same files. </summary>
        /// <param name="projectPath1">The first project file path. </param>
        /// <param name="projectPath2">The second project file path. </param>
        /// <returns>True when the paths are the same files. </returns>
        public static bool AreSameProjectFiles(string projectPath1, string projectPath2)
        {
            return System.IO.Path.GetFullPath(projectPath1).ToLower() == System.IO.Path.GetFullPath(projectPath2).ToLower();
        }
        
        /// <summary>Recursively loads all Visual Studio projects from the given directory. </summary>
        /// <param name="path">The directory path. </param>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions (projects with exceptions are not returned). </param>
        /// <returns>The projects. </returns>
        public static Task<List<VsProject>> LoadAllFromDirectoryAsync(string path, bool ignoreExceptions)
        {
            return LoadAllFromDirectoryAsync(path, ignoreExceptions, ".csproj", Load);
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
            return ProjectReferences.Any(p => p.IsSameProject(project));
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
        public bool IsProjectFile(string filePath)
        {
            return Id == GetIdFromPath(filePath);
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
        
        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object. </returns>
        public override string ToString()
        {
            return string.Format("{0}", Name);
        }

        public void LoadProjectReferences()
        {
            _projectReferences = Project.Items
                .Where(i => i.ItemType == "ProjectReference")
                .Select(p => VsProjectReference.Load(this, p))
                .OrderBy(r => r.Name)
                .ToList(); 
        }

        private string NuGetPackagesFile
        {
            get { return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "packages.config"); }
        }

        private void LoadAssemblyReferences()
        {
            _assemblyReferences = Project.Items
                .Where(i => i.ItemType == "Reference")
                .Select(reference => new AssemblyReference(reference))
                .OrderByThenBy(r => r.Name, r => VersionUtilities.FromString(r.Version))
                .ToList(); 
        }

        private void LoadNuGetReferences()
        {
            var configurationPath = NuGetPackagesFile;
            var references = new List<NuGetPackageReference>();
            if (File.Exists(configurationPath))
            {
                var document = XDocument.Load(configurationPath);
                foreach (var element in document.Descendants("package"))
                    references.Add(new NuGetPackageReference(element.Attribute("id").Value, element.Attribute("version").Value));
            }
            _nuGetReferences = references.OrderBy(r => r.Name).ToList(); 
        }
    }
}