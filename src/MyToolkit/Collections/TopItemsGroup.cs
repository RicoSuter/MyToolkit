//-----------------------------------------------------------------------
// <copyright file="TopItemsGroup.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyToolkit.Collections
{
    /// <summary>An extended group implementation with a list of items but showing only a given number of them. </summary>
    /// <typeparam name="T">The type of an item. </typeparam>
    public class TopItemsGroup<T> : Group<T>
    {
        private int _topItemsCount = -1;
        private bool _updateLock = false;

        /// <summary>Initializes a new instance of the <see cref="TopItemsGroup{T}"/> class. </summary>
        public TopItemsGroup(string title, int topItemsCount = -1)
            : base(title)
        {
            _topItemsCount = topItemsCount;
            UpdateTopItems();
        }

        /// <summary>Initializes a new instance of the <see cref="TopItemsGroup{T}"/> class. </summary>
        public TopItemsGroup(string title, IEnumerable<T> items, int topItemsCount = -1)
            : base(title, items)
        {
            _topItemsCount = topItemsCount;
            UpdateTopItems();
        }

        /// <summary>Gets or sets the number of items to show in the group. </summary>
        public int TopItemsCount
        {
            get { return _topItemsCount; }
            set
            {
                if (_topItemsCount != value)
                {
                    _topItemsCount = value;
                    UpdateTopItems();
                    OnPropertyChanged(new PropertyChangedEventArgs("TopItemsCount"));
                }
            }
        }

        /// <summary>Gets the items to show in the group. </summary>
        public MtObservableCollection<T> TopItems { get; private set; }

        /// <summary>Initializes the group with a list of items. </summary>
        /// <param name="items">The items. </param>
        public void Initialize(IEnumerable<T> items)
        {
            _updateLock = true;

            Clear();
            foreach (var item in items)
                Add(item);

            TopItems.Initialize(TopItemsCount == -1 ? this : this.Take(TopItemsCount));
            _updateLock = false;
        }

        /// <summary>Called when the items collection changed. </summary>
        /// <param name="e">The arguments. </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            UpdateTopItems();
        }

        private void UpdateTopItems()
        {
            if (_updateLock)
                return;

            var collection = TopItemsCount == -1 ? this : this.Take(TopItemsCount);
            if (TopItems == null)
                TopItems = new MtObservableCollection<T>(collection);
            else
                TopItems.Initialize(collection);
        }
    }

    [Obsolete("Use TopItemsGroup<T> instead. 11/29/2014")]
    public class ExtendedGroup<T> : TopItemsGroup<T>
    {
        /// <summary>Initializes a new instance of the <see cref="TopItemsGroup{T}"/> class. </summary>
        public ExtendedGroup(string title, int topItemsCount = -1)
            : base(title, topItemsCount)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TopItemsGroup{T}"/> class. </summary>
        public ExtendedGroup(string title, IEnumerable<T> items, int topItemsCount = -1)
            : base(title, items, topItemsCount)
        {
        }
    }
}