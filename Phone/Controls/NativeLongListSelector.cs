using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace MyToolkit.Controls
{
	public class NativeLongListSelector : LongListSelector
	{
		public NativeLongListSelector()
		{
			var dict = new ResourceDictionary();
			dict.Source = new Uri("/MyToolkit;component/Themes/generic.xaml", UriKind.Relative);

			GroupItemsPanel = (ItemsPanelTemplate)dict["LongListSelectorDefaultGroupItemsPanel"];
			GroupItemTemplate = (DataTemplate)dict["LongListSelectorDefaultGroupItemTemplate"];
			GroupHeaderTemplate = (DataTemplate)dict["LongListSelectorDefaultGroupHeaderTemplate"];
		}
	}
}
