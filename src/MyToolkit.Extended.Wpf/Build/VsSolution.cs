//-----------------------------------------------------------------------
// <copyright file="VsSolution.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio solution. </summary>
    public class VsSolution : VsObject
    {
        private static Type _solutionParserType = null;
        private readonly string _name;
        private readonly object _solutionParser;
        private List<VsProject> _projects;
        private readonly ProjectCollection _projectCollection;

        /// <summary>Initializes a new instance of the <see cref="VsSolution" /> class.</summary>
        /// <param name="path">The solution path.</param>
        /// <param name="projectCollection">The project collection.</param>
        private VsSolution(string path, ProjectCollection projectCollection)
            : base(path)
        {
            _projectCollection = projectCollection; 
            _name = System.IO.Path.GetFileNameWithoutExtension(path);

            _solutionParser = SolutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);
            using (var streamReader = new StreamReader(path))
            {
                _solutionParser.SetPropertyValue("SolutionReader", streamReader);
                _solutionParser.InvokeMethod("ParseSolution");
            }
        }

        /// <summary>Loads a solution from a given file path.</summary>
        /// <param name="filePath">The solution file path.</param>
        /// <param name="projectCollection">The project collection.</param>
        /// <returns>The solution.</returns>
        public static VsSolution Load(string filePath, ProjectCollection projectCollection)
        {
            var path = System.IO.Path.GetFullPath(filePath);
            return new VsSolution(path, projectCollection);
        }

        /// <summary>Gets the name of the project. </summary>
        public override string Name
        {
            get { return _name; }
        }

        /// <summary>Gets the list of projects. </summary>
        public List<VsProject> Projects
        {
            get
            {
                if (_projects == null)
                    LoadProjects();
                return _projects;
            }
        }

        /// <summary>Loads all projects of the solution. </summary>
        public void LoadProjects()
        {
            LoadProjects(false, null);
        }

        /// <summary>Loads all projects of the solution. </summary>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions. </param>
        /// <param name="projectCache">The project cache with already loaded projects. </param>
        public void LoadProjects(bool ignoreExceptions, Dictionary<string, VsProject> projectCache)
        {
            var projects = new List<VsProject>();
            var array = (Array)_solutionParser.GetPropertyValue("Projects");
            foreach (var projectObject in array)
            {
                try
                {
                    var relativePath = projectObject.GetPropertyValue("RelativePath").ToString();
                    var projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), relativePath));
                    if (projectPath.ToLower().EndsWith(".csproj") && File.Exists(projectPath))
                    {
                        if (projectCache != null && projectCache.ContainsKey(projectPath))
                            projects.Add(projectCache[projectPath]);
                        else
                            projects.Add(VsProject.Load(projectPath, _projectCollection));
                    }
                }
                catch (Exception)
                {
                    if (!ignoreExceptions)
                        throw;
                }
            }

            _projects = projects.OrderBy(p => p.Name).ToList();
        }

        /// <summary>Recursively loads all Visual Studio solutions from the given directory.</summary>
        /// <param name="path">The directory path.</param>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions (solutions with exceptions are not returned).</param>
        /// <param name="projectCollection">The project collection.</param>
        /// <returns>The solutions.</returns>
        public static Task<List<VsSolution>> LoadAllFromDirectoryAsync(string path, bool ignoreExceptions, ProjectCollection projectCollection)
        {
            return LoadAllFromDirectoryAsync(path, ignoreExceptions, projectCollection, ".sln", Load);
        }

        private static Type SolutionParserType
        {
            get
            {
                if (_solutionParserType == null)
                    _solutionParserType = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, " +
                                                       "Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
                return _solutionParserType;
            }
        }
    }
}
