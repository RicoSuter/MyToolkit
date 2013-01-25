// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueExpiry.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
// Animation Types
// </summary>
// <credits>
// Kevin Marshall http://blogs.claritycon.com/blog/2010/10/13/wp7-page-transitions-sample/
// </credits>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace MyToolkit.Animation.Transitions
{
	/// <summary>
    /// The animation type.
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// Animation PivotInLeft
        /// </summary>
        PivotInLeft, 

        /// <summary>
        /// Animation PivotInRight
        /// </summary>
        PivotInRight, 

        /// <summary>
        /// Animation PivotOutLeft
        /// </summary>
        PivotOutLeft, 

        /// <summary>
        /// Animation PivotOutRight
        /// </summary>
        PivotOutRight, 

        /// <summary>
        /// Animation NavigateBackwardIn
        /// </summary>
        NavigateBackwardIn, 

        /// <summary>
        /// Animation NavigateBackwardOut
        /// </summary>
        NavigateBackwardOut, 

        /// <summary>
        /// Animation NavigateForwardIn
        /// </summary>
        NavigateForwardIn, 

        /// <summary>
        /// Animation NavigateForwardOut
        /// </summary>
        NavigateForwardOut, 

        /// <summary>
        /// Animation Appear
        /// </summary>
        Appear, 

        /// <summary>
        /// Animation Disappear
        /// </summary>
        Disappear
    }

    /// <summary>
    /// AnimatorHelperBase
    /// </summary>
    public abstract class AnimatorHelperBase
    {
        #region Constants and Fields

        /// <summary>
        /// The one time action.
        /// </summary>
        private Action oneTimeAction;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets RootElement.
        /// </summary>
        public FrameworkElement RootElement { get; set; }

        /// <summary>
        /// Gets or sets Storyboard.
        /// </summary>
        public Storyboard Storyboard { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Begins the specified completion action.
        /// </summary>
        /// <param name="completionAction">
        /// The completion action.
        /// </param>
        public virtual void Begin(Action completionAction)
        {
            if (this.Storyboard != null)
            {
                this.Storyboard.Stop();
                this.Storyboard.Begin();
                this.Storyboard.SeekAlignedToLastTick(TimeSpan.Zero);
                this.Storyboard.Completed += this.OnCompleted;
            }

            this.oneTimeAction = completionAction;
        }

        /// <summary>
        /// Sets the target.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        public void SetTarget(FrameworkElement target)
        {
            foreach (Timeline t in this.Storyboard.Children)
            {
                Storyboard.SetTarget(t, target);
            }
        }

        /// <summary>
        /// Sets the targets.
        /// </summary>
        /// <param name="targets">
        /// The targets.
        /// </param>
        /// <param name="sb">
        /// The sb.
        /// </param>
        public void SetTargets(Dictionary<string, FrameworkElement> targets, Storyboard sb)
        {
            foreach (var kvp in targets)
            {
                KeyValuePair<string, FrameworkElement> kvp1 = kvp;
                IEnumerable timelines = sb.Children.Where(t => Storyboard.GetTargetName(t) == kvp1.Key);
                foreach (Timeline t in timelines)
                {
                    Storyboard.SetTarget(t, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Sets the targets.
        /// </summary>
        /// <param name="targets">
        /// The targets.
        /// </param>
        public void SetTargets(Dictionary<string, FrameworkElement> targets)
        {
            this.SetTargets(targets, this.Storyboard);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [completed].
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void OnCompleted(object sender, EventArgs e)
        {
            this.Storyboard.Completed -= this.OnCompleted;
            Action action = this.oneTimeAction;
            if (action == null)
            {
                return;
            }

            this.oneTimeAction = null;
            action();
        }

        #endregion
    }
}