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
using System.ServiceModel.PeerResolvers;
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
            LoadProjects(new List<VisualStudioProject>(), false);
        }

        /// <summary>Loads all projects of the solution. </summary>
        /// <param name="loadedProjects">The already loaded projects (used instead of reloading a project object). </param>
        /// <param name="ignoreExceptions">Specifies whether to ignore exceptions. </param>
        public void LoadProjects(IList<VisualStudioProject> loadedProjects, bool ignoreExceptions)
        {
            var content = File.ReadAllText(Path);

            var projects = new List<VisualStudioProject>();
            foreach (Match match in Regex.Matches(content.Replace("\r", ""), @"\nProject.*?""([^""]*?.csproj)""(.|\n)*?\nEndProject"))
            {
                var directory = System.IO.Path.GetDirectoryName(Path);
                if (directory != null)
                {
                    var projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(directory, match.Groups[1].Value));
                    if (File.Exists(projectPath))
                    {
                        var loadedProject = loadedProjects.SingleOrDefault(p => p.CheckProjectPath(projectPath));
                        if (loadedProject != null)
                            projects.Add(loadedProject);
                        else
                        {
                            try
                            {
                                projects.Add(VisualStudioProject.FromFilePath(projectPath));
                            }
                            catch (Exception)
                            {
                                if (!ignoreExceptions)
                                    throw;
                            }
                        }
                    }
                }
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
