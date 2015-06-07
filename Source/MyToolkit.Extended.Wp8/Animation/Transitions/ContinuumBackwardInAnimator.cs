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

using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MyToolkit.Animation.Transitions
{
	/// <summary>
    /// The continuum backward in animator.
    /// </summary>
    public class ContinuumBackwardInAnimator : ContinuumAnimator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuumBackwardInAnimator"/> class.
        /// </summary>
        public ContinuumBackwardInAnimator()
        {
            this.Storyboard = XamlReader.Load(Storyboards.ContinuumBackwardInStoryboard) as Storyboard;
        }

        #endregion
    }
}