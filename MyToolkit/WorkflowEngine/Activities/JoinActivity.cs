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
    [XmlType("Join")]
    public class JoinActivity : AutomaticWorkflowActivityBase
    {
        public override Task<bool> PrepareAsync(WorkflowInstance instance)
        {
            return Task.FromResult(!HasCurrentActivityBeforeActivity(instance, this, new List<WorkflowTransition>()));
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