//-----------------------------------------------------------------------
// <copyright file="NuGetPackageReference.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyToolkit.Build.Exceptions;
using MyToolkit.Utilities;

namespace MyToolkit.Build
{
    /// <summary>Describes an installed NuGet package. </summary>
    public class NuGetPackageReference : VsReferenceBase
    {
        private readonly static Dictionary<string, List<NuGetPackageReference>> _cache =
            new Dictionary<string, List<NuGetPackageReference>>();

        private readonly object _lock = new object();
        private bool? _isNuGetOrgPackage;
        private XDocument _nuGetDependencies = null;

        private readonly string _name;
        private readonly string _version;

        /// <summary>Initializes a new instance of the <see cref="NuGetPackageReference"/> class. </summary>
        /// <param name="name">The package id. </param>
        /// <param name="version">The package version. </param>
        internal NuGetPackageReference(string name, string version)
        {
            _name = name;
            _version = version;
        }
        
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
        
        /// <exception cref="WebException">There was a connection exception. </exception>
        public async Task<bool> IsNuGetOrgPackageAsync()
        {
            lock (_lock)
            {
                if (_isNuGetOrgPackage.HasValue)
                    return _isNuGetOrgPackage.Value;
            }

            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    _isNuGetOrgPackage = IsNuGetOrgPackageInternal();
                    return _isNuGetOrgPackage.Value;
                }
            });
        }

        /// <summary>Loads the package dependencies from NuGet.org. </summary>
        /// <returns>The package dependencies. </returns>
        /// <exception cref="NuGetPackageNotFoundException">The NuGet package could not be found on nuget.org</exception>
        /// <exception cref="WebException">There was a connection exception. </exception>
        public async Task<IEnumerable<NuGetPackageReference>> GetDependenciesAsync()
        {
            var cacheKey = Name + ":" + Version;
            lock (_lock)
            {
                if (_cache.ContainsKey(cacheKey))
                    return _cache[cacheKey];
            }

            await IsNuGetOrgPackageAsync();

            return await Task.Run(() =>
            {
                try
                {
                    lock (_lock)
                    {
                        var dependencies = _nuGetDependencies.Root
                            .Value.Split('|')
                            .Where(p => !string.IsNullOrEmpty(p))
                            .Select(p =>
                            {
                                var arr = p.Split(':');
                                return new NuGetPackageReference(arr[0], arr[1]);
                            })
                            .DistinctBy(p => p.Name + ":" + p.Version)
                            .ToList();

                        _cache[cacheKey] = dependencies.ToList();
                        return dependencies;
                    }
                }
                catch (WebException exception)
                {
                    var message = string.Format("The NuGet package '{0}' {1}' could not be found on nuget.org", Name, Version);
                    throw new NuGetPackageNotFoundException(message, exception);
                }                
            });
        }

        /// <summary>Recursively loads all package dependencies from NuGet.org. </summary>
        /// <returns>All package dependencies. </returns>
        public async Task<IEnumerable<NuGetPackageReference>> GetAllDependenciesAsync()
        {
            var externalDependencies = (await GetDependenciesAsync()).ToList();
            foreach (var package in externalDependencies.ToArray())
                externalDependencies.AddRange(await package.GetAllDependenciesAsync());
            return externalDependencies;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object. </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Version);
        }

        /// <exception cref="WebException">There was a connection exception. </exception>
        [DebuggerStepThrough]
        private bool IsNuGetOrgPackageInternal()
        {
            try
            {
                var url = string.Format("https://www.nuget.org/api/v2/Packages(Id='{0}',Version='{1}')/Dependencies", Name, Version);
                _nuGetDependencies = XDocument.Load(url);
                return true;
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    return false;
                throw;
            }
        }
    }
}