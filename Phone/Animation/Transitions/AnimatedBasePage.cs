using MyToolkit.Paging;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace MyToolkit.Animation.Transitions
{
    public class AnimatedBasePage : PhoneApplicationPage
    {
		public bool ForwardInAnimationOnFirstPageEnabled { get; set; }
		public bool TransitionAnimationsEnabled { get; set; }

		private static readonly Uri ExternalUri = new Uri(@"app://external/");
		
		public static readonly DependencyProperty AnimationContextProperty =
            DependencyProperty.Register("AnimationContext", typeof(FrameworkElement), typeof(AnimatedBasePage), new PropertyMetadata(null));
        
		public FrameworkElement AnimationContext
        {
            get { return (FrameworkElement)GetValue(AnimationContextProperty); }
            set { SetValue(AnimationContextProperty, value); }
        }        
		
		private bool isNavigating;
		private Uri nextUri;
		
		private AnimationType currentAnimationType;
        private bool isForwardNavigation;
		
        public AnimatedBasePage()
        {
			ForwardInAnimationOnFirstPageEnabled = false; 
	        TransitionAnimationsEnabled = true;
			isForwardNavigation = true;
        }

        protected virtual void AnimationsComplete(AnimationType animationType)
        {

        }

        protected virtual AnimatorHelperBase GetAnimation(AnimationType animationType)
        {
            AnimatorHelperBase animation;
			switch (animationType)
            {
                case AnimationType.NavigateBackwardIn:
                    animation = new TurnstileBackwardInAnimator();
                    break;

                case AnimationType.NavigateBackwardOut:
                    animation = new TurnstileBackwardOutAnimator();
                    break;

                case AnimationType.NavigateForwardIn:
                    animation = new TurnstileForwardInAnimator();
                    break;

                default:
                    animation = new TurnstileForwardOutAnimator();
                    break;
            }
			animation.RootElement = AnimationContext;
            return animation;
        }

		private void OnPageLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnPageLoaded;

			if (isForwardNavigation)
				currentAnimationType = AnimationType.NavigateForwardIn;
			else
				currentAnimationType = AnimationType.NavigateBackwardIn;

			if (CanAnimate())
				RunAnimation(null);
			else
			{
				if (AnimationContext != null)
					AnimationContext.Opacity = 1;
				OnTransitionAnimationCompleted(null);
			}

			if (isForwardNavigation)
				isForwardNavigation = false;
		}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
	        if (State.ContainsKey("AnimatedBasePageNavigatedToLibraryPage"))
		        navigatedToLibraryPage = (bool) State["AnimatedBasePageNavigatedToLibraryPage"];

			if (e.NavigationMode != NavigationMode.Reset && e.NavigationMode != NavigationMode.Refresh && !navigatedToLibraryPage) // see explanation below
	        {
				if (AnimationContext == null)
					AnimationContext = (FrameworkElement)Content;

				isNavigating = false;

				var isFirstForwardIn = e.NavigationMode == NavigationMode.New && !NavigationService.CanGoBack;
				if ((e.IsNavigationInitiator || isFirstForwardIn) && // <- show transition only if not navigating from other app except if first launch
					TransitionAnimationsEnabled && (ForwardInAnimationOnFirstPageEnabled || !isFirstForwardIn))
				{
					Loaded += OnPageLoaded;
					if (AnimationContext != null)
						AnimationContext.Opacity = 0;
				}
				else
				{
					if (AnimationContext != null)
						AnimationContext.Opacity = 1;
					if (isForwardNavigation)
						isForwardNavigation = false;
				}
			}
		
			base.OnNavigatedTo(e);
		}

	    private bool navigatedToLibraryPage = false; 
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

			navigatedToLibraryPage = e.Uri.ToString().Contains(";component/");
	        State["AnimatedBasePageNavigatedToLibraryPage"] = navigatedToLibraryPage;

			// only for app pages (no external and library pages)
			// needed for correctly working phone toolkit (eg date picker)
			if (!isNavigating && e.Uri != ExternalUri && !navigatedToLibraryPage)
			{
				isNavigating = true;
				nextUri = e.Uri;
				e.Cancel = true;

				switch (e.NavigationMode)
				{
					case NavigationMode.New:
						currentAnimationType = AnimationType.NavigateForwardOut;
						break;

					case NavigationMode.Back:
						currentAnimationType = AnimationType.NavigateBackwardOut;
						break;

					case NavigationMode.Forward:
						currentAnimationType = AnimationType.NavigateForwardOut;
						break;
				}

				RunAnimation(e.NavigationMode);
			}
        }

        private bool CanAnimate()
        {
            return !isNavigating && AnimationContext != null;
        }

        private void RunAnimation(NavigationMode? navigationMode)
        {
			AnimatorHelperBase animation = null;
			switch (currentAnimationType)
            {
                case AnimationType.NavigateForwardIn:
                    animation = GetAnimation(currentAnimationType);
                    break;
                case AnimationType.NavigateBackwardOut:
                    animation = GetAnimation(currentAnimationType);
                    break;
                default:
                    animation = GetAnimation(currentAnimationType);
                    break;
            }

			if (animation == null)
			{
				AnimationContext.Opacity = 1;
				OnTransitionAnimationCompleted(navigationMode);
			}
			else
			{
				Dispatcher.BeginInvoke(() =>
				{
					AnimationContext.Opacity = 1;
					animation.Begin(() => OnTransitionAnimationCompleted(navigationMode));
				});
			}
        }

		private void OnTransitionAnimationCompleted(NavigationMode? navigationMode)
		{
			if (navigationMode.HasValue)
			{
				Dispatcher.BeginInvoke(() =>
				{
					switch (navigationMode)
					{
						case NavigationMode.Forward:
							if (PhonePage.CurrentPage.NavigationService.CanGoForward)
								PhonePage.CurrentPage.NavigationService.GoForward();
							break;

						case NavigationMode.Back:
							if (PhonePage.CurrentPage.NavigationService.CanGoBack)
								PhonePage.CurrentPage.NavigationService.GoBack();
							break;

						case NavigationMode.New:
							PhonePage.CurrentPage.NavigationService.Navigate(nextUri);
							break;
					}
				});
			}
			AnimationsComplete(currentAnimationType);
		}
    }
}