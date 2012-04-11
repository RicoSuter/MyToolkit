using System.ComponentModel;
#if !METRO
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace MyToolkit.UI
{
	internal class MyDependencyObject : DependencyObject { }

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
#if METRO
				return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#else
				if (!isInDesignMode.HasValue)
					isInDesignMode = DesignerProperties.GetIsInDesignMode(new MyDependencyObject());
				return isInDesignMode.Value;
#endif
#else
				return DesignerProperties.IsInDesignTool;
#endif
			}
		}
	}
}
