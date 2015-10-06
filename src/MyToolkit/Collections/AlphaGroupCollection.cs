//-----------------------------------------------------------------------
// <copyright file="AlphaGroups.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MyToolkit.Collections
{
    /// <summary>Groups a set of objects alphabetically (using the ToString() method). </summary>
    /// <typeparam name="T">The item type. </typeparam>
    public class AlphaGroupCollection<T> : List<Group<T>>, INotifyCollectionChanged
    {
        private const string Characters = "#abcdefghijklmnopqrstuvwxyz";
        private Dictionary<string, Group<T>> _groups; // used for faster group access

        /// <summary>Initializes a new instance of the <see cref="AlphaGroupCollection{T}"/> class. </summary>
        public AlphaGroupCollection()
        {
            Initialize(new List<T>());
        }

        /// <summary>Occurs when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>Initializes the group with a list of items. </summary>
        /// <param name="items">The items. </param>
        public void Initialize(IEnumerable<T> items)
        {
            Clear();

            _groups = new Dictionary<string, Group<T>>();

            var itemGroups = items
                .OrderBy(i => i.ToString()).
                GroupBy(i => GetFirstCharacter(i.ToString())).
                ToDictionary(g => g.Key);

            foreach (var alpha in Characters)
            {
                var title = alpha.ToString();

                var group = itemGroups.ContainsKey(title)
                    ? new Group<T>(title, itemGroups[title])
                    : new Group<T>(title);

                _groups.Add(title, group);
            }

            foreach (var g in _groups)
                Add(g.Value);

            var handler = CollectionChanged;
            if (handler != null)
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>Adds multiple items to the group. </summary>
        /// <param name="items">The items. </param>
        public void AddRange(IEnumerable<T> items)
        {
            var changedGroups = new List<Group<T>>();

            foreach (var item in items)
            {
                var group = AddItemToCorrectGroup(item, true);
                if (!changedGroups.Contains(group))
                    changedGroups.Add(group);
            }

            foreach (var group in changedGroups)
                RaiseGroupChanged(group);
        }

        /// <summary>Adds an item to the group. </summary>
        /// <param name="item">The item. </param>
        public void Add(T item)
        {
            var group = AddItemToCorrectGroup(item, true);
            RaiseGroupChanged(group);
        }

        private void RaiseGroupChanged(Group<T> group)
        {
            var index = IndexOf(group);
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, group, group, index));
        }

        private Group<T> AddItemToCorrectGroup(T item, bool searchPosition)
        {
            var name = item.ToString();
            var group = _groups[GetFirstCharacter(name)];

            if (searchPosition && group.Count > 0)
            {
                var newTitle = item.ToString();
                var newIndex = 0;
                foreach (var i in group)
                {
                    if (i.ToString().CompareTo(newTitle) > 0)
                        break;
                    newIndex++;
                }
                group.Insert(newIndex, item);
            }
            else
                group.Add(item);
            return group;
        }

        private static string GetFirstCharacter(string name)
        {
            var firstCharacter = name.Length > 0 ? name.Substring(0, 1).ToLower() : "#";
            if (!Characters.Contains(firstCharacter))
            {
                switch (firstCharacter)
                {
                    case "é": return "e";
                    case "è": return "e";
                    case "ê": return "e";
                    case "à": return "a";
                    case "â": return "a";
                    case "ü": return "u";
                    case "ä": return "a";
                    case "ö": return "o";
                    case "î": return "i";
                    default: return "#";
                }
            }
            return firstCharacter;
        }
    }
}