using System.Windows;
using Microsoft.Phone.Controls;

namespace MyToolkit.Paging
{
	public static class PhonePage
	{
		public static PhoneApplicationPage CurrentPage
		{
			get { return (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content; }
		}

		public static BindableApplicationBar GetApplicationBar(PhoneApplicationPage obj)
		{
			return (BindableApplicationBar)obj.GetValue(ApplicationBarProperty);
		}

		public static void SetApplicationBar(PhoneApplicationPage obj, BindableApplicationBar value)
		{
			obj.SetValue(ApplicationBarProperty, value);
		}

		private static readonly DependencyProperty ApplicationBarProperty =
			DependencyProperty.RegisterAttached("ApplicationBar", typeof(BindableApplicationBar), typeof(PhoneApplicationPage), 
			new PropertyMetadata(null, ApplicationBarPropertyChanged));

		private static void ApplicationBarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var page = (PhoneApplicationPage)obj;
			if (args.NewValue != null)
				page.ApplicationBar = ((BindableApplicationBar)args.NewValue).InternalApplicationBar;
		}
	}
}
