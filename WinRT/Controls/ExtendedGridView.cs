using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Controls
{
	public class ScrolledToEndEventArgs : EventArgs
	{
		public ScrollViewer ScrollViewer { get; set; }
		public ScrolledToEndEventArgs(ScrollViewer viewer)
		{
			ScrollViewer = viewer;
		}
	}

	public class ExtendedGridView : GridView
	{
		private ScrollViewer scrollViewer;
		public ScrollViewer ScrollViewer
		{
			get { return scrollViewer; }
		}

		public ExtendedGridView()
		{
			Loaded += OnLoaded;
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			OnPrepareContainerForItem(new PrepareContainerForItemEventArgs(element, item));
		}

		public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;
		protected void OnPrepareContainerForItem(PrepareContainerForItemEventArgs args)
		{
			var copy = PrepareContainerForItem;
			if (copy != null)
				copy(this, args);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
			RegisterScrollOffset();
		}

		public bool TriggerScrolledToEndEvents { get; set; }

		private event EventHandler<ScrolledToEndEventArgs> scrolledToEnd;
		public event EventHandler<ScrolledToEndEventArgs> ScrolledToEnd
		{
			add
			{
				scrolledToEnd += value;
				RegisterScrollOffset();
			}
			remove { scrolledToEnd -= value; }
		}

		private double lastViewportWidth = 0;
		private static void OnListVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (ExtendedGridView)d;
			if (!ctrl.TriggerScrolledToEndEvents || ctrl.scrolledToEnd == null)
				return;

			var viewer = ctrl.scrollViewer;
			if (viewer != null)
			{
				var triggered = ctrl.lastViewportWidth == viewer.ViewportWidth;
				if (!triggered && viewer.HorizontalOffset >= viewer.ScrollableWidth - viewer.ViewportWidth - viewer.ViewportWidth / 2)
				{
					ctrl.lastViewportWidth = viewer.ViewportWidth;
					var handler = ctrl.scrolledToEnd;
					if (handler != null)
						handler(ctrl, new ScrolledToEndEventArgs(viewer));
				}
			}
		}

		private static readonly DependencyProperty InternalOffsetProperty = DependencyProperty.Register(
			"InternalOffset", typeof(double), typeof(ExtendedGridView),
			new PropertyMetadata(default(double), OnListVerticalOffsetChanged));

		private bool bindingCreated = false;
		private void RegisterScrollOffset()
		{
			if (scrollViewer == null || bindingCreated || scrolledToEnd == null)
				return;

			TriggerScrolledToEndEvents = true;

			var binding = new Binding();
			binding.Source = scrollViewer;
			binding.Path = new PropertyPath("HorizontalOffset");
			binding.Mode = BindingMode.OneWay;
			SetBinding(InternalOffsetProperty, binding);

			bindingCreated = true;
		}
	}
}
