//-----------------------------------------------------------------------
// <copyright file="ApplicationUpdater.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Win32;
using MyToolkit.UI;

namespace MyToolkit.Utilities
{
    /// <summary>Checks for application updates. </summary>
    public class ApplicationUpdater
    {
        private readonly string _updateUri;
        private readonly Version _currentVersion;
        private readonly string _installerFileName;

        /// <summary>Initializes a new instance of the <see cref="ApplicationUpdater" /> class.</summary>
        /// <param name="installerFileName">The installer file name.</param>
        /// <param name="applicationAssembly">The application assembly.</param>
        /// <param name="updateUri">The update URI.</param>
        public ApplicationUpdater(string installerFileName, Assembly applicationAssembly, string updateUri) 
            : this(installerFileName, applicationAssembly.GetName().Version, updateUri)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ApplicationUpdater"/> class. </summary>
        /// <param name="installerFileName">The installer file name.</param>
        /// <param name="currentVersion">The current application version. </param>
        /// <param name="updateUri">The update URI. </param>
        public ApplicationUpdater(string installerFileName, Version currentVersion, string updateUri)
        {
            _installerFileName = installerFileName;
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
                        "Do you want to download and install the new version?";

                    if (MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(_installerFileName) +
                            "_" + newVersion +
                            Path.GetExtension(_installerFileName);

                        var dialog = new ProgressDialog("Downloading...", "Downloading the update to version " + newVersion + "...");
                        dialog.Owner = mainWindow; 
                        dialog.IsIndeterminate = true;

                        var cancellation = new CancellationTokenSource();
                        ThreadPool.QueueUserWorkItem(async delegate {
                            try
                            {
                                var client = new HttpClient();
                                var response = await client.GetAsync(new Uri(downloadLink, UriKind.Absolute), cancellation.Token);
                                var filePath = Path.Combine(Path.GetTempPath(), fileName);

                                using (var fileStream = File.Create(filePath))
                                {
                                    var webStream = await response.Content.ReadAsStreamAsync();
                                    webStream.Seek(0, SeekOrigin.Begin);
                                    webStream.CopyTo(fileStream);
                                }

                                Process.Start(filePath);

                                await dialog.Dispatcher.InvokeAsync(() =>
                                {
                                    dialog.Close();
                                    mainWindow.Close();
                                });
                            }
                            catch (OperationCanceledException) { }
                            catch (Exception exception)
                            {
                                MessageBox.Show("A download error occurred: \n" + exception.Message, "Download Error");
                            }
                        });

                        dialog.Closed += (sender, args) => { cancellation.Cancel(); };
                        dialog.ShowDialog();
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
