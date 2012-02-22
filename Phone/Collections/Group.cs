using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace MyToolkit.Collections
{
	public class Group<T> : ObservableCollection<T>
	{
		public Group(string title) : this(title, new List<T>()) { }
		public Group(string title, IEnumerable<T> items)
			: base(items)
		{
			Title = title;
		}

		public string Title { get; private set; }

		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.PropertyName == "Count")
			{
				OnPropertyChanged(new PropertyChangedEventArgs("HasItems"));
				OnPropertyChanged(new PropertyChangedEventArgs("GroupBackgroundBrush"));
			}
		}

		public bool HasItems
		{
			get { return Items.Count > 0; }
		}

		public Brush GroupBackgroundBrush
		{
			get 
			{
				return HasItems
					? (SolidColorBrush) Application.Current.Resources["PhoneAccentBrush"]
					: (SolidColorBrush) Application.Current.Resources["PhoneChromeBrush"];
			}
		}
	}
}
