//-----------------------------------------------------------------------
// <copyright file="ObservableCollectionView.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MyToolkit.Collections
{
    /// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with automatic sorting and filtering. </summary>
    /// <typeparam name="TItem">The type of an item. </typeparam>
    public class ObservableCollectionView<TItem> : ObservableCollectionViewBase<TItem>, IObservableCollectionView
    {
        private Func<TItem, bool> _filter;
        private Func<TItem, object> _order;

        private int _offset;
        private int _limit;
        private bool _ascending = true;

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        public ObservableCollectionView()
            : this(new ObservableCollection<TItem>())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        public ObservableCollectionView(IList<TItem> items)
            : this(items, null, null, true, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="filter">The filter of the view. </param>
        public ObservableCollectionView(IList<TItem> items, Func<TItem, bool> filter)
            : this(items, filter, null, true, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="filter">The filter of the view. </param>
        /// <param name="orderBy">The order key of the view. </param>
        public ObservableCollectionView(IList<TItem> items, Func<TItem, bool> filter, Func<TItem, object> orderBy)
            : this(items, filter, orderBy, true, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="filter">The filter of the view. </param>
        /// <param name="orderBy">The order key of the view. </param>
        /// <param name="ascending">The value indicating whether to sort ascending. </param>
        public ObservableCollectionView(IList<TItem> items, Func<TItem, bool> filter, Func<TItem, object> orderBy, bool ascending)
            : this(items, filter, orderBy, ascending, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="filter">The filter of the view. </param>
        /// <param name="orderBy">The order key of the view. </param>
        /// <param name="ascending">The value indicating whether to sort ascending. </param>
        /// <param name="trackItemChanges">The value indicating whether to track items which implement <see cref="INotifyPropertyChanged"/></param>
        public ObservableCollectionView(IList<TItem> items, Func<TItem, bool> filter, Func<TItem, object> orderBy, bool ascending, bool trackItemChanges)
            : base(items, trackItemChanges)
        {
            Order = orderBy;
            Filter = filter;
            Ascending = ascending;
        }

        /// <summary>Gets or sets the filter. </summary>
        public Func<TItem, bool> Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                Refresh();
            }
        }

        /// <summary>Gets or sets the filter (a Func{TItem, bool} object). </summary>
        object IObservableCollectionView.Filter
        {
            get { return Filter; }
            set { Filter = (Func<TItem, bool>)value; }
        }

        /// <summary>Gets or sets the sorting/order function. </summary>
        public Func<TItem, object> Order
        {
            get { return _order; }
            set
            {
                _order = value;
                Refresh();
            }
        }

        /// <summary>Gets or sets the order. </summary>
        object IObservableCollectionView.Order
        {
            get { return Order; }
            set { Order = (Func<TItem, object>)value; }
        }

        /// <summary>Gets or sets the maximum number of items in the view. </summary>
        public int Limit
        {
            get { return _limit; }
            set
            {
                if (_limit != value)
                {
                    _limit = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets or sets the offset from where the results a selected. </summary>
        public int Offset
        {
            get { return _offset; }
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether the sorting should be ascending; otherwise descending. </summary>
        public bool Ascending
        {
            get { return _ascending; }
            set
            {
                if (_ascending != value)
                {
                    _ascending = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets the list of items with the current order and filter.</summary>
        /// <returns>The items. </returns>
        protected override IList<TItem> GetItems()
        {
            List<TItem> list;

            if (Filter != null && Order != null && Ascending)
                list = Items.Where(Filter).OrderBy(Order).ToList();
            else if (Filter != null && Order != null && !Ascending)
                list = Items.Where(Filter).OrderByDescending(Order).ToList();
            else if (Filter == null && Order != null && Ascending)
                list = Items.OrderBy(Order).ToList();
            else if (Filter == null && Order != null && !Ascending)
                list = Items.OrderByDescending(Order).ToList();
            else if (Filter != null && Order == null)
                list = Items.Where(Filter).ToList();
            else if (Filter == null && Order == null)
                list = Items.ToList();
            else
                throw new Exception();

            if (Limit > 0 || Offset > 0)
                list = list.Skip(Offset).Take(Limit).ToList();

            return list;
        }
    }
}