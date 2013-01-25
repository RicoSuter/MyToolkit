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
    /// The default page animator.
    /// </summary>
    public class DefaultPageAnimator : TurnstileAnimator
    {
        #region Constants and Fields

        /// <summary>
        /// The storyboard.
        /// </summary>
        private static Storyboard storyboard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPageAnimator"/> class.
        /// </summary>
        public DefaultPageAnimator()
        {
            if (storyboard == null)
            {
                storyboard = XamlReader.Load(Storyboards.DefaultStoryboard) as Storyboard;
            }

            this.Storyboard = XamlReader.Load(Storyboards.DefaultStoryboard) as Storyboard;
        }

        #endregion
    }
}