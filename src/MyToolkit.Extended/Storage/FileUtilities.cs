//-----------------------------------------------------------------------
// <copyright file="FileUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

#if !WINRT
using System.IO.IsolatedStorage;
#endif

#if WP8 || WINRT
using System.Threading.Tasks;
using Windows.Storage;
#endif

#if WPF
using System.Threading.Tasks;
#endif

namespace MyToolkit.Storage
{
    public static class FileUtilities
    {
#if !WP7 && !WPF

        public static async Task WriteAllTextAsync(string fileName, string content)
        {
            var data = Encoding.UTF8.GetBytes(content);
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (var s = await file.OpenStreamForWriteAsync())
                await s.WriteAsync(data, 0, data.Length);
        }

        [DebuggerStepThrough]
        public static async Task<string> ReadAllTextAsync(string fileName)
        {
            // TODO: Should not catch exception?
            var folder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await folder.OpenStreamForReadAsync(fileName);
                using (var streamReader = new StreamReader(file, Encoding.UTF8))
                    return streamReader.ReadToEnd();
            }
            catch (Exception)
            {
                return null;
            }
        }

#endif

#if WP8 || WP7

        public static void WriteAllText(string fileName, string content)
        {
            var data = Encoding.UTF8.GetBytes(content);
            using (var folder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write, folder))
                    stream.Write(data, 0, data.Length);
            }
        }

        [DebuggerStepThrough]
        public static string ReadAllText(string fileName)
        {
            try
            {
                using (var folder = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    var file = folder.OpenFile(fileName, FileMode.Open);
                    using (var streamReader = new StreamReader(file, Encoding.UTF8))
                        return streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

#endif
    }
}
