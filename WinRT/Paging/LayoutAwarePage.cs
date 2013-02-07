using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Paging
{
	[Windows.Foundation.Metadata.WebHostHidden]
	public class LayoutAwarePage : ExtendedPage
    {
        private List<Control> layoutAwareControls;

		public LayoutAwarePage()
		{
            Loaded += (sender, e) => StartLayoutUpdates(InternalPage, e);
            Unloaded += (sender, e) => StopLayoutUpdates(InternalPage, e);
        }

        public void StartLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
           
			if (layoutAwareControls == null)
            {
                Window.Current.SizeChanged += WindowSizeChanged;
                layoutAwareControls = new List<Control>();
            }
            layoutAwareControls.Add(control);

            VisualStateManager.GoToState(control, DetermineVisualState(ApplicationView.Value), false);
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            InvalidateVisualState();
        }

        public void StopLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null || layoutAwareControls == null) 
				return;

            layoutAwareControls.Remove(control);
            if (layoutAwareControls.Count == 0)
            {
                layoutAwareControls = null;
                Window.Current.SizeChanged -= WindowSizeChanged;
            }
        }

        protected virtual string DetermineVisualState(ApplicationViewState viewState)
        {
            return viewState.ToString();
        }

        public void InvalidateVisualState()
        {
            if (layoutAwareControls != null)
            {
                var visualState = DetermineVisualState(ApplicationView.Value);
                foreach (var layoutAwareControl in layoutAwareControls)
                    VisualStateManager.GoToState(layoutAwareControl, visualState, false);
            }
        }
    }
}
