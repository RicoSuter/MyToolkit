//-----------------------------------------------------------------------
// <copyright file="MtGridView.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Controls
{
    /// <summary>A <see cref="GridView"/> with additional features. </summary>
    public class MtGridView : GridView
    {
        private event EventHandler<ScrolledToEndEventArgs> _scrolledToEnd;
        private double _lastExtentWidth = 0;
        private bool _bindingCreated = false;

        /// <summary>Initializes a new instance of the <see cref="MtGridView"/> class. </summary>
        public MtGridView()
        {
            Loaded += OnLoaded;
        }

        /// <summary>Gets the view's <see cref="ScrollViewer"/>. </summary>
        public ScrollViewer ScrollViewer { get; private set; }

        /// <summary>Gets or sets a value indicating whether scrolled to end events should be triggered. </summary>
        public bool TriggerScrolledToEndEvents { get; set; }

        /// <summary>Occurs when the user scrolled to the end of the view. </summary>
        public event EventHandler<ScrolledToEndEventArgs> ScrolledToEnd
        {
            add
            {
                _scrolledToEnd += value;
                RegisterHorizontalOffsetChangedHandler();
            }
            remove { _scrolledToEnd -= value; }
        }

        /// <summary>Occurs when a new container control gets created. </summary>
        public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            OnPrepareContainerForItem(new PrepareContainerForItemEventArgs(element, item));
        }

        private void OnPrepareContainerForItem(PrepareContainerForItemEventArgs args)
        {
            var copy = PrepareContainerForItem;
            if (copy != null)
                copy(this, args);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
            RegisterHorizontalOffsetChangedHandler();
        }

        private static readonly DependencyProperty InternalOffsetProperty = DependencyProperty.Register(
            "InternalOffset", typeof(double), typeof(MtGridView),
            new PropertyMetadata(default(double), OnHorizontalOffsetChanged));

        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (MtGridView)d;
            if (!ctrl.TriggerScrolledToEndEvents || ctrl._scrolledToEnd == null)
                return;

            var viewer = ctrl.ScrollViewer;
            if (viewer != null)
            {
                var triggered = ctrl._lastExtentWidth == viewer.ExtentWidth;
                if (!triggered && viewer.HorizontalOffset >= viewer.ScrollableWidth - viewer.ViewportWidth - viewer.ViewportWidth / 2)
                {
                    ctrl._lastExtentWidth = viewer.ExtentWidth;
                    var handler = ctrl._scrolledToEnd;
                    if (handler != null)
                        handler(ctrl, new ScrolledToEndEventArgs(viewer));
                }
            }
        }

        private void RegisterHorizontalOffsetChangedHandler()
        {
            if (ScrollViewer == null || _bindingCreated || _scrolledToEnd == null)
                return;

            TriggerScrolledToEndEvents = true;

            var binding = new Binding();
            binding.Source = ScrollViewer;
            binding.Path = new PropertyPath("HorizontalOffset");
            binding.Mode = BindingMode.OneWay;
            SetBinding(InternalOffsetProperty, binding);

            _bindingCreated = true;
        }
    }

    /// <summary>A <see cref="GridView"/> with additional features. </summary>
    [Obsolete("Use MtGridView instead. 8/31/2014")]
    public class ExtendedGridView : MtGridView
    {
    }
}

#endif