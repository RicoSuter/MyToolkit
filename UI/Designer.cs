using System.ComponentModel;
using System.Windows;

namespace MyToolkit.UI
{
	internal class MyDependencyObject : DependencyObject { }
	public static class Designer
	{
		public static bool IsDesignMode
		{
			get
			{
				return DesignerProperties.GetIsInDesignMode(new MyDependencyObject());
			}
		}
	}
}
