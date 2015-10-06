//-----------------------------------------------------------------------
// <copyright file="AssemblyReference.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using Microsoft.Build.Evaluation;

namespace MyToolkit.Build
{
    /// <summary>Describes a referenced assembly. </summary>
    public class AssemblyReference : VsReferenceBase
    {
        private readonly string _name;
        private readonly string _version;

        /// <summary>Initializes a new instance of the <see cref="AssemblyReference" /> class.</summary>
        /// <param name="project">The project.</param>
        /// <param name="projectItem">The raw name.</param>
        internal AssemblyReference(VsProject project, ProjectItem projectItem)
        {
            ProjectItem = projectItem;

            var array = ProjectItem.EvaluatedInclude.Split(',');

            _name = array[0];
            _version = "Any";

            LoadHintPath(project, projectItem);

            foreach (var tuple in array.Skip(1)
                .Select(n => n.Trim().Split('='))
                .Select(n => new Tuple<string, string>(n[0], n[1])))
            {
                switch (tuple.Item1)
                {
                    case "Version":
                        _version = tuple.Item2;
                        break;
                }
            }
        }

        /// <summary>Gets the full name. </summary>
        public ProjectItem ProjectItem { get; private set; }

        /// <summary>Gets the name of the NuGet package. </summary>
        public override string Name
        {
            get { return _name; }
        }

        /// <summary>Gets the version of the installed NuGet package. </summary>
        public override string Version
        {
            get { return _version; }
        }

        /// <summary>Gets the assembly hint path.</summary>
        public string HintPath { get; private set; }

        /// <summary>Gets a value indicating whether the assembly reference has been added by using NuGet.</summary>
        public bool IsNuGetReference { get; private set; }

        /// <summary>Gets the NuGet package name.</summary>
        public string NuGetPackageName { get; private set; }

        /// <summary>Gets the NuGet package version.</summary>
        public string NuGetPackageVersion { get; private set; }

        private void LoadHintPath(VsProject project, ProjectItem projectItem)
        {
            HintPath = projectItem.Metadata.Any(m => m.Name == "HintPath") ? projectItem.Metadata.Single(m => m.Name == "HintPath").EvaluatedValue : null;
            if (HintPath != null)
            {
                var packagesPath = project.NuGetPackagesPath;
                if (HintPath.StartsWith(packagesPath))
                {
                    var startIndex = packagesPath.Length;
                    var endIndex = HintPath.IndexOf("\\", startIndex, StringComparison.InvariantCulture);
                    if (endIndex != -1)
                    {
                        IsNuGetReference = true;

                        var isVersionPart = true;
                        var nuGetPackageVersion = string.Empty;
                        var nuGetPackage = string.Empty;

                        var segments = HintPath.Substring(startIndex, endIndex - startIndex).Split('.');
                        foreach (var segment in segments.Reverse())
                        {
                            if (isVersionPart)
                            {
                                int number = 0;
                                if (int.TryParse(segment, out number))
                                {
                                    nuGetPackageVersion = segment + "." + nuGetPackageVersion;
                                }
                                else
                                {
                                    nuGetPackage = segment;
                                    isVersionPart = false;
                                }
                            }
                            else
                                nuGetPackage = segment + "." + nuGetPackage;
                        }

                        NuGetPackageName = nuGetPackage.Trim('.');
                        NuGetPackageVersion = nuGetPackageVersion.Trim('.');
                    }
                }
            }
        }

        
    }
}