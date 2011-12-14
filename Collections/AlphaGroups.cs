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

		public void Add(T item)
		{
			var name = item.ToString();
			var firstCharacter = GetFirstCharacter(name);
			var group = this.SingleOrDefault(g => g.Title == firstCharacter);
			if (group == null)
				group = this.First();

			// TODO: optimize
			var list = group.ToList();
			list.Add(item);
			list = list.OrderBy(i => i.ToString()).ToList();
			var newIndex = list.IndexOf(item);

			group.Insert(newIndex, item);

			var index = IndexOf(group);
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, group, group, index));
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