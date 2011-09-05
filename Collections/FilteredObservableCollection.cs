using System;
using System.Collections.Generic;
using System.Linq; 
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyToolkit.Collections
{
	public class FilteredObservableCollection<T> : ObservableCollection<T>
	{
		private readonly ObservableCollection<T> originalCollection;
		private readonly Func<T, bool> where;

		public FilteredObservableCollection(ObservableCollection<T> originalCollection, Func<T, bool> where)
		{
			this.originalCollection = originalCollection;
			this.where = where;

			originalCollection.CollectionChanged += delegate { UpdateList(); };
			UpdateList();
		}

		private void UpdateList()
		{
			var list = originalCollection.Where(where).ToList();
			
			foreach (var item in this.ToArray()) // remove items
			{
				if (!list.Contains(item))
					Remove(item);
			}

			var prev = -1; 
			foreach (var item in list) // insert new items
			{
				if (!Contains(item))
				{
					if (prev == -1)
						Insert(0, item);
					else
					{
						var prevItem = list[prev];
						Insert(IndexOf(prevItem) + 1, item);
					}
				}
				prev++; 
			}
		}
	}
}
