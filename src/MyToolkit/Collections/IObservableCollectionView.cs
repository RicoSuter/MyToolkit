//-----------------------------------------------------------------------
// <copyright file="IObservableCollectionView.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MyToolkit.Collections
{
    /// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with automatic sorting and filtering. </summary>
    public interface IObservableCollectionView : IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>Gets or sets a value indicating whether the view should automatically be updated when needed. 
        /// Disable this flag when doing multiple of operations on the underlying collection. 
        /// Enabling this flag automatically updates the view if needed. </summary>
        bool IsTracking { get; set; }

        /// <summary>Gets or sets the maximum number of items in the view. </summary>
        int Limit { get; set; }

        /// <summary>Gets or sets the offset from where the results a selected. </summary>
        int Offset { get; set; }

        /// <summary>Gets or sets a value indicating whether to sort ascending or descending. </summary>
        bool Ascending { get; set; }

        /// <summary>Gets or sets the filter (a Func{TItem, bool} object). </summary>
        object Filter { get; set; }

        /// <summary>Gets or sets the order (a Func{TItem, object} object). </summary>
        object Order { get; set; }

        /// <summary>Refreshes the view. </summary>
        void Refresh();
    }
}