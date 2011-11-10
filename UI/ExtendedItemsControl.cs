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

		private FrameworkElement lastElement = null;
		private Thickness lastElementMargin;
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			OnPrepareContainerForItem(new PrepareContainerForItemEventArgs(element, item));

			// changes to this function must be applied also to ExtendedItemsControl
			// TODO: hack to see all elements, only needed if InnerMargin is set
			if (InnerMargin != new Thickness() && Items.IndexOf(item) == Items.Count - 1)
			{
				if (lastElement != null)
					lastElement.Margin = lastElementMargin;

				lastElement = (FrameworkElement)element;
				lastElementMargin = lastElement.Margin;
				lastElement.Margin = new Thickness(lastElementMargin.Left, lastElementMargin.Top,
					lastElementMargin.Right, lastElementMargin.Bottom + InnerMargin.Top + InnerMargin.Bottom);
			}
		}

		public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;
		protected void OnPrepareContainerForItem(PrepareContainerForItemEventArgs args)
		{
			if (PrepareContainerForItem != null)
				PrepareContainerForItem(this, args);
		}
	}
}