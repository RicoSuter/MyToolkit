// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlideRightFadeOutAnimator.cs" company="XamlmNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MyToolkit.Animation.Transitions
{
	/// <summary>
    /// The slide right fade out animator.
    /// </summary>
    public class SlideRightFadeOutAnimator : SlideAnimator
    {
        #region Constants and Fields

        /// <summary>
        /// The storyboard.
        /// </summary>
        private static Storyboard storyboard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideRightFadeOutAnimator"/> class. 
        /// </summary>
        public SlideRightFadeOutAnimator()
        {
            if (storyboard == null)
            {
                storyboard = XamlReader.Load(Storyboards.SlideRightFadeOutStoryboard) as Storyboard;
            }

            this.Storyboard = storyboard;
        }

        #endregion
    }
}