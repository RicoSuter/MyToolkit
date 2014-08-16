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
#elif SL4 || SL5
		public static bool IsInDesignMode
		{
			get { return DesignerProperties.IsInDesignTool; }
		}
#else
		private static bool? _isInDesignMode = null; 
		public static bool IsInDesignMode
		{
			get 
			{ 
				if (!_isInDesignMode.HasValue)
					_isInDesignMode = DesignerProperties.GetIsInDesignMode(new MyDependencyObject());
				return _isInDesignMode.Value;
			}
		}
#endif
	}
}
