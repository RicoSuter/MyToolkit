using System;
using System.Windows;
using System.Windows.Navigation;
using System.Threading.Tasks;
using MyToolkit.Paging;

namespace MyToolkit.Animation.Transitions
{
    public class AnimatedPage : ExtendedPage
    {
		public bool ForwardInAnimationOnFirstPageEnabled { get; set; }
		public bool TransitionAnimationsEnabled { get; set; }

		private bool _isNavigating;
		private bool _isForwardNavigation;
		private AnimationType _currentAnimationType;

		private static readonly Uri ExternalUri = new Uri(@"app://external/");

		private bool CanAnimate()
		{
			return !_isNavigating && AnimationContext != null;
		}

        public AnimatedPage()
        {
			TransitionAnimationsEnabled = true;
			ForwardInAnimationOnFirstPageEnabled = false;

			_isForwardNavigation = true;
		}

		protected virtual void OnAnimationCompleted(AnimationType animationType) { }
		
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

		private async void OnPageLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnPageLoaded;

			if (_isForwardNavigation)
				_currentAnimationType = AnimationType.NavigateForwardIn;
			else
				_currentAnimationType = AnimationType.NavigateBackwardIn;
			_isForwardNavigation = false;

			if (CanAnimate())
				await RunAnimationAsync();
			else
			{
				if (AnimationContext != null)
					AnimationContext.Opacity = 1;
				OnAnimationCompleted(_currentAnimationType);
			}
		}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
	        if (State.ContainsKey("AnimatedPageNavigatedToLibraryPage"))
		        _navigatedToLibraryPage = (bool) State["AnimatedPageNavigatedToLibraryPage"];

#if WP7
			if (e.NavigationMode != NavigationMode.Refresh && !_navigatedToLibraryPage)
#else 
			if (e.NavigationMode != NavigationMode.Reset && e.NavigationMode != NavigationMode.Refresh && !_navigatedToLibraryPage) 
#endif
			{
				_isNavigating = false;

				var isFirstForwardIn = e.NavigationMode == NavigationMode.New && !NavigationService.CanGoBack;
				if ((e.IsNavigationInitiator || isFirstForwardIn) && // show transition only if not navigating from other app except if first launch
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
					_isForwardNavigation = false;
				}
			}
			else if (AnimationContext != null)
				AnimationContext.Opacity = 1; 
		
			base.OnNavigatedTo(e);
        }

	    private bool _navigatedToLibraryPage = false; 
        protected override async Task OnNavigatingFromAsync(NavigatingCancelEventArgs e)
		{
			_navigatedToLibraryPage = e.Uri.ToString().Contains(";component/");
	        State["AnimatedPageNavigatedToLibraryPage"] = _navigatedToLibraryPage;

			// Only for app pages (no external and library pages)
			// needed for correctly working phone toolkit (e.g. date picker)

#if WP7
			if (!_isNavigating && e.Uri != ExternalUri && !_navigatedToLibraryPage && 
				e.NavigationMode != NavigationMode.Refresh)
#else
			if (!_isNavigating && e.Uri != ExternalUri && !_navigatedToLibraryPage &&
				e.NavigationMode != NavigationMode.Reset && e.NavigationMode != NavigationMode.Refresh)
#endif
			{
				_isNavigating = true;
				switch (e.NavigationMode)
				{
					case NavigationMode.New:
						_currentAnimationType = AnimationType.NavigateForwardOut;
						break;

					case NavigationMode.Back:
						_currentAnimationType = AnimationType.NavigateBackwardOut;
						break;

					case NavigationMode.Forward:
						_currentAnimationType = AnimationType.NavigateForwardOut;
						break;
				}

				await RunAnimationAsync();
			}
        }

		private async Task RunAnimationAsync()
        {
			AnimatorHelperBase animation = null;
			switch (_currentAnimationType)
            {
                case AnimationType.NavigateForwardIn:
                    animation = GetAnimation(_currentAnimationType);
                    break;
                case AnimationType.NavigateBackwardOut:
                    animation = GetAnimation(_currentAnimationType);
                    break;
                default:
                    animation = GetAnimation(_currentAnimationType);
                    break;
            }

			if (animation == null)
			{
				AnimationContext.Opacity = 1;
				OnAnimationCompleted(_currentAnimationType);
			}
			else
			{
				var task = new TaskCompletionSource<object>();
				Dispatcher.BeginInvoke(() =>
				{
					AnimationContext.Opacity = 1;
					animation.Begin(() => Dispatcher.BeginInvoke(() =>
					{
						OnAnimationCompleted(_currentAnimationType);
						task.SetResult(null);
					}));
				});
				await task.Task;
			}
        }

		public static readonly DependencyProperty AnimationContextProperty =
			DependencyProperty.Register("AnimationContext", typeof(FrameworkElement), typeof(AnimatedPage), new PropertyMetadata(null));

		public FrameworkElement AnimationContext
		{
			get
			{
				var animationContext = (FrameworkElement)GetValue(AnimationContextProperty);
				if (animationContext == null)
					animationContext = (FrameworkElement)Content;
				return animationContext;
			}
			set { SetValue(AnimationContextProperty, value); }
		}
    }
}