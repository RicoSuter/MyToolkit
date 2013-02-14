using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace MyToolkit.Controls
{
	public class NativeLongListSelector : LongListSelector
	{
		public NativeLongListSelector()
		{
			var dict = new ResourceDictionary();
			dict.Source = new Uri("/MyToolkit.Extended;component/Themes/generic.xaml", UriKind.Relative);

#if !WP8
			GroupItemsPanel = (ItemsPanelTemplate)dict["LongListSelectorDefaultGroupItemsPanel"];
			GroupItemTemplate = (DataTemplate)dict["LongListSelectorDefaultGroupItemTemplate"];
#else
			HideEmptyGroups = true;
			JumpListStyle = (Style)dict["LongListSelectorJumpListStyle"]; 
#endif
			GroupHeaderTemplate = (DataTemplate)dict["LongListSelectorDefaultGroupHeaderTemplate"];
		}

#if WP8
		// dummy properties
		public bool IsFlatList
		{
			get { return !IsGroupingEnabled; }
			set { IsGroupingEnabled = !value; }
		}

		public event EventHandler ScrollingStarted
		{
			add { } 
			remove { }
		}

		public event EventHandler ScrollingCompleted
		{
			add { }
			remove { }
		}
#endif
	}
}
