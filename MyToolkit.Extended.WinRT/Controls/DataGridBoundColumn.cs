//-----------------------------------------------------------------------
// <copyright file="DataGridBoundColumn.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Controls
{
	public abstract class DataGridBoundColumn : DataGridColumnBase
	{
		private Binding _binding;

        /// <summary>Gets or sets the data binding for this column. </summary>
		public Binding Binding
		{
			get { return _binding; }
			set { _binding = value; }
		}

        /// <summary>Gets the property path which is used for sorting. </summary>
		public override PropertyPath OrderPropertyPath
		{
			get { return _binding.Path; }
		}
	}
}