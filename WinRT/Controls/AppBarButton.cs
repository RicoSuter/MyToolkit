using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public class AppBarButton : Button
	{
		public AppBarButton()
		{
			DefaultStyleKey = typeof(AppBarButton);
		}

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(String), typeof(AppBarButton), new PropertyMetadata(default(String)));

		public String Header
		{
			get { return (String) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
	}
}
