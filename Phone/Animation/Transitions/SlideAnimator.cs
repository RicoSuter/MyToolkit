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
using System.Windows;
using System.Windows.Media;

namespace MyToolkit.Animation.Transitions
{
	/// <summary>
    /// The slide animator.
    /// </summary>
    public class SlideAnimator : AnimatorHelperBase
    {
        #region Public Methods

        /// <summary>
        /// Begins the specified completion action.
        /// </summary>
        /// <param name="completionAction">
        /// The completion action.
        /// </param>
        public override void Begin(Action completionAction)
        {
            if (PrepareElement(this.RootElement))
            {
                this.Storyboard.Stop();
                this.SetTarget(this.RootElement);
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
        /// <returns>
        /// The prepare element.
        /// </returns>
        private static bool PrepareElement(UIElement element)
        {
            element.GetTransform<CompositeTransform>(TransformCreationMode.CreateOrAddAndIgnoreMatrix);

            return true;
        }

        #endregion
    }
}