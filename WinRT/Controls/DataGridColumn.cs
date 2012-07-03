using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Controls
{
	public abstract class DataGridBoundColumn : DataGridColumn
	{
		private Binding binding;
		public Binding Binding
		{
			get { return binding; }
			set { binding = value; }
		}

		public override PropertyPath OrderPropertyPath
		{
			get { return binding.Path; }
		}
	}

	public abstract class DataGridColumn : DependencyObject
	{
		public abstract DataGridCell GenerateElement(object dataItem);
		public abstract PropertyPath OrderPropertyPath { get; }

		internal ColumnDefinition CreateGridColumnDefinition()
		{
			return new ColumnDefinition { Width = Width };
		}

		public bool CanSort
		{
			get { return (bool)GetValue(CanSortProperty); }
			set { SetValue(CanSortProperty, value); }
		}
		public static readonly DependencyProperty CanSortProperty =
			DependencyProperty.Register("CanSort", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));


		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			internal set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(default(bool)));


		public bool IsAscending
		{
			get { return (bool)GetValue(IsAscendingProperty); }
			internal set { SetValue(IsAscendingProperty, value); }
		}
		public static readonly DependencyProperty IsAscendingProperty =
			DependencyProperty.Register("IsAscending", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(default(bool)));

		public bool IsAscendingDefault
		{
			get { return (bool)GetValue(IsAscendingDefaultProperty); }
			set { SetValue(IsAscendingDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsAscendingDefaultProperty =
			DependencyProperty.Register("IsAscendingDefault", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));


		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(string), typeof(DataGridColumn), new PropertyMetadata(default(string)));


        public GridLength Width
		{
            get { return (GridLength)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(GridLength), typeof(DataGridColumn), new PropertyMetadata(default(GridLength)));
	}
}
