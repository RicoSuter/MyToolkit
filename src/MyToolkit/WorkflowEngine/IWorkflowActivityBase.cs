//-----------------------------------------------------------------------
// <copyright file="IWorkflowActivityBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>The workflow activity interface. </summary>
    public interface IWorkflowActivityBase
    {
        /// <summary>Gets or sets the activity identifier.</summary>
        string Id { get; set; }

        /// <summary>Gets the type of the input.</summary>
        Type InputType { get; }

        /// <summary>Gets the type of the output.</summary>
        Type OutputType { get; }

        /// <summary>Gets the routes.</summary>
        List<WorkflowRoute> Routes { get; }

        /// <summary>Completes the activity. </summary>
        /// <param name="input">The input. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>True when the activity has been completed. </returns>
        Task<WorkflowActivityOutput> CompleteAsync(WorkflowActivityInput input, CancellationToken cancellationToken);

        /// <summary>Called when the previous activity has been executed. 
        /// The method may be called multiple times when there are multiple incoming transitions. </summary>
        /// <param name="input">The input. </param>
        /// <param name="definition">The workflow definition. </param>
        /// <returns>True when the activity should be automatically and immediately executed (with no args). </returns>
        Task<bool> PrepareAsync(WorkflowActivityInput input, WorkflowDefinition definition);
    }
}