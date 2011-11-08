using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MyToolkit.UI.UIExtensionMethods;

namespace MyToolkit.UI
{
	public class ScrollingStateChangedEventArgs : EventArgs
	{
		public bool OldValue { get; private set; }
		public bool NewValue { get; private set; }
		public ScrollingStateChangedEventArgs(bool oldValue, bool newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}

	public class ExtendedListBox : ListBox
	{
		public ExtendedListBox()
		{
			DefaultStyleKey = typeof(ExtendedListBox);
		}

		public Thickness InnerMargin
		{
			get { return (Thickness)GetValue(InnerMarginProperty); }
			set { SetValue(InnerMarginProperty, value); }
		}

		public static readonly DependencyProperty InnerMarginProperty =
			DependencyProperty.Register("InnerMargin", typeof(Thickness),
			typeof(ExtendedListBox), new PropertyMetadata(null));

		public event EventHandler<ScrollingStateChangedEventArgs> ScrollingStateChanged;

		public static readonly DependencyProperty IsScrollingProperty = 
			DependencyProperty.Register("IsScrolling", typeof(bool),
			typeof(ExtendedListBox), new PropertyMetadata(false, IsScrollingPropertyChanged));

		private bool allowIsScrollingChanges;
		public bool IsScrolling
		{
			get { return (bool) GetValue(IsScrollingProperty); }
			internal set
			{
				// "Unlock" the ability to set the property
				allowIsScrollingChanges = true;
				SetValue(IsScrollingProperty, value);
				allowIsScrollingChanges = false;
			}
		}

		protected virtual void OnScrollingStateChanged(ScrollingStateChangedEventArgs e) { }

		internal static void IsScrollingPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			var listbox = (ExtendedListBox) source;
			if (listbox.allowIsScrollingChanges != true)
				throw new InvalidOperationException("IsScrolling property is read-only");

			var args = new ScrollingStateChangedEventArgs((bool)e.OldValue, (bool)e.NewValue);
			listbox.OnScrollingStateChanged(args);
			if (listbox.ScrollingStateChanged != null)
				listbox.ScrollingStateChanged(listbox, args);
		}

		void ScrollingStateChanging(object sender, VisualStateChangedEventArgs e)
		{
			IsScrolling = (e.NewState.Name == "Scrolling");
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var itemsPresenter = (ItemsPresenter) GetTemplateChild("itemsPresenter");
			itemsPresenter.Margin = InnerMargin;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var size = base.ArrangeOverride(finalSize);
			var scrollViewer = (ScrollViewer) GetTemplateChild("scrollViewer");
			try
			{
				var child = scrollViewer.GetVisualChild(0);
				var group = child.GetVisualStateGroup("ScrollStates");
				if (group != null)
				{
					group.CurrentStateChanging -= ScrollingStateChanging;
					group.CurrentStateChanging += ScrollingStateChanging;
				}
			}
			catch { }
			return size; 
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			OnPrepareContainerForItem(new PrepareContainerForItemEventArgs(element, item));
		}

		public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;
		protected void OnPrepareContainerForItem(PrepareContainerForItemEventArgs args)
		{
			if (PrepareContainerForItem != null)
				PrepareContainerForItem(this, args);
		}
	}
}
