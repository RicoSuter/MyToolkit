// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlideLeftFadeInAnimator.cs" company="XamlmNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MyToolkit.Animation.Transitions
{
	/// <summary>
    /// The slide left fade in animator.
    /// </summary>
    public class SlideLeftFadeInAnimator : SlideAnimator
    {
        #region Constants and Fields

        /// <summary>
        /// The storyboard.
        /// </summary>
        private static Storyboard storyboard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideLeftFadeInAnimator"/> class. 
        /// </summary>
        public SlideLeftFadeInAnimator()
        {
            if (storyboard == null)
            {
                storyboard = XamlReader.Load(Storyboards.SlideLeftFadeInStoryboard) as Storyboard;
            }

            this.Storyboard = storyboard;
        }

        #endregion
    }
}