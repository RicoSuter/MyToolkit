//-----------------------------------------------------------------------
// <copyright file="VisualStudioSolution.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio solution. </summary>
    public class VisualStudioSolution : VisualStudioObject
    {
        private List<VisualStudioProject> _projects;

        /// <summary>Loads a solution from a given file path. </summary>
        /// <param name="filePath">The solution file path. </param>
        /// <returns>The solution. </returns>
        public static VisualStudioSolution FromFilePath(string filePath)
        {
            var solution = new VisualStudioSolution();
            solution.Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            solution.Path = System.IO.Path.GetFullPath(filePath);
            solution.LoadProjects();
            return solution;
        }

        /// <summary>Gets the list of projects. </summary>
        public List<VisualStudioProject> Projects
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
            var content = File.ReadAllText(Path);

            var projects = new List<VisualStudioProject>();
            foreach (Match match in Regex.Matches(content.Replace("\r", ""), @"\nProject.*?""([^""]*?.csproj)"".*?\nEndProject"))
            {
                var projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), match.Groups[1].Value));

                if (File.Exists(projectPath))
                    projects.Add(VisualStudioProject.FromFilePath(projectPath));
            }

            _projects = projects.OrderBy(p => p.Name).ToList(); 
        }

        /// <summary>Recursively loads all Visual Studio solutions from the given directory. </summary>
        /// <param name="path">The directory path. </param>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions (solutions with exceptions are not returned). </param>
        /// <returns>The solutions. </returns>
        public static Task<List<VisualStudioSolution>> LoadAllFromDirectoryAsync(string path, bool ignoreExceptions)
        {
            return LoadAllFromDirectoryAsync(path, ignoreExceptions, ".sln", FromFilePath);
        }
    }
}
