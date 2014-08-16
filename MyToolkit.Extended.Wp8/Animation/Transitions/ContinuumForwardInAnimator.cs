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
    /// ContinuumForwardInAnimator
    /// </summary>
    public class ContinuumForwardInAnimator : ContinuumAnimator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuumForwardInAnimator"/> class.
        /// </summary>
        public ContinuumForwardInAnimator()
        {
            this.Storyboard = XamlReader.Load(Storyboards.ContinuumForwardInStoryboard) as Storyboard;
        }

        #endregion
    }
}