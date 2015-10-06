//-----------------------------------------------------------------------
// <copyright file="QueryObservableCollectionView.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyToolkit.Collections
{
    /// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with a query. </summary>
    /// <typeparam name="TItem">The type of an item. </typeparam>
    public class QueryObservableCollectionView<TItem> : ObservableCollectionViewBase<TItem>
    {
        private Func<IList<TItem>, IEnumerable<TItem>> _query;

        /// <summary>Initializes a new instance of the <see cref="QueryObservableCollectionView{TItem}"/> class. </summary>
        public QueryObservableCollectionView()
            : base(new ObservableCollection<TItem>(), false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="QueryObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        public QueryObservableCollectionView(IList<TItem> items)
            : base(items, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="QueryObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="query">The initial query. </param>
        public QueryObservableCollectionView(IList<TItem> items, Func<IList<TItem>, IEnumerable<TItem>> query)
            : base(items, false)
        {
            Query = query;
        }

        /// <summary>Gets or sets the query. </summary>
        public Func<IList<TItem>, IEnumerable<TItem>> Query
        {
            get { return _query; }
            set
            {
                if (_query != value)
                {
                    _query = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets the list of items with the current order and filter.</summary>
        /// <returns>The items. </returns>
        protected override IList<TItem> GetItems()
        {
            if (Query == null)
                return Items;
            return Query(Items).ToList();
        }
    }
}