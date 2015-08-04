using System;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Paging
{
    /// <summary>
    /// Event arguments for the navigating from event. 
    /// </summary>
    public class MtNavigatingCancelEventArgs
    {
        /// <summary>
        /// Gets or sets a value indiciating whether the navigation should be cancelled. 
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// Gets the page object which is involved in the navigation. 
        /// </summary>
        public object Content { get; internal set; }
        /// <summary>
        /// Gets the navigation mode. 
        /// </summary>
        public NavigationMode NavigationMode { get; internal set; }
        /// <summary>
        /// Gets the type of the page. 
        /// </summary>
        public Type SourcePageType { get; internal set; }

        public object Parameter { get; internal set; }
    }
}