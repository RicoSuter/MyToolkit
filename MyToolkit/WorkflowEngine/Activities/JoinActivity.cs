//-----------------------------------------------------------------------
// <copyright file="JoinActivity.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyToolkit.WorkflowEngine.Activities
{
    /// <summary>Joins two parallel activities. </summary>
    [XmlType("Join")]
    public class JoinActivity : WorkflowActivityBase
    {
        /// <summary>Called when the previous activity has been executed. 
        /// The method may be called multiple times when there are multiple incoming transitions. </summary>
        /// <param name="input">The input. </param>
        /// <param name="definition">The workflow definition. </param>
        /// <returns>True when the activity should be automatically and immediately executed (with no args). </returns>
        internal override async Task<bool> PrepareAsync(WorkflowActivityInput input, WorkflowDefinition definition)
        {
            return !HasCurrentActivityBeforeActivity(input, definition, this, new List<WorkflowTransition>());
        }

        private bool HasCurrentActivityBeforeActivity(WorkflowActivityInput input, WorkflowDefinition definition,
            IWorkflowActivityBase activity, List<WorkflowTransition> checkedTransitions)
        {
            var inboundTransitions = definition.GetInboundTransitions(activity);
            if (inboundTransitions.Any(t => input.Instance.CurrentActivityIds.Contains(t.From)))
                return true;

            checkedTransitions.AddRange(inboundTransitions);
            foreach (var transition in inboundTransitions)
            {
                if (HasCurrentActivityBeforeActivity(input, definition, definition.GetActivityById(transition.From), checkedTransitions))
                    return true;
            }

            return false;
        }
    }
}