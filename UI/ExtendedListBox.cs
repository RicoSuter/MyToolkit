using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MyToolkit.UI.UIExtensionMethods;

// developed by Rico Suter (http://rsuter.com), http://mytoolkit.codeplex.com

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
			LayoutUpdated += RegisterScrollEvent;
			ItemContainerStyle = (Style) XamlReader.Load(
				@"<Style TargetType=""ListBoxItem"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
					<Setter Property=""Template"">
						<Setter.Value>
							<ControlTemplate>
								<ContentPresenter HorizontalAlignment=""Stretch"" VerticalAlignment=""Stretch""/>
							</ControlTemplate>
						</Setter.Value>
					</Setter>
				</Style>");
		}

		#region inner margin

		public Thickness InnerMargin
		{
			get { return (Thickness)GetValue(InnerMarginProperty); }
			set { SetValue(InnerMarginProperty, value); }
		}

		public static readonly DependencyProperty InnerMarginProperty =
			DependencyProperty.Register("InnerMargin", typeof(Thickness),
			typeof(ExtendedListBox), new PropertyMetadata(new Thickness(), InnerMarginChanged));

		private static void InnerMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var box = (ExtendedListBox)d;
			if (box.lastElement != null)
				box.UpdateLastItemMargin();
			box.UpdateInnerMargin();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			UpdateInnerMargin();
		}

		private void UpdateInnerMargin()
		{
			var scrollViewer = (ScrollViewer) GetTemplateChild("ScrollViewer");
			if (scrollViewer != null)
			{
				var itemsPresenter = (ItemsPresenter)scrollViewer.Content;
				if (itemsPresenter != null)
					itemsPresenter.Margin = InnerMargin;
			}
		}

		private void UpdateLastItemMargin()
		{
			lastElement.Margin = new Thickness(lastElementMargin.Left, lastElementMargin.Top, lastElementMargin.Right,
				lastElementMargin.Bottom + InnerMargin.Top + InnerMargin.Bottom);
		}

		private FrameworkElement lastElement = null;
		private Thickness lastElementMargin;
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			OnPrepareContainerForItem(new PrepareContainerForItemEventArgs(element, item));

			if ((InnerMargin.Top > 0.0 || InnerMargin.Bottom > 0.0))
			{
				if (Items.IndexOf(item) == Items.Count - 1) // is last element of list
				{
					if (lastElement != element) // margin not already set
					{
						if (lastElement != null)
							lastElement.Margin = lastElementMargin;
						lastElement = (FrameworkElement)element;
						lastElementMargin = lastElement.Margin;
						UpdateLastItemMargin();
					}
				}
				else if (lastElement == element) // if last element is recycled it appears inside the list => reset margin
				{
					lastElement.Margin = lastElementMargin;
					lastElement = null; 
				}
			}
		}

		#endregion

		#region prepare container for item event

		public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;
		protected void OnPrepareContainerForItem(PrepareContainerForItemEventArgs args)
		{
			if (PrepareContainerForItem != null)
				PrepareContainerForItem(this, args);
		}

		#endregion

		#region scrolling

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

		private bool eventRegistred = false; 
		private void RegisterScrollEvent(object s, EventArgs eventArgs)
		{
			if (eventRegistred)
				return; 

			var scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
			if (scrollViewer != null)
			{
				var child = scrollViewer.GetVisualChild(0);
				var group = child.GetVisualStateGroup("ScrollStates");
				if (group != null)
				{
					group.CurrentStateChanging -= ScrollingStateChanging;
					group.CurrentStateChanging += ScrollingStateChanging;
					eventRegistred = true; 
				}
			}
		}

		//protected override Size ArrangeOverride(Size finalSize)
		//{
		//    var size = base.ArrangeOverride(finalSize);
		//    var scrollViewer = (ScrollViewer) GetTemplateChild("scrollViewer");
		//    try
		//    {
		//        var child = scrollViewer.GetVisualChild(0);
		//        var group = child.GetVisualStateGroup("ScrollStates");
		//        if (group != null)
		//        {
		//            group.CurrentStateChanging -= ScrollingStateChanging;
		//            group.CurrentStateChanging += ScrollingStateChanging;
		//        }
		//    }
		//    catch { }
		//    return size;
		//}

		#endregion
	}
}
