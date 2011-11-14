using System.ComponentModel;
using System.Windows;

namespace MyToolkit.UI
{
#if !SILVERLIGHT
	internal class MyDependencyObject : DependencyObject { }
#endif

	public static class Designer
	{
#if !SILVERLIGHT
		private static bool? isInDesignMode = null; 
#endif

		public static bool IsInDesignMode
		{
			get
			{
#if !SILVERLIGHT
				if (!isInDesignMode.HasValue)
					isInDesignMode = DesignerProperties.GetIsInDesignMode(new MyDependencyObject());
				return isInDesignMode.Value;
#else
				return DesignerProperties.IsInDesignTool;
#endif
			}
		}
	}
}
