using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    public class LongListView : ListView
    {
        private NotifyCollectionChangedEventHandler _collectionChangedHandler = null;

        public LongListView()
        {
            SelectionChanged += OnSelectionChanged;

            // prevent memory leaks
            Loaded += delegate
            {
                if (_collectionChangedHandler != null)
                    ((INotifyCollectionChanged)ItemsSource).CollectionChanged += _collectionChangedHandler;
            };
            Unloaded += delegate
            {
                if (_collectionChangedHandler != null)
                    ((INotifyCollectionChanged)ItemsSource).CollectionChanged -= _collectionChangedHandler;
            };
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LongListView), new PropertyMetadata(default(object), OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = (LongListView)obj;
            control.UpdateList();

            if (control._collectionChangedHandler != null) // prevent memory leaks
            {
                ((INotifyCollectionChanged)args.OldValue).CollectionChanged -= control._collectionChangedHandler;
                control._collectionChangedHandler = null;
            }

            var collection = control.ItemsSource as INotifyCollectionChanged;
            if (collection != null)
            {
                control._collectionChangedHandler = (sender, e) => control.UpdateList();
                collection.CollectionChanged += control._collectionChangedHandler;
            }
        }

        public static readonly DependencyProperty GroupHeaderTemplateProperty =
            DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(LongListView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate GroupHeaderTemplate
        {
            get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
            set { SetValue(GroupHeaderTemplateProperty, value); }
        }


        private void UpdateList()
        {
            Items.Clear();
            foreach (var group in ItemsSource)
            {
                var headerViewItem = new ListViewItem { Content = group, ContentTemplate = GroupHeaderTemplate, Tag = this };
                Items.Add(headerViewItem);
                foreach (var item in (IEnumerable)group)
                {
                    var viewItem = new ListViewItem { Content = item, ContentTemplate = ItemTemplate };
                    Items.Add(viewItem);
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (SelectedItem != null)
            {
                var item = ((ListViewItem)SelectedItem);
                if (item.Tag == this)
                {
                    // TODO show group selector!
                }
                else
                    OnNavigate(new NavigationListEventArgs(item.Content));
            }

            SelectedItem = null;
        }

        public event EventHandler<NavigationListEventArgs> Navigate;

        protected void OnNavigate(NavigationListEventArgs args)
        {
            var copy = Navigate;
            if (copy != null)
                copy(this, args);
        }
    }
}
