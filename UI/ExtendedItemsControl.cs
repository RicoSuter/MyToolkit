using System;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.UI
{
	public class ExtendedItemsControl : ItemsControl
	{
		public Thickness InnerMargin
		{
			get { return (Thickness)GetValue(InnerMarginProperty); }
			set { SetValue(InnerMarginProperty, value); }
		}

		public static readonly DependencyProperty InnerMarginProperty =
			DependencyProperty.Register("InnerMargin", typeof(Thickness),
			typeof(ExtendedItemsControl), new PropertyMetadata(null));

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var itemsPresenter = (ItemsPresenter)GetTemplateChild("itemsPresenter");
			itemsPresenter.Margin = InnerMargin;
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