using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace MyToolkit.Paging
{
	[ContentProperty(Name = "Content")]
	public class Page : Control
	{
		public Frame Frame { get; internal set; }

		public Page()
		{
			DefaultStyleKey = typeof(Page);
		}

		public Windows.UI.Xaml.Controls.Page InternalPage { get; private set; }

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			InternalPage = (Windows.UI.Xaml.Controls.Page)GetTemplateChild("page");
		}

		public static readonly DependencyProperty TopAppBarProperty =
			DependencyProperty.Register("TopAppBar", typeof(AppBar), typeof(Page), new PropertyMetadata(default(AppBar)));

		public AppBar TopAppBar
		{
			get { return (AppBar)GetValue(TopAppBarProperty); }
			set { SetValue(TopAppBarProperty, value); }
		}

		public static readonly DependencyProperty BottomAppBarProperty =
			DependencyProperty.Register("BottomAppBar", typeof(AppBar), typeof(Page), new PropertyMetadata(default(AppBar)));

		public AppBar BottomAppBar
		{
			get { return (AppBar)GetValue(BottomAppBarProperty); }
			set { SetValue(BottomAppBarProperty, value); }
		}
		
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof (FrameworkElement), typeof (Page), new PropertyMetadata(default(FrameworkElement)));

		public FrameworkElement Content
		{
			get { return (FrameworkElement) GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		
		public virtual void OnNavigatedTo(NavigationEventArgs e)
		{
			
		}

		public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{

		}

		public virtual Task OnNavigatingFromAsync(NavigatingCancelEventArgs e)
		{
			return null; 
		}

		public virtual void OnNavigatedFrom(NavigationEventArgs e)
		{
			
		}

		public virtual void OnVisibilityChanged(VisibilityChangedEventArgs args)
		{
			
		}

		// internal methods ensure that base implementations of InternalOn* is always called
		// even if user does not call base.InternalOn* in the overridden On* method. 

		internal protected virtual void InternalOnNavigatedTo(NavigationEventArgs e)
		{
			OnNavigatedTo(e);
		}

		internal protected virtual async Task InternalOnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			OnNavigatingFrom(e);

			var task = OnNavigatingFromAsync(e);
			if (task != null)
				await task;
		}

		internal protected virtual void InternalOnNavigatedFrom(NavigationEventArgs e)
		{
			OnNavigatedFrom(e);
		}

		#region Predefined event handlers

		protected void GoHome(object sender, RoutedEventArgs e)
		{
			if (Frame != null)
				Frame.GoHomeAsync();
		}

		protected void GoBack(object sender, RoutedEventArgs e)
		{
			if (Frame != null && Frame.CanGoBack)
				Frame.GoBackAsync();
		}

		protected void GoForward(object sender, RoutedEventArgs e)
		{
			if (Frame != null && Frame.CanGoForward)
				Frame.GoForwardAsync();
		}

		#endregion
	}
}