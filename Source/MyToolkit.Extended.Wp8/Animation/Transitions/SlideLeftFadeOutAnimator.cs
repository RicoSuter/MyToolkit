// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlideLeftFadeOutAnimator.cs" company="XamlmNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
//   Defines the SlideLeftFadeOutAnimator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MyToolkit.Animation.Transitions
{
	public class SlideLeftFadeOutAnimator : SlideAnimator
    { 
                #region Constants and Fields

        /// <summary>
        /// The storyboard.
        /// </summary>
        private static Storyboard storyboard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideLeftFadeOutAnimator"/> class. 
        /// </summary>
        public SlideLeftFadeOutAnimator()
        {
            if (storyboard == null)
            {
                storyboard = XamlReader.Load(Storyboards.SlideLeftFadeOutStoryboard) as Storyboard;
            }

            this.Storyboard = storyboard;
        }

        #endregion
    }
}