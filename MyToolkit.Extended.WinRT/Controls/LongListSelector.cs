//-----------------------------------------------------------------------
// <copyright file="LongListSelector.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MyToolkit.Collections;

namespace MyToolkit.Controls
{
    /// <summary>Implementation of the <see cref="LongListSelector"/> with native look and feel. </summary>
    public class LongListSelector : Control
    {
        private ListView _listView;

        /// <summary>Initializes a new instance of the <see cref="LongListSelector"/> class. </summary>
        public LongListSelector()
        {
            DefaultStyleKey = typeof(LongListSelector);
        }

        /// <summary>Occurs when the user wants to navigate to an item. </summary>
        public event EventHandler<NavigationListEventArgs> Navigate;

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof (object), typeof (LongListSelector), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the item source (usually an <see cref="AlphaGroupCollection{T}"/> object). </summary>
        public object ItemsSource
        {
            get { return (object) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof (DataTemplate), typeof (LongListSelector), new PropertyMetadata(default(DataTemplate)));

        /// <summary>Gets or sets the item template for rendering an item. </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty UseNavigationEventProperty = DependencyProperty.Register(
            "UseNavigationEvent", typeof (bool), typeof (LongListSelector), new PropertyMetadata(true));

        /// <summary>Gets or sets a value indicating whether the <see cref="Navigate"/> event should be triggered
        /// when clicking on an item (default: true). </summary>
        public bool UseNavigationEvent
        {
            get { return (bool) GetValue(UseNavigationEventProperty); }
            set { SetValue(UseNavigationEventProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listView = (ListView)GetTemplateChild("ListView");
            _listView.ContainerContentChanging += ListViewOnContainerContentChanging;
        }

        protected void OnNavigate(NavigationListEventArgs args)
        {
            var copy = Navigate;
            if (copy != null)
                copy(this, args);
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
    }
}
