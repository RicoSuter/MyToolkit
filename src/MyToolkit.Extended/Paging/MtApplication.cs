//-----------------------------------------------------------------------
// <copyright file="MtApplication.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace MyToolkit.Paging
{
    public abstract class MtApplication : Application
    {
        /// <summary>Initializes a new instance of the <see cref="MtApplication"/> class. </summary>
        protected MtApplication()
        {
            Suspending += OnSuspending;
        }

        /// <summary>Gets the root frame.</summary>
        public MtFrame RootFrame { get; private set; }

        /// <summary>Gets the content of the window.</summary>
        public FrameworkElement WindowContent { get; private set; }

        /// <summary>Gets the type of the start page (first page when launching application). </summary>
        public abstract Type StartPageType { get; }

        /// <summary>Called when a new instance of the application has been created. </summary>
        /// <param name="frame">The frame. </param>
        /// <param name="args">The launch arguments.</param>
        public virtual Task OnInitializedAsync(MtFrame frame, ApplicationExecutionState args)
        {
            return null; // Must be empty
        }

        /// <summary>Creates the window content element. </summary>
        /// <returns>The element. </returns>
        public virtual FrameworkElement CreateWindowContentElement()
        {
            return new MtFrame();
        }

        /// <summary>Gets the root frame given the window content element. </summary>
        /// <returns>The root frame. </returns>
        public virtual MtFrame GetFrame(UIElement windowContentElement)
        {
            return (MtFrame)windowContentElement;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            await InitializeFrameAsync(args.PreviousExecutionState);
        }

        /// <summary>Creates the application's root frame and loads the first page if needed. 
        /// Also calls <see cref="OnInitializedAsync"/> when the application is instantiated the first time. </summary>
        /// <param name="executionState">The application execution state. </param>
        /// <returns>The task. </returns>
        protected async Task InitializeFrameAsync(ApplicationExecutionState executionState)
        {
            if (Window.Current.Content == null)
            {
                WindowContent = CreateWindowContentElement();
                RootFrame = GetFrame(WindowContent);

                MtSuspensionManager.RegisterFrame(RootFrame, "AppFrame");
                if (executionState == ApplicationExecutionState.Terminated)
                    await RestoreStateAsync();

                var task = OnInitializedAsync(RootFrame, executionState);
                if (task != null)
                    await task;

                Window.Current.Content = WindowContent;
            }
            else
                RootFrame = GetFrame(Window.Current.Content);

            if (RootFrame.Content == null)
                RootFrame.Initialize(StartPageType);

            Window.Current.Activate();
        }

        /// <summary>Restores the saved page states using the <see cref="MtSuspensionManager"/>. </summary>
        /// <returns>The task. </returns>
        protected virtual async Task RestoreStateAsync()
        {
            try
            {
                await MtSuspensionManager.RestoreAsync();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("MtApplication.RestoreStateAsync: Could not deserialize the page frame state: \n" +
                    "Own types must be registered with the MtSuspensionManager.KnownTypes.Add() method\n" + exception.ToString());
            }
        }

        /// <summary>Loads the saved page states using the <see cref="MtSuspensionManager"/>. </summary>
        /// <returns>The task. </returns>
        protected virtual async Task SaveStateAsync()
        {
            await MtSuspensionManager.SaveAsync();
        }

        /// <summary>Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact. </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SaveStateAsync();
            deferral.Complete();
        }
    }
}

#endif