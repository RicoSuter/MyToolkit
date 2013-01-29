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
		private NavigationMode? currentNavigationMode;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
	        isNavigating = false; 
			currentNavigationMode = null;

			if (nextUri != ExternalUri && TransitionAnimationsEnabled && (ForwardInAnimationOnFirstPageEnabled || !isForwardNavigation || NavigationService.CanGoBack))
			{
				Loaded += OnPageLoaded;
				if (AnimationContext != null)
					AnimationContext.Opacity = 0;
			}
			else if (isForwardNavigation)
				isForwardNavigation = false;
		
			base.OnNavigatedTo(e);
		}

		private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
			Loaded -= OnPageLoaded;

			if (isForwardNavigation)
				currentAnimationType = AnimationType.NavigateForwardIn;
			else
				currentAnimationType = AnimationType.NavigateBackwardIn;

			if (CanAnimate())
				RunAnimation();
			else
			{
				if (AnimationContext != null)
					AnimationContext.Opacity = 1;
				OnTransitionAnimationCompleted();
			}

			if (isForwardNavigation)
				isForwardNavigation = false;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

			if (!isNavigating)
			{
				isNavigating = true; 
				e.Cancel = true;
				nextUri = e.Uri;

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

				currentNavigationMode = e.NavigationMode;
				if (e.Uri != ExternalUri)
					RunAnimation();
			}
        }

        private bool CanAnimate()
        {
            return !isNavigating && AnimationContext != null;
        }

        private void RunAnimation()
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
				OnTransitionAnimationCompleted();
			}
			else
			{
				Dispatcher.BeginInvoke(() =>
				{
					AnimationContext.Opacity = 1;
					animation.Begin(OnTransitionAnimationCompleted);
				});
			}
        }

		private void OnTransitionAnimationCompleted()
		{
			try
			{
				Dispatcher.BeginInvoke(() =>
				{
					switch (currentNavigationMode)
					{
						case NavigationMode.Forward:
							PhonePage.CurrentPage.NavigationService.GoForward();
							break;

						case NavigationMode.Back:
							PhonePage.CurrentPage.NavigationService.GoBack();
							break;

						case NavigationMode.New:
							PhonePage.CurrentPage.NavigationService.Navigate(nextUri);
							break;
					}
				});
			}
			catch (Exception ex)
			{
				Debug.WriteLine("OnTransitionAnimationCompleted Exception on {0}: {1}", this, ex);
			}
			AnimationsComplete(currentAnimationType);
		}
    }
}