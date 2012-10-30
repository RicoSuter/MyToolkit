using System.ComponentModel;
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.UI
{
	internal class MyDependencyObject : DependencyObject { }

	public static class Designer
	{
#if WINRT
		public static bool IsInDesignMode
		{
			get { return Windows.ApplicationModel.DesignMode.DesignModeEnabled; }
		}
#elif SILVERLIGHT
		public static bool IsInDesignMode
		{
			get { return DesignerProperties.IsInDesignTool; }
		}
#else
		private static bool? isInDesignMode = null; 
		public static bool IsInDesignMode
		{
			get 
			{ 
				if (!isInDesignMode.HasValue)
					isInDesignMode = DesignerProperties.GetIsInDesignMode(new MyDependencyObject());
				return isInDesignMode.Value;
			}
		}
#endif
	}
}
