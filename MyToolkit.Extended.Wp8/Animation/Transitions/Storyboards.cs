// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueExpiry.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
// </summary>
// <credits>
// Kevin Marshall http://blogs.claritycon.com/blog/2010/10/13/wp7-page-transitions-sample/
// </credits>
// --------------------------------------------------------------------------------------------------------------------

namespace MyToolkit.Animation.Transitions
{
    /// <summary>
    /// The storyboards.
    /// </summary>
    public class Storyboards
    {
        #region Constants and Fields

        /// <summary>
        /// The continuum backward in storyboard.
        /// </summary>
        internal static readonly string ContinuumBackwardInStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateX)"" Storyboard.TargetName=""ContinuumElement"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""-70""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseOut"" Exponent=""3""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" Storyboard.TargetName=""ContinuumElement"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""-30""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseOut"" Exponent=""3""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
			<DoubleAnimationUsingKeyFrames Storyboard.TargetName=""ContinuumElement"" Storyboard.TargetProperty=""(UIElement.Opacity)"">
				<DiscreteDoubleKeyFrame KeyTime=""0:0:0"" Value=""1"" />
			</DoubleAnimationUsingKeyFrames>
	        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"" Storyboard.TargetName=""LayoutRoot"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""1"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseOut"" Exponent=""6""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
            <DoubleAnimation Duration=""0"" To=""0"" Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" Storyboard.TargetName=""LayoutRoot""/>
        </Storyboard>";

        /// <summary>
        /// The continuum backward out storyboard.
        /// </summary>
        internal static readonly string ContinuumBackwardOutStoryboard =
            @"<Storyboard  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" 
                                           Storyboard.TargetName=""LayoutRoot"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""50"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseIn"" Exponent=""6""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimation Storyboard.TargetProperty=""(UIElement.Opacity)"" From=""1"" To=""0"" Duration=""0:0:0.15"" 
                                 Storyboard.TargetName=""LayoutRoot"">
                <DoubleAnimation.EasingFunction>
                    <ExponentialEase EasingMode=""EaseIn"" Exponent=""6""/>
                </DoubleAnimation.EasingFunction>
            </DoubleAnimation>
        </Storyboard>";

        /// <summary>
        /// The continuum forward in storyboard.
        /// </summary>
        internal static readonly string ContinuumForwardInStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" Storyboard.TargetName=""LayoutRoot"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""50""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseOut"" Exponent=""3""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" Storyboard.TargetName=""ContinuumElement"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""-70""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseOut"" Exponent=""3""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateX)"" Storyboard.TargetName=""ContinuumElement"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""130""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseOut"" Exponent=""3""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimation Storyboard.TargetProperty=""(UIElement.Opacity)"" From=""0"" To=""1"" Duration=""0:0:0.15"" 
                                 Storyboard.TargetName=""LayoutRoot"">
                <DoubleAnimation.EasingFunction>
                    <ExponentialEase EasingMode=""EaseOut"" Exponent=""6""/>
                </DoubleAnimation.EasingFunction>
            </DoubleAnimation>
        </Storyboard>";

        /// <summary>
        /// The continuum forward out storyboard.
        /// </summary>
        internal static readonly string ContinuumForwardOutStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" Storyboard.TargetName=""LayoutRoot"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""70"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseIn"" Exponent=""3""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
	        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"" Storyboard.TargetName=""LayoutRoot"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""1""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""0"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseIn"" Exponent=""3""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" Storyboard.TargetName=""ContinuumElement"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""73"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseIn"" Exponent=""3""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateX)"" Storyboard.TargetName=""ContinuumElement"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""225"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseIn"" Exponent=""3""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
			<DoubleAnimationUsingKeyFrames Storyboard.TargetName=""ContinuumElement"" Storyboard.TargetProperty=""(UIElement.Opacity)"">
				<DiscreteDoubleKeyFrame KeyTime=""0:0:0.15"" Value=""0"" />
			</DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        /// <summary>
        /// The default storyboard.
        /// </summary>
        internal static readonly string DefaultStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimation Duration=""0"" To=""1"" Storyboard.TargetProperty=""(UIElement.Opacity)"" Storyboard.TargetName=""LayoutRoot"" />
        </Storyboard>";

        /// <summary>
        /// The rotate left storyboard.
        /// </summary>
        internal static readonly string RotateLeftStoryboard = string.Empty;

        /// <summary>
        /// The rotate right storyboard.
        /// </summary>
        internal static readonly string RotateRightStoryboard = string.Empty;

        /// <summary>
        /// The slide down fade out storyboard.
        /// </summary>
        internal static readonly string SlideDownFadeOutStoryboard =
            @"
        <Storyboard  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" 
                                           Storyboard.TargetName=""LayoutRoot"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""150"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseIn"" Exponent=""6""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimation Storyboard.TargetProperty=""(UIElement.Opacity)"" From=""1"" To=""0"" Duration=""0:0:0.25"" 
                                 Storyboard.TargetName=""LayoutRoot"">
                <DoubleAnimation.EasingFunction>
                    <ExponentialEase EasingMode=""EaseIn"" Exponent=""6""/>
                </DoubleAnimation.EasingFunction>
            </DoubleAnimation>
        </Storyboard>";

        /// <summary>
        /// The slide left fade in storyboard.
        /// </summary>
        internal static readonly string SlideLeftFadeInStoryboard =
        @"<Storyboard
        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateX)"" Storyboard.TargetName=""LayoutRoot"">
            <EasingDoubleKeyFrame KeyTime=""0"" Value=""200"">
                <EasingDoubleKeyFrame.EasingFunction>
                    <ExponentialEase EasingMode=""EaseOut"" Exponent=""15""/>
                </EasingDoubleKeyFrame.EasingFunction>
            </EasingDoubleKeyFrame>
            <EasingDoubleKeyFrame KeyTime=""0:0:0.5"" Value=""0""/>
        </DoubleAnimationUsingKeyFrames>
        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"">
            <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                <EasingDoubleKeyFrame.EasingFunction>
                    <ExponentialEase EasingMode=""EaseOut"" Exponent=""15""/>
                </EasingDoubleKeyFrame.EasingFunction>
            </EasingDoubleKeyFrame>
            <EasingDoubleKeyFrame KeyTime=""0:0:0.5"" Value=""1""/>
        </DoubleAnimationUsingKeyFrames>
    </Storyboard>";

        /// <summary>
        /// The slide left fade out storyboard.
        /// </summary>
        internal static readonly string SlideLeftFadeOutStoryboard =
            @"<Storyboard
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateX)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""15""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.3"" Value=""-200""/>
                </DoubleAnimationUsingKeyFrames>
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""1"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""15""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.3"" Value=""0""/>
                </DoubleAnimationUsingKeyFrames>
            </Storyboard>";

        /// <summary>
        /// The slide left storyboard.
        /// </summary>
        internal static readonly string SlideLeftStoryboard = string.Empty;

        /// <summary>
        /// The slide right fade in storyboard.
        /// </summary>
        internal static readonly string SlideRightFadeInStoryboard =
            @"<Storyboard
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
               <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateX)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""-200"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""15""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.5"" Value=""0""/>
                </DoubleAnimationUsingKeyFrames>
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""15""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.5"" Value=""1""/>
                </DoubleAnimationUsingKeyFrames>
            </Storyboard>";

        /// <summary>
        /// The slide right fade out storyboard.
        /// </summary>
        internal static readonly string SlideRightFadeOutStoryboard =
            @"<Storyboard
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
               <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateX)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""15""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.3"" Value=""200""/>
                </DoubleAnimationUsingKeyFrames>
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""1"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""15""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.3"" Value=""0""/>
                </DoubleAnimationUsingKeyFrames>
            </Storyboard>";

        /// <summary>
        /// The slide right storyboard.
        /// </summary>
        internal static readonly string SlideRightStoryboard = string.Empty;

        /// <summary>
        /// The slide up fade in storyboard.
        /// </summary>
        internal static readonly string SlideUpFadeInStoryboard =
            @"
        <Storyboard  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.RenderTransform).(CompositeTransform.TranslateY)"" 
                                           Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""150""/>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.35"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""6""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                </DoubleAnimationUsingKeyFrames>
            <DoubleAnimation Storyboard.TargetProperty=""(UIElement.Opacity)"" From=""0"" To=""1"" Duration=""0:0:0.350"" 
                                 Storyboard.TargetName=""LayoutRoot"">
                <DoubleAnimation.EasingFunction>
                    <ExponentialEase EasingMode=""EaseOut"" Exponent=""6""/>
                </DoubleAnimation.EasingFunction>
            </DoubleAnimation>
        </Storyboard>";

        /// <summary>
        /// The swivel full screen hide storyboard.
        /// </summary>
        internal static readonly string SwivelFullScreenHideStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationX)"" Storyboard.TargetName=""MyPopup"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""45"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseIn"" Exponent=""3""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        /// <summary>
        /// The swivel full screen show storyboard.
        /// </summary>
        internal static readonly string SwivelFullScreenShowStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationX)"" Storyboard.TargetName=""MyPopup"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""-30""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.35"" Value=""0"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseOut"" Exponent=""3""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        /// <summary>
        /// The swivel hide storyboard.
        /// </summary>
        internal static readonly string SwivelHideStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
	        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationX)"" Storyboard.TargetName=""MyPopup"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""60"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseIn"" Exponent=""3""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        /// <summary>
        /// The swivel show storyboard.
        /// </summary>
        internal static readonly string SwivelShowStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
	        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationX)"" Storyboard.TargetName=""MyPopup"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""-45""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.35"" Value=""0"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseOut"" Exponent=""3""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        /// <summary>
        /// The turnstile backward in storyboard.
        /// </summary>
        internal static readonly string TurnstileBackwardInStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
	        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationY)"" Storyboard.TargetName=""LayoutRoot"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""50""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.35"" Value=""0"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseOut"" Exponent=""6""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        /// <summary>
        /// The turnstile backward out storyboard.
        /// </summary>
        internal static readonly string TurnstileBackwardOutStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
	        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationY)"" Storyboard.TargetName=""LayoutRoot"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""-80"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseIn"" Exponent=""6""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
			<DoubleAnimationUsingKeyFrames Storyboard.TargetName=""LayoutRoot"" Storyboard.TargetProperty=""(UIElement.Opacity)"">
				<DiscreteDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0"" />
			</DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        /// <summary>
        /// The turnstile forward in storyboard.
        /// </summary>
        internal static readonly string TurnstileForwardInStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
	        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationY)"" Storyboard.TargetName=""LayoutRoot"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""-80""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.35"" Value=""0"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseOut"" Exponent=""6""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        /// <summary>
        /// The turnstile forward out storyboard.
        /// </summary>
        internal static readonly string TurnstileForwardOutStoryboard =
            @"<Storyboard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
	        <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationY)"" Storyboard.TargetName=""LayoutRoot"">
		        <EasingDoubleKeyFrame KeyTime=""0"" Value=""0""/>
		        <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""50"">
			        <EasingDoubleKeyFrame.EasingFunction>
				        <ExponentialEase EasingMode=""EaseIn"" Exponent=""6""/>
			        </EasingDoubleKeyFrame.EasingFunction>
		        </EasingDoubleKeyFrame>
	        </DoubleAnimationUsingKeyFrames>
			<DoubleAnimationUsingKeyFrames Storyboard.TargetName=""LayoutRoot"" Storyboard.TargetProperty=""(UIElement.Opacity)"">
				<DiscreteDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0"" />
			</DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        internal static readonly string Rotate180ClockwiseInStoryboard =
            @"<Storyboard
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationZ)"" Storyboard.TargetName=""LayoutRoot"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""180"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseOut"" Exponent=""1""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0""/>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetName=""LayoutRoot"" Storyboard.TargetProperty=""(UIElement.Opacity)"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <SineEase EasingMode=""EaseOut""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""1""/>
            </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        internal static readonly string Rotate180ClockwiseOutStoryboard =
            @"<Storyboard
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
           >
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationZ)"" Storyboard.TargetName=""LayoutRoot"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseIn"" Exponent=""1""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""-180""/>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetName=""LayoutRoot"" Storyboard.TargetProperty=""(UIElement.Opacity)"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""1"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <SineEase EasingMode=""EaseOut""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0""/>
            </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        internal static readonly string Rotate180CounterClockwiseInStoryboard =
            @"<Storyboard
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                >
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationZ)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""-180"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""1""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0""/>
                </DoubleAnimationUsingKeyFrames>
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)""
Storyboard.TargetName=""LayoutRoot"" >
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <SineEase EasingMode=""EaseOut""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""1""/>
                </DoubleAnimationUsingKeyFrames>
            </Storyboard>";

        internal static readonly string Rotate180CounterClockwiseOutStoryboard =
            @"<Storyboard
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            >
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationZ)"" Storyboard.TargetName=""LayoutRoot"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ExponentialEase EasingMode=""EaseIn"" Exponent=""1""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""180""/>
            </DoubleAnimationUsingKeyFrames>
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"" Storyboard.TargetName=""LayoutRoot"">
                <EasingDoubleKeyFrame KeyTime=""0"" Value=""1"">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <SineEase EasingMode=""EaseOut""/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
                <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0""/>
            </DoubleAnimationUsingKeyFrames>
        </Storyboard>";

        internal static readonly string Rotate90CounterClockwiseInStoryboard =
            @"<Storyboard
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationZ)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""-90"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""1""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0""/>
                </DoubleAnimationUsingKeyFrames>
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <SineEase EasingMode=""EaseOut""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""1""/>
                </DoubleAnimationUsingKeyFrames>
            </Storyboard>";

        internal static readonly string Rotate90CounterClockwiseOutStoryboard =
            @"<Storyboard
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationZ)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseIn"" Exponent=""1""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""90""/>
                </DoubleAnimationUsingKeyFrames>
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""1"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <SineEase EasingMode=""EaseOut""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0""/>
                </DoubleAnimationUsingKeyFrames>
            </Storyboard>";

        internal static readonly string Rotate90ClockwiseInStoryboard =
            @"<Storyboard
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationZ)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""90"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseOut"" Exponent=""1""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0""/>
                </DoubleAnimationUsingKeyFrames>
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <SineEase EasingMode=""EaseOut""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""1""/>
                </DoubleAnimationUsingKeyFrames>
            </Storyboard>";

        internal static readonly string Rotate90ClockwiseOutStoryboard =
            @"<Storyboard
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Projection).(PlaneProjection.RotationZ)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""0"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <ExponentialEase EasingMode=""EaseIn"" Exponent=""1""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""-90""/>
                </DoubleAnimationUsingKeyFrames>
                <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=""(UIElement.Opacity)"" Storyboard.TargetName=""LayoutRoot"">
                    <EasingDoubleKeyFrame KeyTime=""0"" Value=""1"">
                        <EasingDoubleKeyFrame.EasingFunction>
                            <SineEase EasingMode=""EaseOut""/>
                        </EasingDoubleKeyFrame.EasingFunction>
                    </EasingDoubleKeyFrame>
                    <EasingDoubleKeyFrame KeyTime=""0:0:0.25"" Value=""0""/>
                </DoubleAnimationUsingKeyFrames>
            </Storyboard>";
        #endregion
    }
}