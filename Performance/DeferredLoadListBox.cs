// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace MyToolkit.Performance
{
    /// <summary>
    /// Implements a subclass of ListBox based on a StackPanel that defers the
    /// loading of off-screen items until necessary in order to minimize impact
    /// to the UI thread.
    /// </summary>
    public class DeferredLoadListBox : ListBox
    {
        private enum OverlapKind { Overlap, ChildAbove, ChildBelow };

        private ScrollViewer _scrollViewer;
        private ItemContainerGenerator _generator;
        private bool _queuedUnmaskVisibleContent;
        private bool _inOnApplyTemplate;

        /// <summary>
        /// Handles the application of the Control's Template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Unhook from old Template elements
            _inOnApplyTemplate = true;
            ClearValue(VerticalOffsetShadowProperty);
            _scrollViewer = null;
            _generator = null;

            // Apply new Template
            base.OnApplyTemplate();

            // Hook up to new Template elements
            _scrollViewer = FindFirstChildOfType<ScrollViewer>(this);
            if (null == _scrollViewer)
            {
                throw new NotSupportedException("Control Template must include a ScrollViewer (wrapping ItemsHost).");
            }
            _generator = ItemContainerGenerator;
            SetBinding(VerticalOffsetShadowProperty, new Binding { Source = _scrollViewer, Path = new PropertyPath("VerticalOffset") });
            _inOnApplyTemplate = false;
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own item container.
        /// </summary>
        /// <param name="item">The specified item.</param>
        /// <returns>true if the item is its own item container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            // Check container type
            return item is DeferredLoadListBoxItem;
        }

        /// <summary>
        /// Creates or identifies the element used to display a specified item.
        /// </summary>
        /// <returns>A DeferredLoadListBoxItem corresponding to a specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            // Create container (matches ListBox implementation)
            var item = new DeferredLoadListBoxItem();
            if (ItemContainerStyle != null)
            {
                item.Style = ItemContainerStyle;
            }
            return item;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            // Perform base class preparation
            base.PrepareContainerForItemOverride(element, item);

            // Mask the container's content
            var container = (DeferredLoadListBoxItem)element;
            if (!DesignerProperties.IsInDesignTool)
            {
                container.MaskContent();
            }

            // Queue a (single) pass to unmask newly visible content on the next tick
            if (!_queuedUnmaskVisibleContent)
            {
                _queuedUnmaskVisibleContent = true;
                Dispatcher.BeginInvoke(() =>
                {
                    _queuedUnmaskVisibleContent = false;
                    UnmaskVisibleContent();
                });
            }
        }

        private static readonly DependencyProperty VerticalOffsetShadowProperty =
            DependencyProperty.Register("VerticalOffsetShadow", typeof(double), typeof(DeferredLoadListBox), new PropertyMetadata(-1.0, OnVerticalOffsetShadowChanged));
        private static void OnVerticalOffsetShadowChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Handle ScrollViewer VerticalOffset change by unmasking newly visible content
            ((DeferredLoadListBox)o).UnmaskVisibleContent();
        }

        private void UnmaskVisibleContent()
        {
            // Capture variables
            var count = Items.Count;

            // Find index of any container within view using (1-indexed) binary search
            var index = -1;
            var l = 0;
            var r = count + 1;
            while (-1 == index)
            {
                var p = (r - l) / 2;
                if (0 == p)
                {
                    break;
                }
                p += l;
                var c = (DeferredLoadListBoxItem)_generator.ContainerFromIndex(p - 1);
                if (null == c)
                {
                    if (_inOnApplyTemplate)
                    {
                        // Applying template; don't expect to have containers at this point
                        return;
                    }
                    // Should always be able to get the container
                    var presenter = FindFirstChildOfType<ItemsPresenter>(_scrollViewer);
                    var panel = (null == presenter) ? null : FindFirstChildOfType<Panel>(presenter);
                    if (panel is VirtualizingStackPanel)
                    {
                        throw new NotSupportedException("Must change ItemsPanel to be a StackPanel (via the ItemsPanel property).");
                    }
                    else
                    {
                        throw new NotSupportedException("Couldn't find container for item (ItemsPanel should be a StackPanel).");
                    }
                }
                switch (Overlap(_scrollViewer, c, 0))
                {
                    case OverlapKind.Overlap:
                        index = p - 1;
                        break;
                    case OverlapKind.ChildAbove:
                        l = p;
                        break;
                    case OverlapKind.ChildBelow:
                        r = p;
                        break;
                }
            }

            if (-1 != index)
            {
                // Unmask visible items below the current item
                for (var i = index; i < count; i++)
                {
                    if (!UnmaskItemContent(i))
                    {
                        break;
                    }
                }

                // Unmask visible items above the current item
                for (var i = index - 1; 0 <= i; i--)
                {
                    if (!UnmaskItemContent(i))
                    {
                        break;
                    }
                }
            }
        }

        private bool UnmaskItemContent(int index)
        {
            var container = (DeferredLoadListBoxItem)_generator.ContainerFromIndex(index);
            if (null != container)
            {
                // Return quickly if not masked (but periodically check visibility anyway so we can stop once we're out of range)
                if (!container.Masked && (0 != (index % 16)))
                {
                    return true;
                }
                // Check necessary conditions
                if (0 == container.ActualHeight)
                {
                    // In some cases, ActualHeight will be 0 here, but can be "fixed" with an explicit call to UpdateLayout
                    container.UpdateLayout();
                    if (0 == container.ActualHeight)
                    {
                        throw new NotSupportedException("All containers must have a Height set (ex: via ItemContainerStyle), though the heights do not all need to be the same.");
                    }
                }
                // If container overlaps the "visible" area (i.e. on or near the screen), unmask it
                if (OverlapKind.Overlap == Overlap(_scrollViewer, container, 2 * _scrollViewer.ActualHeight))
                {
                    container.UnmaskContent();
                    return true;
                }
            }
            return false;
        }

        private static bool Overlap(double startA, double endA, double startB, double endB)
        {
            return (((startA <= startB) && (startB <= endA)) ||
                    ((startB <= startA) && (startA <= endB)));
        }

        private static OverlapKind Overlap(ScrollViewer parent, FrameworkElement child, double padding)
        {
            // Get child transform relative to parent
            //var transform = child.TransformToVisual(parent); // Unreliable on Windows Phone 7; throws ArgumentException sometimes
            var layoutSlot = LayoutInformation.GetLayoutSlot(child);
            var transform = new TranslateTransform { /*X = layoutSlot.Left - parent.HorizontalOffset,*/ Y = layoutSlot.Top - parent.VerticalOffset };
            // Get child bounds relative to parent
            var bounds = new Rect(transform.Transform(new Point()), transform.Transform(new Point(/*child.ActualWidth*/ 0, child.ActualHeight)));
            // Return kind of overlap
            if (Overlap(0 - padding, parent.ActualHeight + padding, bounds.Top, bounds.Bottom))
            {
                return OverlapKind.Overlap;
            }
            else if (bounds.Top < 0)
            {
                return OverlapKind.ChildAbove;
            }
            else
            {
                return OverlapKind.ChildBelow;
            }
        }

        private static T FindFirstChildOfType<T>(DependencyObject root) where T : class
        {
            // Enqueue root node
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (0 < queue.Count)
            {
                // Dequeue next node and check its children
                var current = queue.Dequeue();
                for (var i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (null != typedChild)
                    {
                        return typedChild;
                    }
                    // Enqueue child
                    queue.Enqueue(child);
                }
            }
            // No children match
            return null;
        }
    }
}
