//-----------------------------------------------------------------------
// <copyright file="IWorkflowActivityBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyToolkit.WorkflowEngine
{
    public interface IWorkflowActivityBase
    {
        string Id { get; set; }

        Type InputType { get; }

        Type OutputType { get; }

        List<WorkflowRoute> Routes { get; }

        Task<WorkflowActivityOutput> CompleteAsync(WorkflowActivityInput input, CancellationToken cancellationToken);

        Task<bool> PrepareAsync(WorkflowActivityInput input, WorkflowDefinition definition);
    }
}