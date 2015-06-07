//-----------------------------------------------------------------------
// <copyright file="ApplicationUpdater.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MyToolkit.Utilities
{
    /// <summary>Checks for application updates. </summary>
    public class ApplicationUpdater
    {
        private readonly string _updateUri;
        private readonly Version _currentVersion;

        /// <summary>Initializes a new instance of the <see cref="ApplicationUpdater"/> class. </summary>
        /// <param name="applicationAssembly">The application assembly. </param>
        /// <param name="updateUri">The update URI. </param>
        public ApplicationUpdater(Assembly applicationAssembly, string updateUri) 
            : this(applicationAssembly.GetName().Version, updateUri)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ApplicationUpdater"/> class. </summary>
        /// <param name="currentVersion">The current application version. </param>
        /// <param name="updateUri">The update URI. </param>
        public ApplicationUpdater(Version currentVersion, string updateUri)
        {
            _updateUri = updateUri;
            _currentVersion = currentVersion;
        }

        /// <summary>Checks for update and asks user if application should be updated 
        /// (this is currently beta: not localized and opens browser for download). </summary>
        public async Task CheckForUpdate(Window mainWindow)
        {
            try
            {
                var document = await Task.Run(() => XDocument.Load(_updateUri));
                var package = document.Descendants("package").First();

                var newVersionValue = package.Descendants("version").First().Value;
                var downloadLink = package.Descendants("download").First().Value;

                var newVersion = new Version(newVersionValue);
                if (_currentVersion < newVersion)
                {
                    var title = "New version is available";
                    var message = "A new version of the application is available: \n\n" +
                        "Current version: " + _currentVersion + "\n" +
                        "New version: " + newVersion + "\n\n" +
                        "Do you want to download the new version?";

                    if (MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Process.Start(downloadLink);
                        mainWindow.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                // ignore exceptions
                Debug.WriteLine("Exception in CheckForApplicationUpdate: " + exception.Message);
            }
        }
    }
}
