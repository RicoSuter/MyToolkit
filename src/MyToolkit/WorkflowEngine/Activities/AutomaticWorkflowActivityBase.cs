//-----------------------------------------------------------------------
// <copyright file="AutomaticWorkflowActivityBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace MyToolkit.WorkflowEngine.Activities
{
    /// <summary>An abstract class for an activity which is automatically completed: 
    /// The <see cref="IWorkflowActivityBase.CompleteAsync"/> method gets directly called after <see cref="PrepareAsync"/>. </summary>
    public abstract class AutomaticWorkflowActivityBase : WorkflowActivityBase
    {
        /// <summary>Called when the previous activity has been executed. 
        /// The method may be called multiple times when there are multiple incoming transitions. </summary>
        /// <param name="input">The input. </param>
        /// <returns>True when the activity should be automatically and immediately executed (with no args). </returns>
        public override Task<bool> PrepareAsync(WorkflowActivityInput input)
        {
            return Task.FromResult(true);
        }
    }
}