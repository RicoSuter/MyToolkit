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
    public class WorkflowActivityResult
    {
        /// <summary>Gets a value indicating whether the processing of the activity was successful. </summary>
        public bool Successful { get; private set; }

        /// <summary>Gets the next activities (if null then the default ones are loaded, meaning all transitions without condition). </summary>
        public WorkflowActivityBase[] NextActivities { get; private set; }

        /// <summary>Gets or sets the result object (only used to pass data to the caller of the <see cref="WorkflowInstance" /> object's CompleteAsync method). </summary>
        public object Result { get; private set; }

        public WorkflowActivityResult(bool successfull)
        {
            Successful = successfull;
        }

        public WorkflowActivityResult(bool successfull, object result)
        {
            Successful = successfull;
            Result = result;
        }

        public WorkflowActivityResult(bool successfull, WorkflowActivityBase[] nextActivities)
        {
            Successful = successfull;
            NextActivities = nextActivities;
        }

        public WorkflowActivityResult(bool successfull, object result, WorkflowActivityBase[] nextActivities)
        {
            Successful = successfull;
            Result = result;
            NextActivities = nextActivities;
        }

        /// <summary>Creates a new result and sets the next activities based on a condition. 
        /// The method searches for outbound transitions of the given activities with the given condition. </summary>
        /// <param name="transitionCondition">The condition of the transition to the next activity or activities 
        /// (multiple activities are only allowed when activity is a <see cref="ForkActivity"/>). </param>
        /// <param name="instance">The workflow instance. </param>
        /// <param name="activity">The current workflow activity. </param>
        /// <param name="successfull">The value indicating whether the processing was successful. </param>
        public static WorkflowActivityResult CreateByTransitionCondition(bool successfull,
            string transitionCondition, WorkflowInstance instance, WorkflowActivityBase activity)
        {
            return CreateByTransitionCondition(successfull, null, transitionCondition, instance, activity);
        }

        /// <summary>Creates a new result and sets the next activities based on a condition. 
        /// The method searches for outbound transitions of the given activities with the given condition. </summary>
        /// <param name="result">The result object. </param>
        /// <param name="transitionCondition">The condition of the transition to the next activity or activities 
        /// (multiple activities are only allowed when activity is a <see cref="ForkActivity"/>). </param>
        /// <param name="instance">The workflow instance. </param>
        /// <param name="activity">The current workflow activity. </param>
        /// <param name="successfull">The value indicating whether the processing was successful. </param>
        public static WorkflowActivityResult CreateByTransitionCondition(bool successfull, object result, 
            string transitionCondition, WorkflowInstance instance, WorkflowActivityBase activity)
        {
            var nextActivitites = instance.WorkflowDefinition.GetOutboundTransitions(activity)
                .Where(t => t.Condition == transitionCondition)
                .Select(t => instance.WorkflowDefinition.GetActivityById(t.To))
                .ToArray();

            if (nextActivitites.Length == 0)
                throw new WorkflowException(
                    string.Format("No next activities could be found based on the given condition '{0}'. ", transitionCondition));

            if (nextActivitites.Length > 1 && !(activity is ForkActivity))
                throw new WorkflowException(
                    string.Format("More then one activity found for condition '{0}' " +
                                  "but activity is not a ForkActivity. ", transitionCondition));

            return new WorkflowActivityResult(successfull, result, nextActivitites);
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