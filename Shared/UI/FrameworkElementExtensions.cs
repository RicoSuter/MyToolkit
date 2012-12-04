using System;

#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#endif

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Diagnostics;


namespace MyToolkit.UI
{
	public static class FrameworkElementExtensions
	{
		public static object FindParentDataContext(this FrameworkElement elem)
		{
			if (elem.DataContext != null)
				return elem.DataContext;
			if (elem.Parent != null && elem.Parent is FrameworkElement)
				return FindParentDataContext((FrameworkElement)elem.Parent);
			return null;
		}

		public static Rect GetElementRect(this FrameworkElement element)
		{
			var transform = element.TransformToVisual(null);
			var point = transform.TransformPoint(new Point());
			return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
		}


		/// <summary>
		/// Equivalent of FindName, but works on the visual tree to go through templates, etc.
		/// </summary>
		/// <param name="root">The node to search from</param>
		/// <param name="name">The name to look for</param>
		/// <returns>The found node, or null if not found</returns>
		public static FrameworkElement FindVisualChild(this FrameworkElement root, string name)
		{
			var temp = root.FindName(name) as FrameworkElement;
			if (temp != null)
				return temp;

			foreach (FrameworkElement element in root.GetVisualDescendents())
			{
				temp = element.FindName(name) as FrameworkElement;
				if (temp != null)
					return temp;
			}

			return null;
		}

		/// <summary>
		/// Returns the visual parent of an element
		/// </summary>
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

		/// <summary>
		/// Returns a visual child of an element
		/// </summary>
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

		/// <summary>
		/// Gets all the visual children of the element
		/// </summary>
		/// <param name="root">The element to get children of</param>
		/// <returns>An enumerator of the children</returns>
		public static IEnumerable<FrameworkElement> GetVisualChildren(this FrameworkElement root)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
				yield return VisualTreeHelper.GetChild(root, i) as FrameworkElement;
		}


		/// <summary>
		/// Gets the ancestors of the element, up to the root
		/// </summary>
		/// <param name="node">The element to start from</param>
		/// <returns>An enumerator of the ancestors</returns>
		public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
		{
			FrameworkElement parent = node.GetVisualParent();
			while (parent != null)
			{
				yield return parent;
				parent = parent.GetVisualParent();
			}
		}

		/// <summary>
		/// Prepends an item to the beginning of an enumeration
		/// </summary>
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

		/// <summary>
		/// Gets the VisualStateGroup with the given name, looking up the visual tree
		/// </summary>
		/// <param name="root">Element to start from</param>
		/// <param name="groupName">Name of the group to look for</param>
		/// <returns>The group, if found, or null</returns>
		public static VisualStateGroup GetVisualStateGroup(this FrameworkElement root, string groupName)
		{
			IEnumerable<FrameworkElement> selfOrAncestors = root.GetVisualAncestors().PrependWith(root);

			foreach (FrameworkElement element in selfOrAncestors)
			{
				var groups = VisualStateManager.GetVisualStateGroups(element);
				foreach (object o in groups)
				{
					VisualStateGroup group = o as VisualStateGroup;
					if (group != null && group.Name == groupName)
						return group;
				}
			}

			return null;
		}

		/// <summary>
		/// Tests if the given item is visible or not inside a given viewport
		/// </summary>
		/// <param name="item">The item to check for visibility</param>
		/// <param name="viewport">The viewport to check visibility within</param>
		/// <param name="orientation">The orientation to check visibility with respect to (vertical or horizontal)</param>
		/// <param name="wantVisible">Whether the test is for being visible or invisible</param>
		/// <returns>True if the item's visibility matches the wantVisible parameter</returns>
		public static bool TestVisibility(this FrameworkElement item, FrameworkElement viewport, Orientation orientation, bool wantVisible)
		{
			// Determine the bounding box of the item relative to the viewport
			GeneralTransform transform = item.TransformToVisual(viewport);

#if WINRT
			Point topLeft = transform.TransformPoint(new Point(0, 0));
			Point bottomRight = transform.TransformPoint(new Point(item.ActualWidth, item.ActualHeight));
#else
			Point topLeft = transform.Transform(new Point(0, 0));
			Point bottomRight = transform.Transform(new Point(item.ActualWidth, item.ActualHeight));
#endif
			// Check for overlapping bounding box of the item vs. the viewport, depending on orientation
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

#if false
    // Enable this to help with debugging if you are having issues...
    Debug.WriteLine(String.Format("Test visibility of {0}-{1} inside {2}-{3}. wantVisible {4}, result {5}",
    min, max, testMin, testMax, wantVisible, result));
#endif

			return result;
		}

		/// <summary>
		/// Returns the items that are visible in a given container.
		/// </summary>
		/// <remarks>This function assumes that items are ordered top-to-bottom or left-to-right; if items are in random positions it won't work</remarks>
		/// <typeparam name="T">The type of items being tested</typeparam>
		/// <param name="items">The items being tested; typically the children of a StackPanel</param>
		/// <param name="viewport">The viewport to test visibility against; typically a ScrollViewer</param>
		/// <param name="orientation">Whether to check for vertical or horizontal visibility</param>
		/// <returns>The items that are (at least partially) visible</returns>
		public static IEnumerable<T> GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation) where T : FrameworkElement
		{
			// Skip over the non-visible items, then take the visible items
			var skippedOverBeforeItems = items.SkipWhile((item) => item.TestVisibility(viewport, orientation, false));
			var keepOnlyVisibleItems = skippedOverBeforeItems.TakeWhile((item) => item.TestVisibility(viewport, orientation, true));
			return keepOnlyVisibleItems;
		}

		/// <summary>
		/// Returns the items that are visible in a given container plus the invisible ones before and after.
		/// </summary>
		/// <remarks>This function assumes that items are ordered top-to-bottom or left-to-right; if items are in random positions it won't work</remarks>
		/// <typeparam name="T">The type of items being tested</typeparam>
		/// <param name="items">The items being tested; typically the children of a StackPanel</param>
		/// <param name="viewport">The viewport to test visibility against; typically a ScrollViewer</param>
		/// <param name="orientation">Wether to check for vertical or horizontal visibility</param>
		/// <param name="beforeItems">List to be populated with items that precede the visible items</param>
		/// <param name="visibleItems">List to be populated with the items that are visible</param>
		/// <param name="afterItems">List to be populated with the items that follow the visible items</param>
		public static void GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation, out List<T> beforeItems, out List<T> visibleItems, out List<T> afterItems) where T : FrameworkElement
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

		/// <summary>
		/// Simple enumeration used in the state machine of GetVisibleItems
		/// </summary>
		enum VisibleSearchMode
		{
			Before,
			During,
			After
		}


		/// <summary>
		/// Performs a breadth-first enumeration of all the descendents in the tree
		/// </summary>
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

		/// <summary>
		/// Returns all the descendents of a particular type
		/// </summary>
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

		/// <summary>
		/// Print the entire visual element tree of an item to the debug console
		/// </summary>
		/// <param name="root">The item whose descendents should be printed</param>
		[Conditional("DEBUG")]
		public static void PrintDescendentsTree(this FrameworkElement root)
		{
			List<string> results = new List<string>();
			root.GetChildTree("", "  ", results);
			foreach (string s in results)
				Debug.WriteLine(s);
		}

		/// <summary>
		/// Returns a list of descendents, formatted with indentation
		/// </summary>
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

		/// <summary>
		/// Prints the visual ancestor tree for an item to the debug console
		/// </summary>
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

		/// <summary>
		/// Returns a list of ancestors
		/// </summary>
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

		/// <summary>
		/// Gets the vertical offset for a ListBox
		/// </summary>
		/// <param name="list">The ListBox to check</param>
		/// <returns>The vertical offset</returns>
		public static double GetVerticalScrollOffset(this ListBox list)
		{
			ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
			return viewer.VerticalOffset;
		}

		/// <summary>
		/// Gets the horizontal offset for a ListBox
		/// </summary>
		/// <param name="list">The ListBox to check</param>
		/// <returns>The horizontal offset</returns>
		public static double GetHorizontalScrollOffset(this ListBox list)
		{
			ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
			return viewer.HorizontalOffset;
		}

			

		/// <summary>
		/// List of work to do on the next render (at the end of the current tick)
		/// </summary>
		static List<Action> workItems;

		/// <summary>
		/// Schedules work to happen at the end of this tick, when the <see cref="CompositionTarget.Rendering"/> event is raised
		/// </summary>
		/// <param name="action">The work to do</param>
		/// <remarks>
		/// Typically you can schedule work using Dispatcher.BeginInvoke, but sometimes that will result in a single-frame
		/// glitch of the visual tree. In that case, use this method.
		/// </remarks>
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
}
