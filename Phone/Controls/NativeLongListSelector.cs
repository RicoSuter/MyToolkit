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
#endif
			GroupHeaderTemplate = (DataTemplate)dict["LongListSelectorDefaultGroupHeaderTemplate"];
		}
	}
}
