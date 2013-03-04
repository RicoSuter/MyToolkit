using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MyToolkit.Collections
{
	public class ExtendedGroup<T> : Group<T>
	{
		private int topItemsCount = -1;
		public int TopItemsCount
		{
			get { return topItemsCount; }
			set
			{
				if (topItemsCount != value)
				{
					topItemsCount = value;
					UpdateTopItems();
					RaisePropertyChanged();
				}
			}
		}

		public ExtendedGroup(string title, int topItemsCount = -1)
			: base(title)
		{
			this.topItemsCount = topItemsCount;
			UpdateTopItems();
		}

		public ExtendedGroup(string title, IEnumerable<T> items, int topItemsCount = -1)
			: base(title, items)
		{
			this.topItemsCount = topItemsCount;
			UpdateTopItems();
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			UpdateTopItems();
		}

		private void UpdateTopItems()
		{
			if (updateLock)
				return;

			var collection = TopItemsCount == -1 ? this : this.Take(TopItemsCount);
			if (TopItems == null)
				TopItems = new ExtendedObservableCollection<T>(collection);
			else
				TopItems.Initialize(collection);
		}

		private bool updateLock = false; 
		public void Initialize(IEnumerable<T> collection)
		{
			updateLock = true;

			Clear();
			foreach (var item in collection)
				Add(item);

			TopItems.Initialize(TopItemsCount == -1 ? this : this.Take(TopItemsCount));
			updateLock = false; 
		}

		public ExtendedObservableCollection<T> TopItems { get; private set; }

		protected override event PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			var copy = PropertyChanged;
			if (copy != null)
				copy(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}