using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public abstract class DataGridColumn : DependencyObject
	{
		internal ColumnDefinition CreateGridColumnDefinition()
		{
			return new ColumnDefinition { Width = Width };
		}

        /// <summary>
        /// Generates the cell for the given item. 
        /// </summary>
        /// <param name="dataItem">The item to generate the cell for. </param>
        /// <returns>The <see cref="DataGridCell"/>. </returns>
        public abstract DataGridCell GenerateElement(object dataItem);

        /// <summary>
        /// Gets the property path which is used for sorting. 
        /// </summary>
        public abstract PropertyPath OrderPropertyPath { get; }

        public static readonly DependencyProperty CanSortProperty =
            DependencyProperty.Register("CanSort", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));
        
        /// <summary>
        /// Gets or sets a value indicating whether the column can be sorted. 
        /// </summary>
        public bool CanSort
		{
			get { return (bool)GetValue(CanSortProperty); }
			set { SetValue(CanSortProperty, value); }
		}

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Gets a value indicating whether the the column is selected and used for sorting. 
        /// This property should not be set directly, use the SelectColumn method on <see cref="DataGrid"/>. 
        /// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			internal set { SetValue(IsSelectedProperty, value); }
		}

        public static readonly DependencyProperty IsAscendingProperty =
            DependencyProperty.Register("IsAscending", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(default(bool)));
        
        /// <summary>
        /// Gets a value indicating whether the column is sorted ascending (otherwise descending). 
        /// This property should not be set directly, use the SelectColumn method on <see cref="DataGrid"/>. 
        /// </summary>
        public bool IsAscending
		{
			get { return (bool)GetValue(IsAscendingProperty); }
			internal set { SetValue(IsAscendingProperty, value); }
		}

        public static readonly DependencyProperty IsAscendingDefaultProperty =
            DependencyProperty.Register("IsAscendingDefault", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));
        
        /// <summary>
        /// Gets or sets a value indicating whether ascending sorting is default (first click on the column will sort it ascending, otherwise descending). 
        /// </summary>
        public bool IsAscendingDefault
		{
			get { return (bool)GetValue(IsAscendingDefaultProperty); }
			set { SetValue(IsAscendingDefaultProperty, value); }
		}

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(DataGridColumn), new PropertyMetadata(default(string)));
        
        /// <summary>
        /// Gets or sets the header text. 
        /// </summary>
        public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(GridLength), typeof(DataGridColumn), new PropertyMetadata(default(GridLength)));

        /// <summary>
        /// Gets or sets the width of the column. 
        /// </summary>
        public GridLength Width
		{
            get { return (GridLength)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
	}
}
