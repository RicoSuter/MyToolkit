using System.ComponentModel;
using System.Windows;

namespace MyToolkit.UI
{
#if !SILVERLIGHT
	internal class MyDependencyObject : DependencyObject { }
#endif
	public static class Designer
	{
		public static bool IsDesignMode
		{
			get
			{
#if !SILVERLIGHT
				return DesignerProperties.GetIsInDesignMode(new MyDependencyObject());
#else
				return DesignerProperties.IsInDesignTool;
#endif
			}
		}
	}
}
