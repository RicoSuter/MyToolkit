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

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio solution. </summary>
    public class VsSolution : VsObject
    {
        private static Type _solutionParserType = null;
        private readonly string _name;
        private readonly object _solutionParser;
        private List<VsProject> _projects;

        /// <summary>Initializes a new instance of the <see cref="VsSolution"/> class. </summary>
        /// <param name="path">The solution path. </param>
        private VsSolution(string path)
            : base(path)
        {
            _name = System.IO.Path.GetFileNameWithoutExtension(path);

            _solutionParser = SolutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);
            using (var streamReader = new StreamReader(path))
            {
                _solutionParser.SetPropertyValue("SolutionReader", streamReader);
                _solutionParser.InvokeMethod("ParseSolution");
            }
        }

        /// <summary>Loads a solution from a given file path. </summary>
        /// <param name="filePath">The solution file path. </param>
        /// <returns>The solution. </returns>
        public static VsSolution Load(string filePath)
        {
            var path = System.IO.Path.GetFullPath(filePath);
            return new VsSolution(path);
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
            LoadProjects(false);
        }

        /// <summary>Loads all projects of the solution. </summary>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions. </param>
        public void LoadProjects(bool ignoreExceptions)
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
                        projects.Add(VsProject.Load(projectPath));
                }
                catch (Exception)
                {
                    if (!ignoreExceptions)
                        throw;
                }
            }

            _projects = projects.OrderBy(p => p.Name).ToList();
        }

        /// <summary>Recursively loads all Visual Studio solutions from the given directory. </summary>
        /// <param name="path">The directory path. </param>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions (solutions with exceptions are not returned). </param>
        /// <returns>The solutions. </returns>
        public static Task<List<VsSolution>> LoadAllFromDirectoryAsync(string path, bool ignoreExceptions)
        {
            return LoadAllFromDirectoryAsync(path, ignoreExceptions, ".sln", Load);
        }

        private static Type SolutionParserType
        {
            get
            {
                if (_solutionParserType == null)
                    _solutionParserType = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build", false, false);
                return _solutionParserType;
            }
        }
    }
}
