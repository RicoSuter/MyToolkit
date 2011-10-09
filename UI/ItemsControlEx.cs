using System;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.UI
{
	/// <summary>
	/// Extends an ItemsControl, raising an event when the PrepareContainerForItemOverride
	/// override is invoked.
	/// </summary>
	public class ItemsControlEx : ItemsControl
	{
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			OnPrepareContainerForItem(new PrepareContainerForItemEventArgs(element, item));
		}

		public Thickness InnerMargin
		{
			get { return (Thickness)GetValue(InnerMarginProperty); }
			set { SetValue(InnerMarginProperty, value); }
		}

		public static readonly DependencyProperty InnerMarginProperty =
			DependencyProperty.Register("InnerMargin", typeof(Thickness),
			typeof(ItemsControlEx), new PropertyMetadata(null));

		/// <summary>
		/// Occurs when the PrepareContainerForItemOverride method is invoked
		/// </summary>
		public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;

		/// <summary>
		/// Raises the PrepareContainerForItem event.
		/// </summary>
		protected void OnPrepareContainerForItem(PrepareContainerForItemEventArgs args)
		{
			if (PrepareContainerForItem != null)
				PrepareContainerForItem(this, args);
		}

		public override void OnApplyTemplate()
		{
			var itemsPresenter = (ItemsPresenter) GetTemplateChild("itemsPresenter");
			itemsPresenter.Margin = InnerMargin;
		}
	}

	/// <summary>
	/// Provides data for the PrepareContainerForItem event.
	/// </summary>
	public class PrepareContainerForItemEventArgs : EventArgs
	{
		public PrepareContainerForItemEventArgs(DependencyObject element, object item)
		{
			Element = element;
			Item = item;
		}

		public DependencyObject Element { get; private set; }
		public object Item { get; private set; }
	}
}