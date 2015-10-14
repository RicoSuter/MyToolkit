//-----------------------------------------------------------------------
// <copyright file="VsProject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using MyToolkit.Utilities;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio project. </summary>
    public class VsProject : VsObject
    {
        private List<VsProjectReference> _projectReferences;
        private List<AssemblyReference> _assemblyReferences;
        private List<NuGetPackageReference> _nuGetReferences;
        private string _nuGetPackagesPath;

        /// <summary>Initializes a new instance of the <see cref="VsProject" /> class.</summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="projectCollection">The project collection.</param>
        /// <exception cref="InvalidProjectFileException">The project file could not be found.</exception>
        private VsProject(string filePath, ProjectCollection projectCollection)
            : base(filePath)
        {
            Project = projectCollection.LoadProject(filePath);
            Solutions = new ObservableCollection<VsSolution>();
            LoadNuSpecFile(filePath);
        }

        /// <summary>Loads a project from a given file path, if the project has already been loaded before, the same reference is returned.</summary>
        /// <param name="filePath">The project file path.</param>
        /// <param name="projectCollection">The project collection.</param>
        /// <returns>The project.</returns>
        /// <exception cref="InvalidProjectFileException">The project file could not be found.</exception>
        public static VsProject Load(string filePath, ProjectCollection projectCollection)
        {
            return new VsProject(filePath, projectCollection);
        }

        /// <summary>Gets the internal MSBuild project. </summary>
        public Project Project { get; private set; }

        /// <summary>Gets or sets the file path to the project's NuSpec file if available.</summary>
        public string NuSpecFilePath { get; private set; }

        /// <summary>Gets or sets the NuGet package identifier if a NuSpec file could be found.</summary>
        public string NuGetPackageId { get; private set; }

        /// <summary>Gets or sets the NuGet package title if a NuSpec file could be found.</summary>
        public string NuGetPackageTitle { get; private set; }

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

        /// <summary>Gets the assembly name of the project. </summary>
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

        /// <summary>Gets the .NET target framework version.</summary>
        public string TargetFrameworkVersion
        {
            get { return Project.GetPropertyValue("TargetFrameworkVersion"); }
        }
        
        /// <summary>Gets the output path.</summary>
        public string OutputPath
        {
            get { return Project.GetPropertyValue("OutputPath"); }
        }

        /// <summary>Gets the output type (Exe, Library, ...).</summary>
        public string OutputType
        {
            get { return Project.GetPropertyValue("OutputType"); }
        }

        /// <summary>Gets the project type GUIDs. </summary>
        public string[] ProjectTypeGuids
        {
            get
            {
                var guids = Project.GetPropertyValue("ProjectTypeGuids");
                if (!string.IsNullOrEmpty(guids))
                    return guids.Split(';');
                return new string[] { };
            }
        }

        /// <summary>Gets the project types. </summary>
        public VsProjectType[] ProjectTypes
        {
            get { return ProjectTypeGuids.Select(ProjectTypeGuidMapper.ResolveGuid).ToArray(); }
        }
        
        /// <summary>Gets the solutions of the project (filled by <see cref="VsSolution.LoadProjects()"/>).</summary>
        public IList<VsSolution> Solutions { get; internal set; }


        /// <summary>Checks whether the two project file paths are the same files. </summary>
        /// <param name="projectPath1">The first project file path. </param>
        /// <param name="projectPath2">The second project file path. </param>
        /// <returns>True when the paths are the same files. </returns>
        public static bool AreSameProjectFiles(string projectPath1, string projectPath2)
        {
            return System.IO.Path.GetFullPath(projectPath1).ToLower() == System.IO.Path.GetFullPath(projectPath2).ToLower();
        }

        /// <summary>Recursively loads all Visual Studio projects from the given directory.</summary>
        /// <param name="path">The directory path.</param>
        /// <param name="pathFilter">The path filter.</param>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions (projects with exceptions are not returned).</param>
        /// <param name="projectCollection">The project collection.</param>
        /// <param name="errors">The loading errors (out param).</param>
        /// <returns>The projects.</returns>
        public static Task<List<VsProject>> LoadAllFromDirectoryAsync(string path, string pathFilter, bool ignoreExceptions, ProjectCollection projectCollection, Dictionary<string, Exception> errors = null)
        {
            return LoadAllFromDirectoryAsync(path.Replace('/', '\\'), pathFilter.Replace('/', '\\'), ignoreExceptions, projectCollection, ".csproj", Load, errors);
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

        /// <summary>Gets the NuGet packages directory path relative to the project file.</summary>
        public string NuGetPackagesPath
        {
            get
            {
                if (_nuGetPackagesPath == null)
                    _nuGetPackagesPath = FindNuGetPackagesPath(System.IO.Path.GetDirectoryName(Path), string.Empty, null);
                return _nuGetPackagesPath;
            }
        }
        
        private string FindNuGetPackagesPath(string directory, string prefix, string fallbackDirectory)
        {
            var configFile = System.IO.Path.Combine(directory, "nuget.config");
            if (File.Exists(configFile))
            {
                using (var stream = File.Open(configFile, FileMode.Open))
                {
                    var document = new XPathDocument(stream);
                    var navigator = document.CreateNavigator();
                    var packagesPath = navigator.Select("configuration/config/add[@key = \"repositorypath\"]/@value");
                    if (packagesPath.Count == 1)
                    {
                        packagesPath.MoveNext();
                        return prefix + packagesPath.Current.Value + "\\";
                    }
                }
            }

            if (string.IsNullOrEmpty(fallbackDirectory))
            {
                if (Directory.GetFiles(directory).Any(f => f.EndsWith(".sln")))
                    fallbackDirectory = prefix + "packages\\";
            }

            directory = System.IO.Path.GetDirectoryName(directory);
            if (!string.IsNullOrEmpty(directory))
                return FindNuGetPackagesPath(directory, prefix + "..\\", fallbackDirectory);

            return fallbackDirectory;
        }

        private bool HasNuGetPackagesFile
        {
            get { return File.Exists(NuGetPackagesFile); }
        }

        private bool HasNuGetProjectFile
        {
            get { return File.Exists(NuGetProjectFile); }
        }

        private string NuGetPackagesFile
        {
            get { return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "packages.config"); }
        }

        private string NuGetProjectFile
        {
            get { return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "project.json"); }
        }

        private void LoadAssemblyReferences()
        {
            _assemblyReferences = Project.Items
                .Where(i => i.ItemType == "Reference")
                .Select(reference => new AssemblyReference(this, reference))
                .OrderByThenBy(r => r.Name, r => VersionUtilities.FromString(r.Version))
                .ToList();
        }

        private void LoadNuSpecFile(string filePath)
        {
            NuSpecFilePath = Directory.GetFiles(System.IO.Path.GetDirectoryName(filePath), "*.nuspec", SearchOption.AllDirectories).FirstOrDefault();

            if (NuSpecFilePath != null)
            {
                using (var stream = File.Open(NuSpecFilePath, FileMode.Open))
                {
                    var reader = new NuspecReader(stream);

                    var packageId = reader.GetId();
                    if (packageId != null && packageId.ToLower() == "$id$")
                        packageId = Name;

                    NuGetPackageId = packageId;
                    NuGetPackageTitle = Name; 

                    var metadata = reader.GetMetadata().ToArray();
                    if (metadata.Any(p => p.Key == "title"))
                    {
                        var titlePair = metadata.SingleOrDefault(p => p.Key == "title");
                        NuGetPackageTitle = titlePair.Value;
                    }
                }
            }
        }

        private void LoadNuGetReferences()
        {
            var references = new List<NuGetPackageReference>();

            if (HasNuGetPackagesFile)
            {
                var document = XDocument.Load(NuGetPackagesFile);
                foreach (var element in document.Descendants("package"))
                    references.Add(new NuGetPackageReference(element.Attribute("id").Value, element.Attribute("version").Value));
            }

            if (HasNuGetProjectFile)
            {
                var document = JToken.Parse(File.ReadAllText(NuGetProjectFile, Encoding.UTF8));
                var dependencies = document["dependencies"];
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies.OfType<JProperty>())
                        references.Add(new NuGetPackageReference(dependency.Name, dependency.Value.Value<string>()));
                }
            }

            _nuGetReferences = references.OrderBy(r => r.Name).ToList();
        }
    }
}