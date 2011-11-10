using System;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.UI
{
	public class ExtendedItemsControl : ItemsControl
	{
		#region inner margin

		public Thickness InnerMargin
		{
			get { return (Thickness)GetValue(InnerMarginProperty); }
			set { SetValue(InnerMarginProperty, value); }
		}

		public static readonly DependencyProperty InnerMarginProperty =
			DependencyProperty.Register("InnerMargin", typeof(Thickness),
			typeof(ExtendedItemsControl), new PropertyMetadata(new Thickness(), InnerMarginChanged));

		private static void InnerMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var box = (ExtendedItemsControl)d;
			if (box.lastElement != null)
				box.ResetLastItemMargin();
			box.ResetInnerMargin();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ResetInnerMargin();
		}

		private void ResetInnerMargin()
		{
			var itemsPresenter = (ItemsPresenter)GetTemplateChild("itemsPresenter");
			if (itemsPresenter != null)
				itemsPresenter.Margin = InnerMargin;
		}

		private void ResetLastItemMargin()
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

			if (InnerMargin != new Thickness() && Items.IndexOf(item) == Items.Count - 1)
			{
				if (lastElement != null)
					lastElement.Margin = lastElementMargin;
				lastElement = (FrameworkElement)element;
				lastElementMargin = lastElement.Margin;
				ResetLastItemMargin();
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
	}
}