using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace MyToolkit.Controls
{
    /// <summary>
    /// Native looking/correctly styled <see cref="LongListSelector"/> which correctly works on WP7 and WP8. 
    /// </summary>
	public class NativeLongListSelector : LongListSelector
	{
		public NativeLongListSelector()
		{
			var dict = new ResourceDictionary();

#if WP8
			dict.Source = new Uri("/MyToolkit.Extended;component/Themes/Generic.WP8.xaml", UriKind.Relative);
			HideEmptyGroups = true;
			JumpListStyle = (Style)dict["LongListSelectorJumpListStyle"]; 
#else
			dict.Source = new Uri("/MyToolkit.Extended;component/Themes/Generic.WP7.xaml", UriKind.Relative);
			GroupItemsPanel = (ItemsPanelTemplate)dict["LongListSelectorDefaultGroupItemsPanel"];
			GroupItemTemplate = (DataTemplate)dict["LongListSelectorDefaultGroupItemTemplate"];
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
