using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace MyToolkit.Paging
{
	[ContentProperty(Name = "Content")]
	public class Page : Control
	{
		public Frame Frame { get; internal set; }
		public bool IsPreparingNavigatingFrom { get; internal set; }

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

		public virtual void OnNavigatingFrom(NavigationEventArgs e)
		{

		}

		public virtual void OnNavigatedFrom(NavigationEventArgs e)
		{
			
		}

		public virtual bool OnPrepareNavigatingFrom(Action continuationAction)
		{
			return false; 
		}
	}
}