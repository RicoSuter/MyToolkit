using Windows.UI.Xaml;
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
}