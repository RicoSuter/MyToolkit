using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MyToolkit.Collections
{
	public class AlphaGroups<T> : List<Group<T>>, INotifyCollectionChanged
	{
		private const string Characters = "#abcdefghijklmnopqrstuvwxyz";
		private Dictionary<string, Group<T>> groups; // used for faster group access

		public AlphaGroups()
		{
			Initialize(new List<T>());
		}

		public void Initialize(IEnumerable<T> items)
		{
			Clear();

			groups = new Dictionary<string, Group<T>>();

			var itemGroups = items.OrderBy(i => i.ToString()).
				GroupBy(i => GetFirstCharacter(i.ToString())).
				ToDictionary(g => g.Key);

			foreach (var alpha in Characters)
			{
				var title = alpha.ToString();
				var group = itemGroups.ContainsKey(title) ? 
					new Group<T>(title, itemGroups[title]) : new Group<T>(title);
				groups.Add(title, group);
			}

			foreach (var g in groups)
				Add(g.Value);

			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void AddRange(IEnumerable<T> items)
		{
			var changedGroups = new List<Group<T>>();

			foreach (var item in items)
			{
				var group = AddEx(item, true);
				if (!changedGroups.Contains(group))
					changedGroups.Add(group);
			}

			foreach (var group in changedGroups)
				RaiseGroupChanged(group);
		}

		private void RaiseGroupChanged(Group<T> group)
		{
			var index = IndexOf(group);
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, group, group, index));
		}

		public void Add(T item)
		{
			var group = AddEx(item, true);
			RaiseGroupChanged(group);
		}

		private Group<T> AddEx(T item, bool searchPosition)
		{
			var name = item.ToString();
			var group = groups[GetFirstCharacter(name)];

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

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}