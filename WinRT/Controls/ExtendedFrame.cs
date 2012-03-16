using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Controls
{
	public sealed class ExtendedFrame : Control
	{
		public ExtendedFrame()
		{
			DefaultStyleKey = typeof(ExtendedFrame);
			PageStack = new Stack<object>();
		}

		private ContentControl contentControl;
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			contentControl = (ContentControl)GetTemplateChild("content");
			if (nextType != null)
				Navigate(nextType, nextParameter); 
		}

		public bool CanGoBack { get { return PageStack.Count > 1; } }
		public object Content { get { return PageStack.Count == 0 ? null : PageStack.Peek(); } }

		public Stack<object> PageStack { get; private set; }

		public event NavigatedEventHandler Navigated;

		public void GoBack()
		{
			PageStack.Pop();

			var page = PageStack.Peek();
			var args = new NavigationEventArgs(page, null, page.GetType(), null, NavigationMode.Back);

			if (page is Page)
			{
				var method = typeof(Page).GetTypeInfo().GetDeclaredMethod("OnNavigatedTo");
				method.Invoke(page, new object[] { args });
			}
			contentControl.Content = page;

			if (Navigated != null)
				Navigated(this, args);
		}

		private Type nextType;
		private object nextParameter; 
		public void Navigate(Type type, object parameter)
		{
			if (contentControl == null)
			{
				if (nextType != null)
					throw new Exception();

				nextType = type;
				nextParameter = parameter;
				return; 
			}

			var page = (Control)Activator.CreateInstance(type);
			var args = new NavigationEventArgs(page, parameter, type, null, NavigationMode.New);
		
			PageStack.Push(page);
			contentControl.Content = page;

			if (page is Page)
			{
				var method = typeof(Page).GetTypeInfo().GetDeclaredMethod("OnNavigatedTo");
				method.Invoke(page, new object[] { args });
			}

			if (Navigated != null)
				Navigated(this, args);
		}
	}
}
