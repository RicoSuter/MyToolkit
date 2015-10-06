//-----------------------------------------------------------------------
// <copyright file="ObservableGroupCollection.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF && !WP8

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

        /// <summary>Gets the items path (default: TopItems).</summary>
        public string ItemsPath { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="ObservableGroupCollection{TItem}"/> class.</summary>
        public ObservableGroupCollection() : this("TopItems")
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableGroupCollection{TItem}"/> class.</summary>
        public ObservableGroupCollection(string itemsPath = "TopItems")
        {
            ItemsPath = itemsPath;
        }

        /// <summary>Creates and adds a group to the collection. </summary>
        /// <param name="title">The title of the group. </param>
        /// <param name="topItemsCount">The top items count. </param>
        /// <returns>The created <see cref="TopItemsGroup{T}"/></returns>
        public TopItemsGroup<TItem> AddGroup(string title, int topItemsCount = -1)
        {
            return AddGroup(title, new List<TItem>(), topItemsCount);
        }

        /// <summary>Creates and adds a group to the collection. </summary>
        /// <param name="title">The title of the group. </param>
        /// <param name="items">The items in the group. </param>
        /// <param name="topItemsCount">The top items count. </param>
        /// <returns>The created <see cref="TopItemsGroup{T}"/></returns>
        public TopItemsGroup<TItem> AddGroup(string title, IEnumerable<TItem> items, int topItemsCount = -1)
        {
            var group = new TopItemsGroup<TItem>(title, items, topItemsCount);
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
                    _view.ItemsPath = new PropertyPath(ItemsPath);
                    _view.Source = this; 
                }
                return _view; 
            }
        }
    }
}

#endif