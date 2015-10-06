//-----------------------------------------------------------------------
// <copyright file="VsObject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;

namespace MyToolkit.Build
{
    /// <summary>Describes a Visual Studio object. </summary>
    public abstract class VsObject
    {
        /// <summary>Initializes a new instance of the <see cref="VsObject"/> class. </summary>
        /// <param name="path">The path to the object. </param>
        protected VsObject(string path)
        {
            Id = GetIdFromPath(path);
            Path = path;
        }

        /// <summary>Gets the id of the object. </summary>
        public string Id { get; private set; }

        /// <summary>Gets the path of the project file. </summary>
        public string Path { get; private set; }

        /// <summary>Gets the name of the project. </summary>
        public abstract string Name { get; }

        /// <summary>Gets the file name of the project. </summary>
        public string FileName
        {
            get { return System.IO.Path.GetFileName(Path); }
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>. </summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false. </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            if (!(obj is VsObject))
                return false;

            return Id == ((VsObject)obj).Id;
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object"/>. </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        internal static string GetIdFromPath(string path)
        {
            return System.IO.Path.GetFullPath(path).ToLower();
        }

        internal static Task<List<T>> LoadAllFromDirectoryAsync<T>(string path, string pathFilter, bool ignoreExceptions, ProjectCollection projectCollection, string extension, Func<string, ProjectCollection, T> creator, Dictionary<string, Exception> errors)
        {
            var pathFilterTerms = pathFilter.ToLower().Split(' ');

            return Task.Run(async () =>
            {
                var tasks = new List<Task<T>>();
                var projects = new List<T>();

                try
                {
                    var files = Directory.GetFiles(path, "*" + extension, SearchOption.AllDirectories);
                    foreach (var file in files.Distinct().Where(s => pathFilterTerms.All(s.ToLower().Contains)))
                    {
                        var ext = System.IO.Path.GetExtension(file);
                        if (ext != null && ext.ToLower() == extension)
                        {
                            var lFile = file;
                            tasks.Add(Task.Run(() =>
                            {
                                try
                                {
                                    return creator(lFile, projectCollection);
                                }
                                catch (Exception exception)
                                {
                                    if (!ignoreExceptions)
                                        throw;

                                    if (errors != null)
                                        errors[lFile] = exception;
                                }
                                return default(T);
                            }));
                        }
                    }

                    await Task.WhenAll(tasks);

                    foreach (var task in tasks.Where(t => t.Result != null))
                        projects.Add(task.Result);
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
