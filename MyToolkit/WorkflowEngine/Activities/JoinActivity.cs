//-----------------------------------------------------------------------
// <copyright file="JoinActivity.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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
        /// <param name="instance">The workflow instance. </param>
        /// <returns>True when the activity should be automatically and immediately executed (with no args). </returns>
        public override async Task<bool> PrepareAsync(WorkflowInstance instance)
        {
            return !HasCurrentActivityBeforeActivity(instance, this, new List<WorkflowTransition>());
        }

        private bool HasCurrentActivityBeforeActivity(WorkflowInstance instance, WorkflowActivityBase activity, List<WorkflowTransition> checkedTransitions)
        {
            var inboundTransitions = instance.WorkflowDefinition.GetInboundTransitions(activity);
            if (inboundTransitions.Any(t => instance.CurrentActivityIds.Contains(t.From)))
                return true;

            checkedTransitions.AddRange(inboundTransitions);
            foreach (var transition in inboundTransitions)
            {
                if (HasCurrentActivityBeforeActivity(instance, instance.WorkflowDefinition.GetActivityById(transition.From), checkedTransitions))
                    return true;
            }

            return false;
        }
    }
}