using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Data;
using Microsoft.Phone.Shell;

namespace MyToolkit.Paging
{
    [ContentProperty("Buttons")]
    public class BindableApplicationBar : DependencyObject, IApplicationBar
    {
		private readonly BindableApplicationBarIconButtonCollection buttons;
		private readonly BindableApplicationBarMenuItemCollection menuItems;
		private readonly IApplicationBar internalApplicationBar;

        public BindableApplicationBar()
        {
			internalApplicationBar = new ApplicationBar();
			
			buttons = new BindableApplicationBarIconButtonCollection(internalApplicationBar);
			menuItems = new BindableApplicationBarMenuItemCollection(internalApplicationBar);

			buttons.CollectionChanged += OnCollectionChanged;
			menuItems.CollectionChanged += OnCollectionChanged;

            BindingOperations.SetBinding(this, DataContextProperty, new Binding());
        }

    	private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    	{
    		UpdateDataContext();
    	}

		private void UpdateDataContext()
		{
			buttons.UpdateDataContext(DataContext);
			menuItems.UpdateDataContext(DataContext);
		}

    	public double DefaultSize
		{
			get { return internalApplicationBar.DefaultSize; }
		}

		public double MiniSize
		{
			get { return internalApplicationBar.MiniSize; }
		}

		IList IApplicationBar.Buttons
		{
			get { return buttons; }
		}

		IList IApplicationBar.MenuItems
		{
			get { return menuItems; }
		}

		public BindableApplicationBarIconButtonCollection Buttons
		{
			get { return buttons; }
		}

		public BindableApplicationBarMenuItemCollection MenuItems
		{
			get { return menuItems; }
		}

		internal IApplicationBar InternalApplicationBar
		{
			get { return internalApplicationBar; }
		}




		public event EventHandler<ApplicationBarStateChangedEventArgs> StateChanged
		{
			add { internalApplicationBar.StateChanged += value; }
			remove { internalApplicationBar.StateChanged -= value; }
		}






        public object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register("DataContext", typeof(object),
			typeof(BindableApplicationBar), new PropertyMetadata(default(object), OnDataContextChanged));

    	private static void OnDataContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    	{
    		var bar = (BindableApplicationBar) obj;
			bar.UpdateDataContext();
    	}

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(
                "BackgroundColor",
                typeof(Color),
                typeof(BindableApplicationBar),
                new PropertyMetadata(default(Color), (d, e) => ((BindableApplicationBar)d).BackgroundColorChanged((Color)e.NewValue)));

        private void BackgroundColorChanged(Color newColor)
        {
            internalApplicationBar.BackgroundColor = newColor;
        } 

		public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(
                "ForegroundColor",
                typeof(Color),
                typeof(BindableApplicationBar),
                new PropertyMetadata(default(Color), (d, e) => ((BindableApplicationBar)d).ForegroundColorChanged((Color)e.NewValue)));

        private void ForegroundColorChanged(Color newColor)
        {
            internalApplicationBar.ForegroundColor = newColor;
        } 

		public bool IsMenuEnabled
        {
            get { return (bool)GetValue(IsMenuEnabledProperty); }
            set { SetValue(IsMenuEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsMenuEnabledProperty =
            DependencyProperty.Register("IsMenuEnabled", typeof(bool), typeof(BindableApplicationBar),
                new PropertyMetadata(default(bool), (d, e) => ((BindableApplicationBar)d).IsMenuEnabledChanged((bool)e.NewValue)));

        private void IsMenuEnabledChanged(bool isMenuEnabled)
        {
            internalApplicationBar.IsMenuEnabled = isMenuEnabled;
        }

		public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

		public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(BindableApplicationBar),
                new PropertyMetadata(default(bool), (d, e) => ((BindableApplicationBar)d).IsVisibleChanged((bool)e.NewValue)));

        private void IsVisibleChanged(bool isVisible)
        {
            internalApplicationBar.IsVisible = isVisible;
        }

		public ApplicationBarMode Mode
        {
            get { return (ApplicationBarMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

		public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(ApplicationBarMode), typeof(BindableApplicationBar),
                new PropertyMetadata(default(ApplicationBarMode), (d, e) => ((BindableApplicationBar)d).ModeChanged((ApplicationBarMode)e.NewValue)));

        private void ModeChanged(ApplicationBarMode mode)
        {
            internalApplicationBar.Mode = mode;
        }

		public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

		public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(BindableApplicationBar),
                new PropertyMetadata(default(double), (d, e) => ((BindableApplicationBar)d).OpacityChanged((double)e.NewValue)));

        private void OpacityChanged(double opacity)
        {
            internalApplicationBar.Opacity = opacity;
        }
	}
}
