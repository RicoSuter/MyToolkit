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
    /// The continuum backward out animator.
    /// </summary>
    public class ContinuumBackwardOutAnimator : ContinuumAnimator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuumBackwardOutAnimator"/> class.
        /// </summary>
        public ContinuumBackwardOutAnimator()
        {
            this.Storyboard = XamlReader.Load(Storyboards.ContinuumBackwardOutStoryboard) as Storyboard;
        }

        #endregion
    }
}