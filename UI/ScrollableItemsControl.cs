using System;
using System.Collections;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyToolkit.UI
{
	public class ScrollableItemsControl : ListBox // TODO: change to ItemsControl => problem: long list, scroll to bottom, go to other page, go back => wrong scroll position ?? correct only with ListBox 
	{
		public ScrollableItemsControl()
		{
			DefaultStyleKey = typeof(ScrollableItemsControl);
		}

		public Thickness InnerMargin
		{
			get { return (Thickness)GetValue(InnerMarginProperty); }
			set { SetValue(InnerMarginProperty, value); }
		}

		public static readonly DependencyProperty InnerMarginProperty =
			DependencyProperty.Register("InnerMargin", typeof(Thickness),
			typeof(ScrollableItemsControl), new PropertyMetadata(null));

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var itemsPresenter = (ItemsPresenter)GetTemplateChild("itemsPresenter");
			itemsPresenter.Margin = InnerMargin;
		}
	}
}
