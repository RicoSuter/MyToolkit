//-----------------------------------------------------------------------
// <copyright file="FrameworkElementExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

#if WPF || SL5 || WP7 || WP8
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
#elif WINRT
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Diagnostics;
using MyToolkit.Utilities;

namespace MyToolkit.UI
{
    /// <summary>Provides extension methods for <see cref="FrameworkElement"/> objects. </summary>
    public static class FrameworkElementExtensions
    {
        /// <summary>Gets the vertical offset for a ListBox</summary>
        /// <param name="list">The ListBox to check</param>
        /// <returns>The vertical offset</returns>
        public static double GetVerticalScrollOffset(this ListBox list)
        {
            ScrollViewer viewer = (ScrollViewer)list.FindVisualChild("ScrollViewer");
            return viewer.VerticalOffset;
        }

        /// <summary>Gets the horizontal offset for a ListBox</summary>
        /// <param name="list">The ListBox to check</param>
        /// <returns>The horizontal offset</returns>
        public static double GetHorizontalScrollOffset(this ListBox list)
        {
            ScrollViewer viewer = (ScrollViewer)list.FindVisualChild("ScrollViewer");
            return viewer.HorizontalOffset;
        }

        /// <summary>Gets the vertical scroll position of the element's <see cref="ScrollViewer"/>. </summary>
        /// <param name="element">The element which must have a <see cref="ScrollViewer"/> as child. </param>
        /// <returns>The scroll position. </returns>
        public static double GetVerticalScrollPosition(this FrameworkElement element)
        {
            var scrollViewer = element.FindVisualChild<ScrollViewer>();
            return scrollViewer.VerticalOffset;
        }

        /// <summary>Sets the horizontal scroll position of the element's <see cref="ScrollViewer"/>. </summary>
        /// <param name="element">The element which must have a <see cref="ScrollViewer"/> as child. </param>
        /// <returns>The scroll position. </returns>
        public static double GetHorizontalScrollPosition(this FrameworkElement element)
        {
            var scrollViewer = element.FindVisualChild<ScrollViewer>();
            return scrollViewer.HorizontalOffset;
        }

        /// <summary>Sets the horizontal scroll position of the element's <see cref="ScrollViewer"/>. </summary>
        /// <param name="element">The element which must have a <see cref="ScrollViewer"/> as child. </param>
        /// <param name="position">The scroll position. </param>
        public static void SetVerticalScrollPosition(this FrameworkElement element, double position)
        {
            var scrollViewer = element.FindVisualChild<ScrollViewer>();
            scrollViewer.ScrollToVerticalOffset(position);
        }

        /// <summary>Gets the horizontal scroll position of the element's <see cref="ScrollViewer"/>. </summary>
        /// <param name="element">The element which must have a <see cref="ScrollViewer"/> as child. </param>
        /// <param name="position">The scroll position. </param>
        public static void SetHorizontalScrollPosition(this FrameworkElement element, double position)
        {
            var scrollViewer = element.FindVisualChild<ScrollViewer>();
            scrollViewer.ScrollToHorizontalOffset(position);
        }

#if WPF || WINRT || WP8 || WP7 || SL5

        /// <summary>Checks whether an element which is contained in a container is currently visible on the screen. </summary>
        /// <param name="element">The element. </param>
        /// <param name="container">The element's container (e.g. a <see cref="ListBox"/>). </param>
        /// <returns>true if the element is visible to the user; false otherwise. </returns>
        public static bool IsVisibleOnScreen(this FrameworkElement element, FrameworkElement container)
        {
#if WPF
            if (!element.IsVisible)
                return false;

            var bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
#else
            var bounds = element.TransformToVisual(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(new Point(bounds.Left, bounds.Top)) || rect.Contains(new Point(bounds.Right, bounds.Right));
#endif
        }

#endif

        /// <summary>Finds the parent data context of the <see cref="DependencyObject"/> by checking the parents.</summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/>.</param>
        /// <returns>The data context or <c>null</c> when no data context could be found. </returns>
        public static object FindParentDataContext(this DependencyObject dependencyObject)
        {
            if (dependencyObject is FrameworkElement)
            {
                if (((FrameworkElement)dependencyObject).DataContext != null)
                    return ((FrameworkElement)dependencyObject).DataContext;
            }

            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent != null)
                return FindParentDataContext(parent);

            return null;
        }

#if WINRT

        /// <summary>Gets the rectangle of the given <see cref="FrameworkElement"/>.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The rectangle. </returns>
        public static Rect GetElementRect(this FrameworkElement element)
        {
            var transform = element.TransformToVisual(null);
            var point = transform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

#endif

        /// <summary>Gets the rectangle of the element in which is contained in a <see cref="Canvas"/>. </summary>
        /// <param name="element">The element.</param>
        /// <returns>The rectangle. </returns>
        public static Rect GetCanvasElementRect(this FrameworkElement element)
        {
            return new Rect(new Point(Canvas.GetLeft(element), Canvas.GetTop(element)),
                new Size(element.ActualWidth, element.ActualHeight));
        }

        /// <summary>Finds a <see cref="FrameworkElement" /> by its name by scanning the visual tree.</summary>
        /// <param name="root">The root node.</param>
        /// <param name="name">The element name to search.</param>
        /// <returns>The found element or <c>null</c> if no element could be found. </returns>
        public static FrameworkElement FindVisualChild(this FrameworkElement root, string name)
        {
            var element = root.FindName(name) as FrameworkElement;
            if (element != null)
                return element;

            foreach (FrameworkElement parent in root.GetVisualDescendents())
            {
                element = parent.FindName(name) as FrameworkElement;
                if (element != null)
                    return element;
            }

            return null;
        }

#if !WPF

        /// <summary>Returns an element's <see cref="PlaneProjection"/>. </summary>
        /// <param name="element">The element. </param>
        /// <param name="createIfNecessary">Whether or not to create the projection if it doesn't already exist. </param>
        /// <returns>The plane project, or null if not found or created. </returns>
        public static PlaneProjection GetPlaneProjection(this UIElement element, bool createIfNecessary)
        {
            Projection originalProjection = element.Projection;
            if (originalProjection is PlaneProjection)
                return (PlaneProjection)originalProjection;

            if (originalProjection == null)
            {
                if (createIfNecessary)
                {
                    var projection = new PlaneProjection();
                    element.Projection = projection;
                    return projection;
                }
            }
            return null;
        }

#endif

        /// <summary>Returns a render transform of the specified type from the element, creating it if necessary. </summary>
        /// <typeparam name="TRequestedTransform">The type of transform (Rotate, Translate, etc)</typeparam>
        /// <param name="element">The element to check</param>
        /// <param name="mode">The mode to use for creating transforms, if not found</param>
        /// <returns>The specified transform, or null if not found and not created</returns>
        public static TRequestedTransform GetTransform<TRequestedTransform>(this UIElement element, TransformCreationMode mode)
            where TRequestedTransform : Transform, new()
        {
            Transform originalTransform = element.RenderTransform;
            if (originalTransform == null)
            {
                if ((mode & TransformCreationMode.Create) == TransformCreationMode.Create)
                {
                    element.RenderTransform = new TRequestedTransform();
                    return (TRequestedTransform)element.RenderTransform;
                }
                return null;
            }

            var requestedTransform = originalTransform as TRequestedTransform;
            if (requestedTransform != null)
                return requestedTransform;

            var matrixTransform = originalTransform as MatrixTransform;
            if (matrixTransform != null)
            {
                if (matrixTransform.Matrix.IsIdentity
                    && (mode & TransformCreationMode.Create) == TransformCreationMode.Create
                    && (mode & TransformCreationMode.IgnoreIdentityMatrix) == TransformCreationMode.IgnoreIdentityMatrix)
                {
                    requestedTransform = new TRequestedTransform();
                    element.RenderTransform = requestedTransform;
                    return requestedTransform;
                }

                return null;
            }

            var transformGroup = originalTransform as TransformGroup;
            if (transformGroup != null)
            {
                foreach (Transform child in transformGroup.Children)
                {
                    if (child is TRequestedTransform)
                        return child as TRequestedTransform;
                }

                if ((mode & TransformCreationMode.AddToGroup) == TransformCreationMode.AddToGroup)
                {
                    requestedTransform = new TRequestedTransform();
                    transformGroup.Children.Add(requestedTransform);
                    return requestedTransform;
                }

                return null;
            }

            if ((mode & TransformCreationMode.CombineIntoGroup) == TransformCreationMode.CombineIntoGroup)
            {
                requestedTransform = new TRequestedTransform();
                transformGroup = new TransformGroup();
                transformGroup.Children.Add(originalTransform);
                transformGroup.Children.Add(requestedTransform);
                element.RenderTransform = transformGroup;
                return requestedTransform;
            }

            return null;
        }


        /// <summary>Gets the visual parent of an element. </summary>
        /// <param name="node">The element whose parent is desired</param>
        /// <returns>The visual parent, or null if not found (usually means visual tree is not ready)</returns>
        public static FrameworkElement GetVisualParent(this FrameworkElement node)
        {
            try
            {
                return VisualTreeHelper.GetParent(node) as FrameworkElement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception while trying to get parent. " + ex.Message);
            }
            return null;
        }

        /// <summary>Gets the visual parent of an element and a given type. </summary>
        /// <param name="element">The element whose parent is desired</param>
        /// <returns>The visual parent, or null if not found (usually means visual tree is not ready)</returns>
        public static TParent GetVisualParentOfType<TParent>(this FrameworkElement element)
            where TParent : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                if (parent is TParent)
                    return (TParent)parent;

                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        /// <summary>Returns a visual child of an element</summary>
        /// <param name="node">The element whose child is desired</param>
        /// <param name="index">The index of the child</param>
        /// <returns>The found child, or null if not found (usually means visual tree is not ready)</returns>
        public static FrameworkElement GetVisualChild(this FrameworkElement node, int index)
        {
            try
            {
                return VisualTreeHelper.GetChild(node, index) as FrameworkElement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception while trying to get child index " + index + ". " + ex.Message);
            }
            return null;
        }

        /// <summary>Gets the visual children of type T.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject target)
            where T : DependencyObject
        {
            return GetVisualChildren(target).Where(child => child is T).Cast<T>();
        }

        /// <summary>Gets the visual children of type T.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <param name="strict">if set to <c>true</c> [strict].</param>
        /// <returns></returns>
        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject target, bool strict)
            where T : DependencyObject
        {
            return GetVisualChildren(target, strict).Where(child => child is T).Cast<T>();
        }

        /// <summary>Get the visual tree children of an element.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree children of an element.</returns>
        /// <exception cref="ArgumentNullException">The value of 'element' cannot be null. </exception>
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return GetVisualChildrenAndSelfIterator(element).Skip(1);
        }

        /// <summary>Get the visual tree children of an element and the element itself.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree children of an element and the element itself.</returns>
        private static IEnumerable<DependencyObject> GetVisualChildrenAndSelfIterator(this DependencyObject element)
        {
            yield return element;

            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
                yield return VisualTreeHelper.GetChild(element, i);
        }

        /// <summary>Gets the visual children.</summary>
        /// <param name="target">The target.</param>
        /// <param name="strict">Prevents the search from navigating the logical tree; eg. ContentControl.Content</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject target, bool strict)
        {
            int count = VisualTreeHelper.GetChildrenCount(target);
            if (count == 0)
            {
                if (!strict && target is ContentControl)
                {
                    var child = ((ContentControl)target).Content as DependencyObject;
                    if (child != null)
                        yield return child;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                    yield return VisualTreeHelper.GetChild(target, i);
            }
        }

        /// <summary>Gets all the visual children of the element</summary>
        /// <param name="root">The element to get children of</param>
        /// <returns>An enumerator of the children</returns>
        public static IEnumerable<FrameworkElement> GetVisualChildren(this FrameworkElement root)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                yield return VisualTreeHelper.GetChild(root, i) as FrameworkElement;
        }

        /// <summary>A helper method used to get visual descendants using a depth-first strategy.</summary>
        /// <param name="target">The target.</param>
        /// <param name="strict">Prevents the search from navigating the logical tree; eg. ContentControl.Content</param>
        /// <param name="stack">The stack.</param>
        /// <returns></returns>
        private static IEnumerable<DependencyObject> GetVisualDescendants(DependencyObject target, bool strict, Stack<DependencyObject> stack)
        {
            foreach (var child in GetVisualChildren(target, strict))
                stack.Push(child);

            if (stack.Count == 0)
                yield break;

            DependencyObject node = stack.Pop();
            yield return node;

            foreach (var descendant in GetVisualDescendants(node, strict, stack))
                yield return descendant;
        }

        /// <summary>Get the visual tree descendants of an element.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree descendants of an element.</returns>
        /// <exception cref="ArgumentNullException">The value of 'element' cannot be null. </exception>
        public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return GetVisualDescendantsAndSelfIterator(element).Skip(1);
        }

        /// <summary>Get the visual tree descendants of an element.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree descendants of an element.</returns>
        /// <exception cref="ArgumentNullException">The value of 'element' cannot be null. </exception>
        public static IEnumerable<TDescendant> GetVisualDescendantsOfType<TDescendant>(this DependencyObject element)
            where TDescendant : DependencyObject
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return GetVisualDescendantsAndSelfIterator(element).Skip(1).OfType<TDescendant>();
        }

        /// <summary>Get the first visual tree descendant of an element.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree descendants of an element.</returns>
        /// <exception cref="ArgumentNullException">The value of 'element' cannot be null. </exception>
        public static TDescendant GetFirstVisualDescendantOfType<TDescendant>(this DependencyObject element)
            where TDescendant : DependencyObject
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return GetVisualDescendantsAndSelfIterator(element).Skip(1).OfType<TDescendant>().First();
        }

        /// <summary>Get the visual tree descendants of an element and the element itself. </summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree descendants of an element and the element itself.</returns>
        /// <exception cref="ArgumentNullException">The value of 'element' cannot be null. </exception>
        public static IEnumerable<DependencyObject> GetVisualDescendantsAndSelf(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return GetVisualDescendantsAndSelfIterator(element);
        }

        /// <summary>Get the visual tree descendants of an element and the element  itself.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The visual tree descendants of an element and the element itself.</returns>
        private static IEnumerable<DependencyObject> GetVisualDescendantsAndSelfIterator(DependencyObject element)
        {
            var remaining = new Queue<DependencyObject>();
            remaining.Enqueue(element);

            while (remaining.Count > 0)
            {
                DependencyObject obj = remaining.Dequeue();
                yield return obj;

                foreach (DependencyObject child in obj.GetVisualChildren())
                    remaining.Enqueue(child);
            }
        }

        /// <summary>Gets the ancestors of the element, up to the root. </summary>
        /// <param name="node">The element to start from. </param>
        /// <returns>An enumerator of the ancestors. </returns>
        public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
        {
            FrameworkElement parent = node.GetVisualParent();
            while (parent != null)
            {
                yield return parent;
                parent = parent.GetVisualParent();
            }
        }

        /// <summary>Prepends an item to the beginning of an enumeration</summary>
        /// <typeparam name="T">The type of item in the enumeration</typeparam>
        /// <param name="list">The existing enumeration</param>
        /// <param name="head">The item to return before the enumeration</param>
        /// <returns>An enumerator that returns the head, followed by the rest of the list</returns>
        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> list, T head)
        {
            yield return head;
            foreach (T item in list)
                yield return item;
        }

        /// <summary>Gets the VisualStateGroup with the given name, looking up the visual tree</summary>
        /// <param name="root">AssociatedObject to start from</param>
        /// <param name="groupName">Name of the group to look for</param>
        /// <returns>The group, if found, or null</returns>
        public static VisualStateGroup GetVisualStateGroup(this FrameworkElement root, string groupName)
        {
            var selfOrAncestors = root.GetVisualAncestors().PrependWith(root);

            foreach (var element in selfOrAncestors)
            {
                var groups = VisualStateManager.GetVisualStateGroups(element);
                if (groups != null)
                {
                    var group = groups.OfType<VisualStateGroup>().FirstOrDefault(g => g.Name == groupName);
                    if (group != null)
                        return group;
                }
            }

            return null;
        }

        /// <summary>Tests if the given item is visible or not inside a given viewport</summary>
        /// <param name="item">The item to check for visibility</param>
        /// <param name="viewport">The viewport to check visibility within</param>
        /// <param name="orientation">The orientation to check visibility with respect to (vertical or horizontal)</param>
        /// <param name="wantVisible">Whether the test is for being visible or invisible</param>
        /// <returns>True if the item's visibility matches the wantVisible parameter</returns>
        public static bool TestVisibility(this FrameworkElement item, FrameworkElement viewport, Orientation orientation, bool wantVisible)
        {
            GeneralTransform transform = item.TransformToVisual(viewport);

#if WINRT
            Point topLeft = transform.TransformPoint(new Point(0, 0));
            Point bottomRight = transform.TransformPoint(new Point(item.ActualWidth, item.ActualHeight));
#else
            Point topLeft = transform.Transform(new Point(0, 0));
            Point bottomRight = transform.Transform(new Point(item.ActualWidth, item.ActualHeight));
#endif
            double min, max, testMin, testMax;
            if (orientation == Orientation.Vertical)
            {
                min = topLeft.Y;
                max = bottomRight.Y;
                testMin = 0;
                testMax = Math.Min(viewport.ActualHeight, double.IsNaN(viewport.Height) ? double.PositiveInfinity : viewport.Height);
            }
            else
            {
                min = topLeft.X;
                max = bottomRight.X;
                testMin = 0;
                testMax = Math.Min(viewport.ActualWidth, double.IsNaN(viewport.Width) ? double.PositiveInfinity : viewport.Width);
            }

            bool result = wantVisible;
            if (min >= testMax || max <= testMin)
                result = !wantVisible;

            return result;
        }

        /// <summary>Returns the items that are visible in a given container.</summary>
        /// <typeparam name="T">The type of items being tested</typeparam>
        /// <param name="items">The items being tested; typically the children of a StackPanel</param>
        /// <param name="viewport">The viewport to test visibility against; typically a ScrollViewer</param>
        /// <param name="orientation">Whether to check for vertical or horizontal visibility</param>
        /// <returns>The items that are (at least partially) visible</returns>
        /// <remarks>This function assumes that items are ordered top-to-bottom or left-to-right; if items are in random positions it won't work</remarks>
        public static IEnumerable<T> GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation)
            where T : FrameworkElement
        {
            var skippedOverBeforeItems = items.SkipWhile(item => item.TestVisibility(viewport, orientation, false));
            var keepOnlyVisibleItems = skippedOverBeforeItems.TakeWhile(item => item.TestVisibility(viewport, orientation, true));
            return keepOnlyVisibleItems;
        }

        /// <summary>Returns the items that are visible in a given container plus the invisible ones before and after.</summary>
        /// <typeparam name="T">The type of items being tested</typeparam>
        /// <param name="items">The items being tested; typically the children of a StackPanel</param>
        /// <param name="viewport">The viewport to test visibility against; typically a ScrollViewer</param>
        /// <param name="orientation">Wether to check for vertical or horizontal visibility</param>
        /// <param name="beforeItems">List to be populated with items that precede the visible items</param>
        /// <param name="visibleItems">List to be populated with the items that are visible</param>
        /// <param name="afterItems">List to be populated with the items that follow the visible items</param>
        /// <remarks>This function assumes that items are ordered top-to-bottom or left-to-right; if items are in random positions it won't work</remarks>
        public static void GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation, out List<T> beforeItems, out List<T> visibleItems, out List<T> afterItems)
            where T : FrameworkElement
        {
            beforeItems = new List<T>();
            visibleItems = new List<T>();
            afterItems = new List<T>();

            VisibleSearchMode mode = VisibleSearchMode.Before;

            // Use a state machine to go over the enumertaion and populate the lists
            foreach (var item in items)
            {
                switch (mode)
                {
                    case VisibleSearchMode.Before:
                        if (item.TestVisibility(viewport, orientation, false))
                        {
                            beforeItems.Add(item);
                        }
                        else
                        {
                            visibleItems.Add(item);
                            mode = VisibleSearchMode.During;
                        }
                        break;

                    case VisibleSearchMode.During:
                        if (item.TestVisibility(viewport, orientation, true))
                        {
                            visibleItems.Add(item);
                        }
                        else
                        {
                            afterItems.Add(item);
                            mode = VisibleSearchMode.After;
                        }
                        break;

                    default:
                        afterItems.Add(item);
                        break;
                }
            }
        }

        /// <summary>Simple enumeration used in the state machine of GetVisibleItems</summary>
        enum VisibleSearchMode
        {
            Before,
            During,
            After
        }

        /// <summary>Performs a breadth-first enumeration of all the descendents in the tree</summary>
        /// <param name="root">The root node</param>
        /// <returns>An enumerator of all the children</returns>
        public static IEnumerable<FrameworkElement> GetVisualDescendents(this FrameworkElement root)
        {
            Queue<IEnumerable<FrameworkElement>> toDo = new Queue<IEnumerable<FrameworkElement>>();

            toDo.Enqueue(root.GetVisualChildren());
            while (toDo.Count > 0)
            {
                IEnumerable<FrameworkElement> children = toDo.Dequeue();
                foreach (FrameworkElement child in children)
                {
                    yield return child;
                    toDo.Enqueue(child.GetVisualChildren());
                }
            }
        }

        /// <summary>Returns all the descendents of a particular type</summary>
        /// <typeparam name="T">The type to look for</typeparam>
        /// <param name="root">The root element</param>
        /// <param name="allAtSameLevel">Whether to stop searching the tree after the first set of items are found</param>
        /// <returns>List of the element found</returns>
        /// <remarks>
        /// The allAtSameLevel flag is used to control enumeration through the tree. For many cases (eg, finding ListBoxItems in a
        /// ListBox) you want enumeration to stop as soon as you've found all the items in the ListBox (no need to search further
        /// in the tree). For other cases though (eg, finding all the Buttons on a page) you want to exhaustively search the entire tree
        /// </remarks>
        public static IEnumerable<T> GetVisualDescendents<T>(this FrameworkElement root, bool allAtSameLevel) where T : FrameworkElement
        {
            bool found = false;
            foreach (FrameworkElement e in root.GetVisualDescendents())
            {
                if (e is T)
                {
                    found = true;
                    yield return e as T;
                }
                else
                {
                    if (found == true && allAtSameLevel == true)
                        yield break;
                }
            }
        }

        /// <summary>Print the entire visual element tree of an item to the debug console</summary>
        /// <param name="root">The item whose descendents should be printed</param>
        [Conditional("DEBUG")]
        public static void PrintDescendentsTree(this FrameworkElement root)
        {
            List<string> results = new List<string>();
            root.GetChildTree("", "  ", results);
            foreach (string s in results)
                Debug.WriteLine(s);
        }

        /// <summary>Returns a list of descendents, formatted with indentation</summary>
        /// <param name="root">The item whose tree should be returned</param>
        /// <param name="prefix">The prefix for this level of hierarchy</param>
        /// <param name="addPrefix">The string to add for the next level</param>
        /// <param name="results">A list that will contain the items on return</param>
        [Conditional("DEBUG")]
        static void GetChildTree(this FrameworkElement root, string prefix, string addPrefix, List<string> results)
        {
            string thisElement = "";
            if (String.IsNullOrEmpty(root.Name))
                thisElement = "[Anon]";
            else
                thisElement = root.Name;

            thisElement += " " + root.GetType().Name;

            results.Add(prefix + thisElement);
            foreach (FrameworkElement child in root.GetVisualChildren())
            {
                child.GetChildTree(prefix + addPrefix, addPrefix, results);
            }
        }

        /// <summary>Prints the visual ancestor tree for an item to the debug console</summary>
        /// <param name="node">The item whost ancestors you want to print</param>
        [Conditional("DEBUG")]
        public static void PrintAncestorTree(this FrameworkElement node)
        {
            List<string> tree = new List<string>();
            node.GetAncestorVisualTree(tree);
            string prefix = "";
            foreach (string s in tree)
            {
                Debug.WriteLine(prefix + s);
                prefix = prefix + "  ";
            }
        }

        /// <summary>Returns a list of ancestors</summary>
        /// <param name="node">The node whose ancestors you want</param>
        /// <param name="children">A list that will contain the children</param>
        [Conditional("DEBUG")]
        private static void GetAncestorVisualTree(this FrameworkElement node, List<string> children)
        {
            string name = String.IsNullOrEmpty(node.Name) ? "[Anon]" : node.Name;
            string thisNode = name + ": " + node.GetType().Name;
            children.Insert(0, thisNode);
            FrameworkElement parent = node.GetVisualParent();
            if (parent != null)
                GetAncestorVisualTree(parent, children);
        }

        /// <summary>List of work to do on the next render (at the end of the current tick)</summary>
        static List<Action> workItems;

        /// <summary>Schedules work to happen at the end of this tick, when the <see cref="CompositionTarget.Rendering" /> event is raised</summary>
        /// <param name="action">The work to do</param>
        /// <remarks>Typically you can schedule work using Dispatcher.BeginInvoke, but sometimes that will result in a single-frame
        /// glitch of the visual tree. In that case, use this method.</remarks>
        public static void ScheduleOnNextRender(Action action)
        {
            if (workItems == null)
            {
                workItems = new List<Action>();
                CompositionTarget.Rendering += DoWorkOnRender;
            }

            workItems.Add(action);
        }

#if WINRT
        private static void DoWorkOnRender(object sender, object e)
#else
        static void DoWorkOnRender(object sender, EventArgs args)
#endif
        {
            Debug.WriteLine("DoWorkOnRender running... if you see this message a lot then something is wrong!");

            // Remove ourselves from the event and clear the list
            CompositionTarget.Rendering -= DoWorkOnRender;
            List<Action> work = workItems;
            workItems = null;

            foreach (Action action in work)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();

                    Debug.WriteLine("Exception while doing work for " + action + ". " + ex.Message);
                }
            }
        }
    }

    [Flags]
    public enum TransformCreationMode
    {
        /// <summary>Don't try and create a transform if it doesn't already exist</summary>
        None = 0,

        /// <summary>Create a transform if none exists</summary>
        Create = 1,

        /// <summary>Create and add to an existing group</summary>
        AddToGroup = 2,

        /// <summary>Create a group and combine with existing transform; may break existing animations</summary>
        CombineIntoGroup = 4,

        /// <summary>Treat identity matrix as if it wasn't there; may break existing animations</summary>
        IgnoreIdentityMatrix = 8,

        /// <summary>Create a new transform or add to group</summary>
        CreateOrAddAndIgnoreMatrix = Create | AddToGroup | IgnoreIdentityMatrix,

        /// <summary>Default behavior, equivalent to CreateOrAddAndIgnoreMatrix</summary>
        Default = CreateOrAddAndIgnoreMatrix,
    }
}
