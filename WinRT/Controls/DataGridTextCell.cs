using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	public class DefaultDataGridCell : DataGridCell
	{
		public DefaultDataGridCell(FrameworkElement control)
			: base(control) { }

		public override void OnSelectedChanged(bool isSelected)
		{
			
		}
	}

	public abstract class DataGridCell
	{
		public FrameworkElement Control { get; private set; }

		public abstract void OnSelectedChanged(bool isSelected);

		public DataGridCell(FrameworkElement control)
		{
			Control = control; 
		}
	}
}
