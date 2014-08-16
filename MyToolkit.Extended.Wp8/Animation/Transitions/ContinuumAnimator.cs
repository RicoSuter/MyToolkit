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

using MyToolkit.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;

namespace MyToolkit.Animation.Transitions
{
	/// <summary>
    /// ContinuumAnimator
    /// </summary>
    public class ContinuumAnimator : AnimatorHelperBase
    {
        #region Constants and Fields

        /// <summary>
        /// The popup.
        /// </summary>
        private Popup popup;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets LayoutRoot.
        /// </summary>
        public FrameworkElement LayoutRoot { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Begins the specified completion action.
        /// </summary>
        /// <param name="completionAction">
        /// The completion action.
        /// </param>
        public override void Begin(Action completionAction)
        {
            this.Storyboard.Stop();

            PrepareElement(this.LayoutRoot);

            if (this is ContinuumForwardOutAnimator)
            {
                var bitmap = new WriteableBitmap(this.RootElement, null);
                bitmap.Invalidate();
                var image = new Image { Source = bitmap, Stretch = Stretch.None };

                var rootVisual = Application.Current.RootVisual as PhoneApplicationFrame;
                this.popup = new Popup();
                if (rootVisual != null)
                {
                    var popupChild = new Canvas { Width = rootVisual.ActualWidth, Height = rootVisual.ActualHeight };

                    GeneralTransform transfrom = this.RootElement.TransformToVisual(rootVisual);
                    Point origin = transfrom.Transform(new Point(0, 0));
                    popupChild.Children.Add(image);
                    PrepareElement(image);
                    Canvas.SetLeft(image, origin.X);
                    Canvas.SetTop(image, origin.Y);

                    this.popup.Child = popupChild;
                }

                this.RootElement.Opacity = 0;
                this.popup.IsOpen = true;

                this.Storyboard.Completed += this.OnContinuumBackwardOutStoryboardCompleted;

                this.SetTargets(
                    new Dictionary<string, FrameworkElement> { { "LayoutRoot", this.LayoutRoot }, { "ContinuumElement", image } });
            }
            else
            {
                PrepareElement(this.RootElement);
                this.SetTargets(
                    new Dictionary<string, FrameworkElement> { { "LayoutRoot", this.LayoutRoot }, { "ContinuumElement", this.RootElement } });
            }

            base.Begin(completionAction);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepares the element.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        private static void PrepareElement(UIElement element)
        {
            element.GetTransform<CompositeTransform>(TransformCreationMode.CreateOrAddAndIgnoreMatrix);

            return;
        }

        /// <summary>
        /// Called when [continuum backward out storyboard completed].
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void OnContinuumBackwardOutStoryboardCompleted(object sender, EventArgs e)
        {
            this.Storyboard.Completed -= this.OnContinuumBackwardOutStoryboardCompleted;
            this.popup.IsOpen = false;
            this.popup.Child = null;
            this.popup = null;
        }

        #endregion
    }
}