using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyToolkit.Collections;
using MyToolkit.Utilities;

namespace MyToolkit.Controls
{
    public class FilterListBox : Control
    {
        private ListBox _listBox;

        static FilterListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterListBox), new FrameworkPropertyMetadata(typeof(FilterListBox)));
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            "Filter", typeof (string), typeof (FilterListBox), new PropertyMetadata(default(string), (o, args) => ((FilterListBox)o).OnFilterChanged()));

        public string Filter
        {
            get { return (string) GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof (object), typeof (FilterListBox), new PropertyMetadata(default(object), (o, args) => ((FilterListBox)o).OnItemsSourceChanged()));

        public object ItemsSource
        {
            get { return (object) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof (object), typeof (FilterListBox), new PropertyMetadata(default(object)));

        public object SelectedItem
        {
            get { return (object) GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            "SelectionMode", typeof (SelectionMode), typeof (FilterListBox), new PropertyMetadata(default(SelectionMode)));

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode) GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof (DataTemplate), typeof (FilterListBox), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register(
            "HorizontalContentAlignment", typeof (HorizontalAlignment), typeof (FilterListBox), new PropertyMetadata(default(HorizontalAlignment)));

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment) GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty FilterPathProperty = DependencyProperty.Register(
            "FilterPath", typeof(string), typeof(FilterListBox), new PropertyMetadata(default(string), (o, args) => ((FilterListBox)o).OnFilterChanged()));

        public string FilterPath
        {
            get { return (string) GetValue(FilterPathProperty); }
            set { SetValue(FilterPathProperty, value); }
        }

        public static readonly DependencyProperty FilteredItemsProperty = DependencyProperty.Register(
            "FilteredItems", typeof (IObservableCollectionView), typeof (FilterListBox), new PropertyMetadata(default(IObservableCollectionView)));

        public IObservableCollectionView FilteredItems
        {
            get { return (IObservableCollectionView) GetValue(FilteredItemsProperty); }
            private set { SetValue(FilteredItemsProperty, value); }
        }
        

        private void OnFilterChanged()
        {
            if (FilteredItems != null)
                FilteredItems.Refresh();
        }

        private void OnItemsSourceChanged()
        {
            if (_listBox != null)
            {
                if (ItemsSource != null)
                {
                    FilteredItems = (IObservableCollectionView)typeof(ObservableCollectionView<>)
                        .CreateGenericObject(ItemsSource.GetType().GenericTypeArguments[0], ItemsSource);
                    FilteredItems.Filter = new Func<object, bool>(ApplyFilter);
                    _listBox.ItemsSource = FilteredItems;
                }
                else
                    _listBox.ItemsSource = null;

                
            }
        }

        private bool ApplyFilter(object item)
        {
            if (!string.IsNullOrEmpty(FilterPath) && !string.IsNullOrEmpty(Filter))
            {
                var terms = Filter.ToLower().Split(' ');
                var value = item.GetType().GetProperty(FilterPath).GetValue(item);
                if (value != null)
                    return terms.All(t => value.ToString().ToLower().Contains(t));
                else
                    return false;
            }
            else
                return true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _listBox = (ListBox)GetTemplateChild("ListBox");
            OnItemsSourceChanged();
        }
    }
}
