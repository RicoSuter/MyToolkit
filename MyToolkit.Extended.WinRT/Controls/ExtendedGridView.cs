using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Controls
{
	public class ExtendedGridView : GridView
	{
        private event EventHandler<ScrolledToEndEventArgs> _scrolledToEnd;
        private ScrollViewer _scrollViewer;
        private double _lastExtentWidth = 0;
        private bool _bindingCreated = false;

		public ExtendedGridView()
		{
			Loaded += OnLoaded;
		}

        /// <summary>
        /// Gets the view's <see cref="ScrollViewer"/>. 
        /// </summary>
        public ScrollViewer ScrollViewer
        {
            get { return _scrollViewer; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether scrolled to end events should be triggered. 
        /// </summary>
        public bool TriggerScrolledToEndEvents { get; set; }

        /// <summary>
        /// Occurs when the user scrolled to the end of the view. 
        /// </summary>
        public event EventHandler<ScrolledToEndEventArgs> ScrolledToEnd
        {
            add
            {
                _scrolledToEnd += value;
                RegisterHorizontalOffsetChangedHandler();
            }
            remove { _scrolledToEnd -= value; }
        }

        /// <summary>
        /// Occurs when a new container control gets created. 
        /// </summary>
        public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;
        
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			OnPrepareContainerForItem(new PrepareContainerForItemEventArgs(element, item));
		}

		private void OnPrepareContainerForItem(PrepareContainerForItemEventArgs args)
		{
			var copy = PrepareContainerForItem;
			if (copy != null)
				copy(this, args);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
			RegisterHorizontalOffsetChangedHandler();
		}

        private static readonly DependencyProperty InternalOffsetProperty = DependencyProperty.Register(
            "InternalOffset", typeof(double), typeof(ExtendedGridView),
            new PropertyMetadata(default(double), OnHorizontalOffsetChanged));
        
        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (ExtendedGridView)d;
			if (!ctrl.TriggerScrolledToEndEvents || ctrl._scrolledToEnd == null)
				return;

			var viewer = ctrl._scrollViewer;
			if (viewer != null)
			{
				var triggered = ctrl._lastExtentWidth == viewer.ExtentWidth;
				if (!triggered && viewer.HorizontalOffset >= viewer.ScrollableWidth - viewer.ViewportWidth - viewer.ViewportWidth / 2)
				{
                    ctrl._lastExtentWidth = viewer.ExtentWidth;
					var handler = ctrl._scrolledToEnd;
					if (handler != null)
						handler(ctrl, new ScrolledToEndEventArgs(viewer));
				}
			}
		}

		private void RegisterHorizontalOffsetChangedHandler()
		{
			if (_scrollViewer == null || _bindingCreated || _scrolledToEnd == null)
				return;

			TriggerScrolledToEndEvents = true;

			var binding = new Binding();
			binding.Source = _scrollViewer;
			binding.Path = new PropertyPath("HorizontalOffset");
			binding.Mode = BindingMode.OneWay;
			SetBinding(InternalOffsetProperty, binding);

			_bindingCreated = true;
		}
	}

    public class ScrolledToEndEventArgs : EventArgs
    {
        public ScrollViewer ScrollViewer { get; set; }
        public ScrolledToEndEventArgs(ScrollViewer viewer)
        {
            ScrollViewer = viewer;
        }
    }
}
