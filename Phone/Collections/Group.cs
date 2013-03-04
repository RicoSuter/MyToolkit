using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

#if WINRT
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace MyToolkit.Collections
{
	public interface IGroup
	{
		string Title { get; }
	}

	public class Group<T> : ObservableCollection<T>, IGroup
	{
		public Group(string title) : this(title, new List<T>()) { }
		public Group(string title, IEnumerable<T> items)
			: base(items)
		{
			Title = title;
		}

		private string title;
		public string Title
		{
			get { return title; }
			set
			{
				if (value != title)
				{
					title = value;
					OnPropertyChanged(new PropertyChangedEventArgs("Title"));
				}
			}
		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.PropertyName == "Count")
			{
				OnPropertyChanged(new PropertyChangedEventArgs("HasItems"));
#if !WINRT
				OnPropertyChanged(new PropertyChangedEventArgs("GroupBackgroundBrush"));
#endif
			}
		}

		public bool HasItems
		{
			get { return Items.Count > 0; }
		}

#if !WINRT
		public Brush GroupBackgroundBrush
		{
			get 
			{
				return HasItems
					? (SolidColorBrush) Application.Current.Resources["PhoneAccentBrush"]
					: (SolidColorBrush) Application.Current.Resources["PhoneChromeBrush"];
			}
		}
#endif
	}
}
