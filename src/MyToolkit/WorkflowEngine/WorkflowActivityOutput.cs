//-----------------------------------------------------------------------
// <copyright file="WorkflowActivityOutput.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Xml.Serialization;
using MyToolkit.WorkflowEngine.Activities;
using MyToolkit.WorkflowEngine.Exceptions;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Represents a result of an activity's execution. </summary>
    public class WorkflowActivityOutput
    {
        /// <summary>Gets or sets a value indicating whether the processing of the activity was successful. </summary>
        [XmlIgnore]
        public bool Successful { get; set; }

        /// <summary>Gets or sets the condition for the next activities (null if using the default activities from the workflow). </summary>
        [XmlIgnore]
        public string NextActivitiesCondition { get; set; }

        /// <summary>Gets the next activities or null when no condition is available. </summary>
        /// <param name="activity">The current workflow activity. </param>
        /// <param name="definition">The workflow definition. </param>
        /// <returns>The next activities. </returns>
        /// <exception cref="WorkflowException">No next activities could be found based on the given condition. </exception>
        /// <exception cref="WorkflowException">More then one activity found for condition but activity is not a ForkActivity. </exception>
        public IWorkflowActivityBase[] GetNextActivities(IWorkflowActivityBase activity, WorkflowDefinition definition)
        {
            if (string.IsNullOrEmpty(NextActivitiesCondition))
                return null; 
            
            var nextActivities = definition.GetOutboundTransitions(activity)
                .Where(t => t.Condition == NextActivitiesCondition)
                .Select(t => definition.GetActivityById(t.To))
                .ToArray();

            if (nextActivities.Length == 0)
                throw new WorkflowException(
                    string.Format("No next activities could be found based on the given condition '{0}'. ", NextActivitiesCondition));

            if (nextActivities.Length > 1 && !(activity is ForkActivity))
                throw new WorkflowException(
                    string.Format("More then one activity found for condition '{0}' " +
                                  "but activity is not a ForkActivity. ", NextActivitiesCondition));

            return nextActivities;
        }
    }
}