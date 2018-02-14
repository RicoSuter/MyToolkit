//-----------------------------------------------------------------------
// <copyright file="ViewModelHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using MyToolkit.Model;
using MyToolkit.MVVM;

#if WINRT
using MyToolkit.Paging;
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Mvvm
{
    public static class ViewModelHelper
    {
        #if WINRT

        /// <summary>Binds the <see cref="ViewModelBase.IsLoading"/> property of the view model to 
        /// the progress bar visibility of the status bar (Windows Phone only). </summary>
        /// <param name="viewModel">The view model. </param>
        public static void BindViewModelToStatusBarProgress(ViewModelBase viewModel)
        {
            var type = Type.GetType(
                "Windows.UI.ViewManagement.StatusBar, " +
                "Windows, Version=255.255.255.255, Culture=neutral, " +
                "PublicKeyToken=null, ContentType=WindowsRuntime");
            var method = type.GetTypeInfo().GetDeclaredMethod("GetForCurrentView");

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.IsProperty<ViewModelBase>(i => i.IsLoading))
                {
                    if (viewModel.IsLoading)
                    {
                        dynamic statusBar = method.Invoke(null, null);
                        statusBar.ProgressIndicator.ShowAsync();
                    }
                    else
                    {
                        dynamic statusBar = method.Invoke(null, null);
                        statusBar.ProgressIndicator.HideAsync();
                    }
                }
            };
        }

        /// <summary>Initializes the view model and registers events so that the OnLoaded and OnUnloaded methods are called. 
        /// This method must be called in the constructor after the <see cref="InitializeComponent"/> method call. </summary>
        /// <param name="viewModel">The view model. </param>
        /// <param name="view">The view. </param>
        /// <param name="registerForStateHandling">Registers the view model also for state handling
        /// The view model has to implement <see cref="IStateHandlingViewModel"/> and the view must be a <see cref="MtPage"/>. </param>
        public static void RegisterViewModel(ViewModelBase viewModel, FrameworkElement view, bool registerForStateHandling = true)
        {
            viewModel.Initialize();

            view.Loaded += (sender, args) => viewModel.CallOnLoaded();
            view.Unloaded += (sender, args) => viewModel.CallOnUnloaded();

            if (registerForStateHandling && viewModel is IStateHandlingViewModel)
                RegisterViewModelForStateHandling((IStateHandlingViewModel) viewModel, (MtPage) view);
        }

        /// <summary>Registers the view model for state handling. </summary>
        /// <param name="viewModel">The view model. </param>
        /// <param name="page">The page. </param>
        public static void RegisterViewModelForStateHandling(IStateHandlingViewModel viewModel, MtPage page)
        {
            page.PageStateHandler.SaveState += (sender, args) => viewModel.OnSaveState(args);
            page.PageStateHandler.LoadState += (sender, args) => viewModel.OnLoadState(args);
        }

        #else

        /// <summary>Initializes the view model and registers events so that the OnLoaded and OnUnloaded methods are called. 
        /// This method must be called in the constructor after the <see cref="InitializeComponent"/> method call. </summary>
        /// <param name="viewModel">The view model. </param>
        /// <param name="view">The view. </param>
        public static void RegisterViewModel(ViewModelBase viewModel, FrameworkElement view)
        {
            viewModel.Initialize();

            view.Loaded += (sender, args) => viewModel.CallOnLoaded();
            view.Unloaded += (sender, args) => viewModel.CallOnUnloaded();

            view.Dispatcher.ShutdownStarted += delegate { viewModel.CallOnUnloaded(); };
        }

        #endif
    }
}
