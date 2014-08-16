using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MyToolkit.Paging
{
    /// <summary>
    /// Represents an Application Bar in Windows Phone applications with support for bindings. 
    /// </summary>
    [ContentProperty("Buttons")]
    public class BindableApplicationBar : DependencyObject, IApplicationBar
    {
		private readonly IApplicationBar _internalApplicationBar;
		private readonly BindableApplicationBarIconButtonCollection _buttons;
		private readonly BindableApplicationBarMenuItemCollection _menuItems;

		#region Attached property

        /// <summary>
        /// Gets the Application Bar of the page. 
        /// </summary>
        /// <param name="obj">The page. </param>
        /// <returns>The <see cref="BindableApplicationBar"/>. </returns>
		public static BindableApplicationBar GetApplicationBar(PhoneApplicationPage obj)
		{
			return (BindableApplicationBar)obj.GetValue(ApplicationBarProperty);
		}

        /// <summary>
        /// Sets the Application Bar of the page. 
        /// </summary>
        /// <param name="obj">The page. </param>
        /// <param name="value">The <see cref="BindableApplicationBar"/>.</param>
        public static void SetApplicationBar(PhoneApplicationPage obj, BindableApplicationBar value)
		{
			obj.SetValue(ApplicationBarProperty, value);
		}

		public static readonly DependencyProperty ApplicationBarProperty =
			DependencyProperty.RegisterAttached("ApplicationBar", typeof(BindableApplicationBar), typeof(PhoneApplicationPage),
			new PropertyMetadata(null, ApplicationBarPropertyChanged));

		private static void ApplicationBarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var page = (PhoneApplicationPage)obj;
			if (args.NewValue != null)
				page.ApplicationBar = ((BindableApplicationBar)args.NewValue).InternalApplicationBar;
		}

		#endregion

        /// <summary>
        /// Creates a new <see cref="BindableApplicationBar"/> instance. 
        /// </summary>
		public BindableApplicationBar()
        {
			_internalApplicationBar = new ApplicationBar();
			
			_buttons = new BindableApplicationBarIconButtonCollection(this);
			_menuItems = new BindableApplicationBarMenuItemCollection(this);

			_buttons.CollectionChanged += OnCollectionChanged;
			_menuItems.CollectionChanged += OnCollectionChanged;

            BindingOperations.SetBinding(this, DataContextProperty, new Binding());
        }

        #region Properties 

        /// <summary>
        /// Gets the distance that the Application Bar extends into a page when the Microsoft.Phone.Shell.IApplicationBar.Mode
        /// property is set to Microsoft.Phone.Shell.ApplicationBarMode.Default.
        /// </summary>
        public double DefaultSize
		{
			get { return _internalApplicationBar.DefaultSize; }
		}

        /// <summary>
        /// Gets the distance that the Application Bar extends into a page when the Microsoft.Phone.Shell.IApplicationBar.Mode
        /// property is set to Microsoft.Phone.Shell.ApplicationBarMode.Minimized.
        /// </summary>
		public double MiniSize
		{
			get { return _internalApplicationBar.MiniSize; }
		}

        /// <summary>
        /// Gets the list of icon buttons that appear on the Application Bar. 
        /// </summary>
        IList IApplicationBar.Buttons
		{
			get { return _buttons; }
		}

        /// <summary>
        /// Gets the list of the menu items that appear on the Application Bar.
        /// </summary>
        IList IApplicationBar.MenuItems
		{
			get { return _menuItems; }
		}

        /// <summary>
        /// Gets the list of icon buttons that appear on the Application Bar. 
        /// </summary>
		public BindableApplicationBarIconButtonCollection Buttons
		{
			get { return _buttons; }
		}

        /// <summary>
        /// Gets the list of the menu items that appear on the Application Bar.
        /// </summary>
		public BindableApplicationBarMenuItemCollection MenuItems
		{
			get { return _menuItems; }
		}

        /// <summary>
        /// Gets the internal <see cref="IApplicationBar"/>. 
        /// </summary>
		internal IApplicationBar InternalApplicationBar
		{
			get { return _internalApplicationBar; }
		}

        /// <summary>
        /// Occurs when the user opens or closes the Application Bar by clicking the ellipsis. 
        /// </summary>
		public event EventHandler<ApplicationBarStateChangedEventArgs> StateChanged
		{
			add { _internalApplicationBar.StateChanged += value; }
			remove { _internalApplicationBar.StateChanged -= value; }
		}

        #endregion

        #region Dependency properties

        /// <summary>
        /// Gets or sets the data context. 
        /// </summary>
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

        /// <summary>
        /// Gets or sets the background color. 
        /// </summary>
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
            _internalApplicationBar.BackgroundColor = newColor;
        } 

        /// <summary>
        /// Gets or sets the foreground color. 
        /// </summary>
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
            _internalApplicationBar.ForegroundColor = newColor;
        } 

        /// <summary>
        /// Gets or sets a value indicating whether the menu is enabled. 
        /// </summary>
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
            _internalApplicationBar.IsMenuEnabled = isMenuEnabled;
        }

        /// <summary>
        /// Gets or sets a value whether the <see cref="BindableApplicationBar"/> is visible. 
        /// </summary>
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
            _internalApplicationBar.IsVisible = isVisible;
        }

        /// <summary>
        /// Gets or sets the Application Bar mode. 
        /// </summary>
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
            _internalApplicationBar.Mode = mode;
        }

        /// <summary>
        /// Gets or sets the Application Bar opacity. 
        /// </summary>
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
            _internalApplicationBar.Opacity = opacity;
        }

        #endregion
        
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateDataContext();
        }

        private void UpdateDataContext()
        {
            _buttons.UpdateDataContext(DataContext);
            _menuItems.UpdateDataContext(DataContext);
        }
    }
}
