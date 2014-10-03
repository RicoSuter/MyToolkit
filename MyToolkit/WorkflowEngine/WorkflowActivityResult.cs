//-----------------------------------------------------------------------
// <copyright file="WorkflowActivityResult.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using MyToolkit.WorkflowEngine.Activities;
using MyToolkit.WorkflowEngine.Exceptions;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Represents a result of an activity's execution. </summary>
    public class WorkflowActivityResult
    {
        /// <summary>Gets a value indicating whether the processing of the activity was successful. </summary>
        public bool Successful { get; private set; }

        /// <summary>Gets the condition for the next activities (null if using the default activities from the workflow). </summary>
        public string NextActivitiesCondition { get; private set; }

        /// <summary>Gets or sets the result object (only used to pass data to the caller of the <see cref="WorkflowInstance" /> object's CompleteAsync method). </summary>
        public object Result { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="WorkflowActivityResult"/> class. </summary>
        public WorkflowActivityResult(bool successfull)
        {
            Successful = successfull;
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowActivityResult"/> class. </summary>
        public WorkflowActivityResult(bool successfull, object result)
        {
            Successful = successfull;
            Result = result;
        }

        /// <summary>Gets the next activities or null when no condition is available. </summary>
        /// <param name="activity">The current workflow activity. </param>
        /// <param name="definition">The workflow definition. </param>
        /// <returns>The next activities. </returns>
        public WorkflowActivityBase[] GetNextActivities(WorkflowActivityBase activity, WorkflowDefinition definition)
        {
            if (string.IsNullOrEmpty(NextActivitiesCondition))
                return null; 
            
            var nextActivitites = definition.GetOutboundTransitions(activity)
                .Where(t => t.Condition == NextActivitiesCondition)
                .Select(t => definition.GetActivityById(t.To))
                .ToArray();

            if (nextActivitites.Length == 0)
                throw new WorkflowException(
                    string.Format("No next activities could be found based on the given condition '{0}'. ", NextActivitiesCondition));

            if (nextActivitites.Length > 1 && !(activity is ForkActivity))
                throw new WorkflowException(
                    string.Format("More then one activity found for condition '{0}' " +
                                  "but activity is not a ForkActivity. ", NextActivitiesCondition));

            return nextActivitites;
        }

        /// <summary>Gets the typed result (simply casts and returns the <see cref="Result"/> property). </summary>
        /// <typeparam name="T">The type of the result. </typeparam>
        /// <returns>The result. </returns>
        public T GetResult<T>()
        {
            return (T)Result;
        }
    }
}