//-----------------------------------------------------------------------
// <copyright file="ProjectRepository.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MyToolkit.Build
{
    /// <summary>Provides a fast lookup table to find projects by their paths. </summary>
    public class VsProjectRepository
    {
        private readonly Dictionary<string, VsProject> _projects;

        /// <summary>Initializes a new instance of the <see cref="VsProjectRepository"/> class with no projects. </summary>
        public VsProjectRepository()
        {
            _projects = new Dictionary<string, VsProject>();
        }

        /// <summary>Initializes a new instance of the <see cref="VsProjectRepository"/> class. </summary>
        /// <param name="projects">The projects. </param>
        public VsProjectRepository(IEnumerable<VsProject> projects)
        {
            _projects = projects.ToDictionary(p => p.Id, p => p);
        }

        /// <summary>Returns the cached project in the repository or returns null. </summary>
        /// <param name="projectPath">The project path. </param>
        /// <returns>The project. </returns>
        public VsProject TryGetProject(string projectPath)
        {
            var path = Path.GetFullPath(projectPath).ToLower();
            if (_projects.ContainsKey(path))
                return _projects[path];
            return null;
        }
    }
}
