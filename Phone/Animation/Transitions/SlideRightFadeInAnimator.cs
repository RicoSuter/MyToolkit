// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlideRightFadeInAnimator.cs" company="XamlmNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MyToolkit.Animation.Transitions
{
	/// <summary>
    /// The slide right fade in animator.
    /// </summary>
    public class SlideRightFadeInAnimator : SlideAnimator
    {
        #region Constants and Fields

        /// <summary>
        /// The storyboard.
        /// </summary>
        private static Storyboard storyboard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideRightFadeInAnimator"/> class. 
        /// </summary>
        public SlideRightFadeInAnimator()
        {
            if (storyboard == null)
            {
                storyboard = XamlReader.Load(Storyboards.SlideRightFadeInStoryboard) as Storyboard;
            }

            this.Storyboard = storyboard;
        }

        #endregion
    }
}