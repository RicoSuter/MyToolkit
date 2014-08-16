using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
    public class LongListSelector : Control
    {
        public LongListSelector()
        {
            DefaultStyleKey = typeof(LongListSelector);
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof (object), typeof (LongListSelector), new PropertyMetadata(default(object)));

        public object ItemsSource
        {
            get { return (object) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof (DataTemplate), typeof (LongListSelector), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private ListView _listView;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listView = (ListView)GetTemplateChild("ListView");
            _listView.ContainerContentChanging += ListViewOnContainerContentChanging;
        }

        private void ListViewOnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.ItemContainer.Tag = args.Item;
            args.ItemContainer.Tapped -= ItemContainerOnTapped;
            args.ItemContainer.Tapped += ItemContainerOnTapped;
        }

        private void ItemContainerOnTapped(object sender, TappedRoutedEventArgs args)
        {
            if (UseNavigationEvent)
            {
                OnNavigate(new NavigationListEventArgs(((SelectorItem)sender).Tag));
                _listView.SelectedItem = null; 
            }
        }

        public static readonly DependencyProperty UseNavigationEventProperty = DependencyProperty.Register(
            "UseNavigationEvent", typeof (bool), typeof (LongListSelector), new PropertyMetadata(true));

        public bool UseNavigationEvent
        {
            get { return (bool) GetValue(UseNavigationEventProperty); }
            set { SetValue(UseNavigationEventProperty, value); }
        }

		public event EventHandler<NavigationListEventArgs> Navigate;

		protected void OnNavigate(NavigationListEventArgs args)
		{
			var copy = Navigate;
			if (copy != null)
				copy(this, args);
		}
    }

    //public class NavigationListEventArgs : EventArgs
    //{
    //    internal NavigationListEventArgs(object item)
    //    {
    //        Item = item;
    //    }

    //    public object Item { private set; get; }

    //    public T GetItem<T>()
    //    {
    //        return (T)Item;
    //    }
    //}
}
