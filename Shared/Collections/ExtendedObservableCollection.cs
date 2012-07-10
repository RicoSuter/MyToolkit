using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyToolkit.Collections
{
	public class ExtendedObservableCollection<T> : ObservableCollection<T>
	{
		public new event PropertyChangedEventHandler PropertyChanged
		{
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}
		
		public void AddRange(ICollection<T> collection)
		{
			foreach (var i in collection)
				Add(i);
		}
	}
}