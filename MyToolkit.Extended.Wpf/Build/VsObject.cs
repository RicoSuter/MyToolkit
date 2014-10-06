//-----------------------------------------------------------------------
// <copyright file="VsObject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio object. </summary>
    public class VsObject
    {
        private string _path;

        /// <summary>Gets the id of the object. </summary>
        public string Id { get; private set; }

        /// <summary>Gets the name of the project. </summary>
        public string Name { get; internal set; }

        /// <summary>Gets the path of the project file. </summary>
        public string Path
        {
            get { return _path; }
            internal set
            {
                if (_path != value)
                {
                    _path = value;
                    Id = GetIdFromPath(_path);
                }
            }
        }

        /// <summary>Gets the root namespace. </summary>
        public string Namespace { get; internal set; }

        /// <summary>Gets or sets the target framework version. </summary>
        public string FrameworkVersion { get; internal set; }

        /// <summary>Gets the file name of the project. </summary>
        public string FileName
        {
            get { return System.IO.Path.GetFileName(Path); }
        }

        internal static string GetIdFromPath(string path)
        {
            return System.IO.Path.GetFullPath(path).ToLower();
        }

        internal static Task<List<T>> LoadAllFromDirectoryAsync<T>(string path, bool ignoreExceptions, string extension, Func<string, T> creator) 
            where T : new()
        {
            return Task.Run(async () =>
            {
                var tasks = new List<Task>();
                var projects = new List<T>();

                try
                {
                    foreach (var directoy in Directory.GetDirectories(path))
                        tasks.Add(LoadAllFromDirectoryAsync(directoy, ignoreExceptions, extension, creator));

                    foreach (var file in Directory.GetFiles(path))
                    {
                        var ext = System.IO.Path.GetExtension(file);
                        if (ext != null && ext.ToLower() == extension)
                        {
                            var lfile = file;
                            tasks.Add(Task.Run(() =>
                            {
                                try
                                {
                                    return creator(lfile);
                                }
                                catch (Exception)
                                {
                                    if (!ignoreExceptions)
                                        throw;
                                }
                                return default(T);
                            }));
                        }
                    }

                    await Task.WhenAll(tasks);

                    foreach (var task in tasks.OfType<Task<T>>().Where(t => t.Result != null))
                        projects.Add(task.Result);

                    foreach (var task in tasks.OfType<Task<List<T>>>())
                        projects.AddRange(task.Result);

                }
                catch (Exception)
                {
                    if (!ignoreExceptions)
                        throw;
                }

                return projects;
            });
        }
    }
}
