//-----------------------------------------------------------------------
// <copyright file="TreeViewExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Diagnostics.Contracts;
using System.Reflection;
using System.Windows.Controls;

namespace MyToolkit.Controls
{
    /// <summary>Extension methods for TreeView controls.</summary>
    public static class TreeViewExtensions
    {
        /// <summary>Expands all TreeView items.</summary>
        /// <param name="treeView">The tree view.</param>
        public static void ExpandAll(this TreeView treeView)
        {
            Contract.Requires(treeView != null);

            SetExpandedAll(treeView, true);
        }

        /// <summary>Expands a given item.</summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="item">The item to expand.</param>
        public static void ExpandItem(this TreeView treeView, object item)
        {
            Contract.Requires(treeView != null);
            Contract.Requires(item != null);

            var container = ContainerFromItem(treeView, item);
            if (container != null)
                container.IsExpanded = true;
        }

        /// <summary>Collapses all TreeView items.</summary>
        /// <param name="treeView">The tree view.</param>
        public static void CollapseAll(this TreeView treeView)
        {
            Contract.Requires(treeView != null);

            SetExpandedAll(treeView, false);
        }

        /// <summary>Selects the given item in the three view</summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="item">The item to select.</param>
        public static void SetSelectedItem(this TreeView treeView, object item)
        {
            Contract.Requires(treeView != null);

            if (treeView.SelectedItem == item)
                return;

            if (item == null)
            {
                DeselectItem(treeView);
                return;
            }

            var container = ContainerFromItem(treeView, item);
            if (container != null)
            {
                container.IsSelected = true;

                var selectMethod = typeof(TreeViewItem).GetMethod("Select", BindingFlags.NonPublic | BindingFlags.Instance);
                selectMethod.Invoke(container, new object[] { true });
            }
            else
                DeselectItem(treeView);
        }

        /// <summary>Removes the selection from the TreeView.</summary>
        /// <param name="treeView">The tree view.</param>
        public static void DeselectItem(this TreeView treeView)
        {
            Contract.Requires(treeView != null);

            if (treeView.ItemContainerGenerator.Items.Count == 0 || treeView.ItemContainerGenerator.Items[0] == null)
                return;

            var item = treeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                item.IsSelected = false;
            }
        }

        private static TreeViewItem ContainerFromItem(TreeView treeView, object item)
        {
            Contract.Requires(treeView != null);
            Contract.Requires(item != null);

            var containerThatMightContainItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item);

            if (containerThatMightContainItem != null)
                return containerThatMightContainItem;

            return ContainerFromItem(treeView.ItemContainerGenerator, treeView.Items, item);
        }

        private static TreeViewItem ContainerFromItem(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, object item)
        {
            Contract.Requires(parentItemContainerGenerator != null);
            Contract.Requires(itemCollection != null);
            Contract.Requires(item != null);

            foreach (object curChildItem in itemCollection)
            {
                var parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);

                if (parentContainer != null)
                {
                    var containerThatMightContainItem =
                        (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);

                    if (containerThatMightContainItem != null)
                        return containerThatMightContainItem;

                    TreeViewItem recursionResult = ContainerFromItem(
                        parentContainer.ItemContainerGenerator,
                        parentContainer.Items,
                        item);

                    if (recursionResult != null)
                        return recursionResult;
                }
            }

            return null;
        }

        private static void SetExpandedAll(TreeView treeView, bool expand)
        {
            foreach (object item in treeView.Items)
            {
                var treeItem = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                {
                    SetExpandedAll(treeItem, expand);
                    treeItem.IsExpanded = expand;
                }
            }
        }

        private static void SetExpandedAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                var childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                    SetExpandedAll(childControl, expand);

                var item = childControl as TreeViewItem;
                if (item != null)
                    item.IsExpanded = expand;
            }
        }
    }
}
