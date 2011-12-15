using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MyToolkit.Collections
{
	public class AlphaGroups<T> : List<Group<T>>, INotifyCollectionChanged
	{
		private const string Characters = "#abcdefghijklmnopqrstuvwxyz";

		public AlphaGroups()
		{
			foreach (var alpha in Characters)
				Add(new Group<T>(alpha.ToString()));
		}

		public void Initialize(IEnumerable<T> items)
		{
			foreach (var group in this)
				group.Clear();

			var list = items.OrderBy(i => i.ToString());
			foreach (var item in list)
				AddEx(item, false);

			foreach (var group in this)
				RaiseGroupChanged(group);
		}

		public void AddRange(IEnumerable<T> items)
		{
			AddRangeEx(items, true);
		}

		public void AddRangeEx(IEnumerable<T> items, bool searchPosition)
		{
			var changedGroups = new List<Group<T>>();

			foreach (var item in items)
			{
				var group = AddEx(item, searchPosition);
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
			var firstCharacter = GetFirstCharacter(name);
			var group = this.SingleOrDefault(g => g.Title == firstCharacter);
			if (group == null)
				group = this.First();

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
				}
			}
			return firstCharacter;
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}