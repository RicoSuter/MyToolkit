// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateOut90ClockwiseAnimator.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
// </summary>
// <credits>
//  Kevin Marshall http://blogs.claritycon.com/blog/2010/10/13/wp7-page-transitions-sample/
// </credits>
// <credits>
//  Silverlight toolkit http://silverlight.codeplex.com/
// </credits>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MyToolkit.Animation.Transitions
{
	public class RotateOut90ClockwiseAnimator : RotateAnimator
    {
        #region Constants and Fields

        /// <summary>
        /// The storyboard.
        /// </summary>
        private static Storyboard storyboard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateOut90ClockwiseAnimator"/> class. 
        /// </summary>
        public RotateOut90ClockwiseAnimator()
        {
            if (storyboard == null)
            {
                storyboard = XamlReader.Load(Storyboards.Rotate90ClockwiseOutStoryboard) as Storyboard;
            }

            this.Storyboard = storyboard;
        }

        #endregion
    }
}