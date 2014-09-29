//-----------------------------------------------------------------------
// <copyright file="ObservableGroupCollection.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Collections
{
    /// <summary>Provides a collection of groups which is useful in Windows 8's GridView control. </summary>
    /// <typeparam name="TItem">The item type. </typeparam>
	public class ObservableGroupCollection<TItem> : ObservableCollection<IGroup>
	{
        private CollectionViewSource _view;
        
        /// <summary>Creates and adds a group to the collection. </summary>
        /// <param name="title">The title of the group. </param>
        /// <param name="topItemsCount">The top items count. </param>
        /// <returns>The created <see cref="ExtendedGroup{T}"/></returns>
		public ExtendedGroup<TItem> AddGroup(string title, int topItemsCount = -1)
		{
			return AddGroup(title, new List<TItem>(), topItemsCount);
		}

        /// <summary>Creates and adds a group to the collection. </summary>
        /// <param name="title">The title of the group. </param>
        /// <param name="items">The items in the group. </param>
        /// <param name="topItemsCount">The top items count. </param>
        /// <returns>The created <see cref="ExtendedGroup{T}"/></returns>
        public ExtendedGroup<TItem> AddGroup(string title, IEnumerable<TItem> items, int topItemsCount = -1)
		{
			var group = new ExtendedGroup<TItem>(title, items, topItemsCount);
			Add(group);
			return group;
		}

        /// <summary>The <see cref="CollectionViewSource"/> to use in XAML. </summary>
		public CollectionViewSource View
		{
			get
			{
				if (_view == null)
				{
					_view = new CollectionViewSource();
					_view.IsSourceGrouped = true;
					_view.ItemsPath = new PropertyPath("TopItems");
					_view.Source = this; 
				}
				return _view; 
			}
		}
	}
}